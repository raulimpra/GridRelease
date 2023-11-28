using ClosedXML;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.Excel;
using CsvHelper.TypeConversion;
using GridPromocional.Exceptions;
using GridPromocional.Models;
using GridPromocional.Resources;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using MissingFieldException = CsvHelper.MissingFieldException;

namespace GridPromocional.Services
{
    public class CsvService<T, S> : ICsvService<T, S> where T : class where S : UploadError
    {
        private const string ASCII_VALIDATION_KEYWORD = nameof(Messages.ErrorReAsciiUpercase);
        private readonly ClaimsIdentity? _identity;
        public readonly ILogger<CsvService<T, S>> _logger;
        private readonly CultureInfo _cultureInfo;

        private int _rowOffset;
        private readonly Type _typeT;
        private readonly Type _typeS;
        private readonly Type _typeR;
        private readonly IEnumerable<PropertyInfo> _propertiesT;
        private readonly IEnumerable<PropertyInfo> _propertiesS;
        private readonly Dictionary<PropertyInfo, PropertyInfo> _propertyMapT2S;
        private readonly Dictionary<PropertyInfo, ColumnValidation> _propertyValidations = new();

        public Dictionary<string, object> Parameters { get; } = new();
        public Dictionary<string, Dictionary<string, object>> Lookups { get; } = new();
        public Dictionary<string, Func<Register<T, S>, object?, object>> Transformations { get; } = new();

        public List<Register<T, S>> Registers { get; } = new();
        public DateTime ProcessDate { get; } = DateTime.Now;

        public CsvService(IHttpContextAccessor httpContextAccessor, ILogger<CsvService<T, S>> logger)
        {
            _identity = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            _logger = logger;

            _typeT = typeof(T);
            _typeS = typeof(S);
            _typeR = typeof(Register<,>).MakeGenericType(new Type[] { _typeT, _typeS });

            // Properties needed to upload the record to the target table
            _propertiesT = _typeT.GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(ColumnAttribute)));
            // Properties needed to read a row from the source
            var errorProperties = typeof(UploadError).GetProperties().Select(x => x.Name).ToList();
            _propertiesS = _typeS.GetProperties()
                .Where(p => !Attribute.IsDefined(p, typeof(IgnoreAttribute)) && !errorProperties.Contains(p.Name))
                .OrderBy(p => p.GetCustomAttribute<IndexAttribute>()?.Index);

            // Map target properties to source headers
            _propertyMapT2S = _propertiesT
                .Join(_propertiesS, t => t.Name, s => s.Name, (t, s) => new { t, s })
                .ToDictionary(x => x.t, x => x.s);

