using GridPromocional.Models;
using System.Text;

namespace GridPromocional.Services
{
    public interface ICsvService<T, S> where T : class where S : UploadError
    {
        public Dictionary<string, object> Parameters { get; }
        public Dictionary<string, Dictionary<string, object>> Lookups { get; }
        public Dictionary<string, Func<Register<T, S>, object?, object>> Transformations { get; }

        public List<Register<T, S>> Registers { get; }
        public DateTime ProcessDate { get; }

        public byte[] WriteData(IEnumerable<object> records, Encoding? encoding = null);

        public Task<List<Register<T, S>>> GetRegisters(Stream stream, Encoding? encoding = null);

        public Register<T, S> AddErrorRegister(string column, Exception ex);

        public IEnumerable<S> GetSourceRecords(bool? hasErrors);
        public IEnumerable<Register<T, S>> GetCorrectRegisters();
    }
}