            _cultureInfo = new CultureInfo("es-mx");
            CultureInfo.DefaultThreadCurrentCulture = _cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = _cultureInfo;
        }

        public byte[] WriteData(IEnumerable<object> records, Encoding? encoding = null)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, encoding ?? Encoding.UTF8);
            using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
            csvWriter.WriteRecords(records);
            streamWriter.Flush();
            return memoryStream.ToArray();
        }

        public async Task<List<Register<T, S>>> GetRegisters(Stream stream, Encoding? encoding = null)
        {
            var archivo = Parameters.ContainsKey("Filename") ? (string)Parameters["Filename"] : String.Empty;

            try
            {
                Registers.Clear();

                // Internal configuration validation
                ValidateParameters();

                CsvConfiguration conf = new(CultureInfo.InvariantCulture)
                {
                    // PrepareHeaderForMatch = args => args.Header.ToUpper(),
                    ReadingExceptionOccurred = ReadingExceptionOccurred,
                    DetectColumnCountChanges = true,
                    Encoding = encoding ?? Encoding.Latin1
                };

                await Task.Run(() =>
                {
                    if (archivo.EndsWith("xlsx"))
                        ReadFromExcel(stream, conf);
                    else
                        ReadFromCsv(stream, conf);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fatal procesando '{archivo}'.", archivo);

                AddErrorRegister(string.Empty, new GridException("Error fatal procesando archivo.", ex));
            }

            return Registers;
        }

        private void ReadFromExcel(Stream stream, CsvConfiguration conf)
        {
            using var parser = new ExcelParser(stream, null, conf);
            using var csv = new CsvReader(parser);
            _rowOffset = -1; // after reading the row it is already pointing to start of next

            Read(csv);
        }

        private void ReadFromCsv(Stream stream, CsvConfiguration conf)
        {
            using StreamReader reader = new(stream, Encoding.Latin1);
            using CsvReader csv = new(reader, conf);
            _rowOffset = 0;

            Read(csv);
        }

        private void Read(CsvReader csv)
        {
            // Load rows if header is valid
            if (csv.Read() && ValidateHeader(csv))
            {
                UpdatePropertyMappings(csv);

                while (csv.Parser.Read())
                {
                    ReadRow(csv);
                }
            }
        }

        /// <summary>
        /// Updates property mappings to remove Optional properties that are
        /// not provided by current file
        /// </summary>
        /// <param name="csv"></param>
        private void UpdatePropertyMappings(CsvReader csv)
        {
            foreach (var propertyT in _propertyMapT2S.Keys)
            {
                var propertyS = _propertyMapT2S[propertyT];
                var colName = propertyS.GetCustomAttribute<NameAttribute>()?.Names[0] ?? propertyS.Name;
                if (Attribute.IsDefined(propertyS, typeof(OptionalAttribute)) && csv.HeaderRecord?.Contains<string>(colName) != true)
                {
                    _propertyMapT2S.Remove(propertyT);
                }
            }
        }

        /// <summary>
        /// Validate that target class properties are assigned from the source
        /// class or the Parameters
        /// </summary>
        /// <exception cref="GridException">Triggers a fatal exception if there are errors</exception>
        private void ValidateParameters()
        {
            var missing = _propertiesT.Select(p => p.Name)
                .Except(_propertiesS.Select(p => p.Name))
                .Except(Transformations.Keys)
                .Except(Parameters.Keys);

            if (missing.Any())
            {
                string names = missing.Aggregate((x, y) => x + "', '" + y);
                throw new GridException($"No se definió una fuente para asignar los siguientes campos: '{names}'.");
            }
        }

        /// <summary>
        /// Validate that the file has the required columns
        /// </summary>
        /// <param name="csv"></param>
        /// <returns>True keep reading, false abort</returns>
        /// <exception cref="GridException"></exception>
        private bool ValidateHeader(CsvReader csv)
        {
            bool ok = true;

            try
            {
                if (csv.Context.Configuration.HasHeaderRecord)
                {
                    ok = csv.ReadHeader();
                    csv.ValidateHeader<S>();
                }
            }
            catch (HeaderValidationException ex)
            {
                ok = false;
                var headers = csv.HeaderRecord;
                var missingHeaders = ex.InvalidHeaders.Select(x => $"'{x.Names[0]}'");
                string names = missingHeaders.Aggregate((x, y) => x + ", " + y);

                var record = AddRegister(csv).AddError(names, new GridException("Error fatal, Encabezados faltantes.", ex)).Source;

                foreach (var property in _propertiesS)
                {
                    var colName = property.GetCustomAttribute<NameAttribute>()?.Names[0] ?? property.Name;
                    if (headers?.Contains(colName) == true)
                        property.SetValue(record, colName);
                }
            }

            return ok;
        }

        /// <summary>
        /// Read a new row using the helper, Parameters and Lookup tables and
        /// extract a Register.
        /// </summary>
        /// <param name="csv"></param>
        /// <returns></returns>
        private void ReadRow(CsvReader csv)
        {
            var register = AddRegister(csv);

            // Read all fields, stores all erros
            var properties = _propertiesT.OrderBy(p => Transformations.ContainsKey(p.Name));
            foreach (PropertyInfo propertyT in properties)
            {
                ReadField(csv, propertyT, register);
            }
        }

        /// <summary>
        /// Get value from all posible sources: csv field, parameter, foreign key
        /// </summary>
        /// <param name="csv"></param>
        /// <param name="propertyT"></param>
        /// <param name="register"></param>
        /// <returns></returns>
        private object? ReadField(CsvReader csv, PropertyInfo propertyT, Register<T, S> register)
        {
            object? valor = null;
            object? valorS = null;
            string name = propertyT.Name;
            string? columnS = null;

            try
            {
                // Read from csv, if it's there
                if (_propertyMapT2S.TryGetValue(propertyT, out var propertyS))
                {
                    columnS = propertyS.GetCustomAttribute<NameAttribute>()?.Names[0] ?? propertyS.Name;
                    valorS = valor = csv.GetField<string>(columnS);
                    AssignValue(register.Source, propertyS, valor);
                }

                // if null, read from Parameters, if it's there
                if (valor == null && Parameters.ContainsKey(name))
                    valor = Parameters[name];

                // Apply transformations
                if (Transformations.ContainsKey(name))
                    valor = Transformations[name](register, valor);

                // Replace description with foreign key, if a lookup table is defined
                if (Lookups.ContainsKey(name))
                    valor = GetForeignKey(name, valor);

                // Validate source and target
                if (propertyS != null)
                {
                    valorS = ValidateAnnotations(register.Source, propertyS, valorS, true);
                    AssignValue(register.Source, propertyS, valorS);
                }

                valor = ValidateAnnotations(register.Target, propertyT, valor);
                var converted = AssignValue(register.Target, propertyT, valor);

                return valor;
            }
            catch (Exception ex)
            {
                var columnT = propertyT.GetCustomAttribute<NameAttribute>()?.Names[0] ?? name;
                register.AddError(columnS ?? columnT, ex);
            }

            return valor;
        }

        /// <summary>
        /// Create instance of a type, thorw Exception instead of returning null
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="GridException"></exception>
        private static object CreateInstance(Type type)
        {
            try
            {
                return Activator.CreateInstance(type) ?? throw new GridException("Instancia nula.");
            }
            catch (Exception ex)
            {
                throw new GridException($"Error faltal, no puedo crear instancia de type '{GetTypeName(type)}'.", ex);
            }
        }

        /// <summary>
        /// Create instance of a Register, and its Source and Target
        /// </summary>
        /// <returns></returns>
        /// <exception cref="GridException"></exception>
        private Register<T, S> CreateRegisterInstance()
        {
            try
            {
                var register = (Register<T, S>)CreateInstance(_typeR);
                register.Source = (S)CreateInstance(_typeS);
                register.Target = (T)CreateInstance(_typeT);

                return register;
            }
            catch (Exception ex)
            {
                throw new GridException("Error faltal, no puedo crear instancia de registro.", ex);
            }
        }

        /// <summary>
        /// Clone and instance of a source model
        /// Properties must be base types, objects wont be cloned recursively
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private S CloneSource(S source)
        {
            S result = (S)CreateInstance(_typeS);

            foreach (var property in _typeS.GetProperties())
                property.SetValue(result, property.GetValue(source, null));

            return result;
        }

        /// <summary>
        /// Create a Register with additional source information and add it to the list.
        /// </summary>
        /// <param name="csv">row information</param>
        private Register<T, S> AddRegister(CsvReader? csv)
        {
            var register = CreateRegisterInstance();

            S source = register.Source;
            source.Username = _identity?.Name;
            source.ModelName = _typeS.Name;
            source.Filename = (string)Parameters["Filename"];
            source.UploadDate = ProcessDate;
            source.Row = csv?.Parser.Row + _rowOffset;
            source.Column = null;
            source.Error = null;
            source.Exception = null;

            Registers.Add(register);

            return register;
        }

        public Register<T, S> AddErrorRegister(string column, Exception ex)
        {
            return AddRegister(null).AddError(column, ex);
        }

        /// <summary>
        /// Get source records, filter by hasErrors, include all if null.
        /// Errors are flatten, there'll be a source record per error.
        /// </summary>
        /// <param name="hasErrors"></param>
        /// <returns></returns>
        public IEnumerable<S> GetSourceRecords(bool? hasErrors)
        {
            foreach (var register in Registers)
            {
                var source = register.Source;

                if (hasErrors != true && !register.HasErrors())
                    yield return source;

                if (hasErrors != false)
                {
                    foreach (var pair in register.Errors)
                    {
                        S error = CloneSource(source);

                        error.Column = pair.Key;
                        Exception ex = pair.Value;

                        error.Error = TranslatErrorMessage(ex);
                        error.Exception = ex.ToString();

                        yield return error;
                    }
                }
            }
        }

        /// <summary>
        /// Get a list of correct target registers.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Register<T, S>> GetCorrectRegisters()
        {
            return Registers.Where(x => !x.HasErrors());
        }

        /// <summary>
        /// Assigns the value to the property of data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <exception cref="GridException">Friendly error message</exception>
        private object? AssignValue(object data, PropertyInfo property, object? value)
        {
            try
            {
                object? converted = ConvertTo(property.PropertyType, value);
                property.SetValue(data, converted);

                return converted;
            }
            catch (Exception ex)
            {
                string typeName = GetTypeName(property.PropertyType);
                throw new GridException($"Valor '{value}' no es del Tipo '{typeName}'.", ex);
            }
        }

        /// <summary>
        /// Get the foreign key value for the property name and value
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        /// <exception cref="GridException">Friendly error message</exception>
        private object GetForeignKey(string name, object? valor)
        {
            var lookup = Lookups[name];
            string? desc = valor?.ToString();

            if (desc != null && lookup.ContainsKey(desc))
                return lookup[desc];
            else
                throw new GridException($"Valor '{desc}' sin llave foránea en catálogo.");
        }

        /// <summary>
        /// Converto fieldValue to given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        /// <exception cref="GridException"></exception>
        private object? ConvertTo(Type type, object? fieldValue)
        {
            if (fieldValue == null)
                return null;
            else if (type.IsInstanceOfType(fieldValue))
                return fieldValue;
            else
                return TypeDescriptor.GetConverter(type).ConvertFrom(null, _cultureInfo, fieldValue);
        }

        /// <summary>
        /// Get the base type name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetTypeName(Type type)
        {
            return Nullable.GetUnderlyingType(type)?.Name ?? type.Name;
        }

        /// <summary>
        /// Validate, and fix if possible, de values against the annotations
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyT"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="GridException"></exception>
        private object? ValidateAnnotations(object entity, PropertyInfo propertyT, object? value, bool autoTruncate = false)
        {
            ColumnValidation validation;
            if (!_propertyValidations.ContainsKey(propertyT))
            {
                validation = new()
                {
                    Context = new ValidationContext(entity) { MemberName = propertyT.Name },
                    Attributes = propertyT.GetCustomAttributes<ValidationAttribute>().ToList(),
                    Results = new()
                };
                validation.IsUpperCase = validation.Attributes.Any(x => x.GetType() == typeof(RegularExpressionAttribute)
                    && x.ErrorMessageResourceName?.Equals(ASCII_VALIDATION_KEYWORD) == true);

                validation.TruncateToStringLenght = autoTruncate && validation.Attributes
                    .SingleOrDefault(x => x.GetType() == typeof(StringLengthAttribute)) is StringLengthAttribute att ? att.MaximumLength : 0;

                _propertyValidations.Add(propertyT, validation);
            }
            else
            {
                validation = _propertyValidations[propertyT];
                validation.Results.Clear();
            }

            // String fix
            if (value is string str && value != null)
            {
                // Convert to UpperCase
                if (validation.IsUpperCase)
                    str = str.ToUpperInvariant();
                // Truncate if necesary
                if (validation.TruncateToStringLenght > 0 && str.Length > validation.TruncateToStringLenght)
                    str = str[..validation.TruncateToStringLenght];
                // Remove ASCII 160
                value = str.Replace(((char)160).ToString(), string.Empty);
            }

            #pragma warning disable CS8604 // Possible null reference argument.
            if (!Validator.TryValidateValue(value, validation.Context, validation.Results, validation.Attributes))
                throw new GridException($"Valor '{value}' {validation.Results[0].ErrorMessage}.");
            #pragma warning restore CS8604 // Possible null reference argument.

            return value;
        }

        /// <summary>
        /// Trap reading exceptions and throw a new exception with a translated message.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="GridException">Friendly error message</exception>
        public bool ReadingExceptionOccurred(ReadingExceptionOccurredArgs args)
        {
            throw new GridException(TranslatErrorMessage(args.Exception), args.Exception);
        }

        /// <summary>
        /// Translate helper exception messages
        /// </summary>
        /// <param name="csvEx"></param>
        /// <returns></returns>
        private static string TranslatErrorMessage(Exception csvEx)
        {
            switch (csvEx)
            {
                case TypeConverterException:
                    TypeConverterException converterEx = (TypeConverterException)csvEx;
                    MemberMapData memberMapData = converterEx.MemberMapData;
                    return $"Convirtiendo el valor '{converterEx.Text}' de la columna '{memberMapData.Member?.Name}'" +
                        $" tipo '{GetTypeName(memberMapData.Member.MemberType())}'.";

                case BadDataException:
                    //return $"Formato de campo no válido '{dataEx..Field}'.";
                    return "Formato de campo no válido.";

                case MissingFieldException:
                    return "Campo no encontrado.";

                case FieldValidationException:
                    FieldValidationException validationEx = (FieldValidationException)csvEx;
                    return $"Validación de dato incorrecto detectado en campo '{validationEx.Field}'.";

                case GridException:
                    GridException gridEx = (GridException)csvEx;
                    return gridEx.ToStringGrid();

                default:
                    return "Error procesando archivo.";
            }
        }
    }
}
