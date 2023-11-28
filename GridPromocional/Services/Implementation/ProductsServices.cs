using DocumentFormat.OpenXml.Spreadsheet;
using GridPromocional.Data;
using GridPromocional.Exceptions;
using GridPromocional.Models;
using GridPromocional.Models.Views;
using GridPromocional.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Xml.Linq;

namespace GridPromocional.Services.Implementation
{
    public class ProductsServices : IProductsServices
    {
        private readonly GridContext _context;
        public ProductsServices(GridContext context)
        {
            _context = context;
        }

        public bool AddProducts(PgCatProducts element, string username)
        {
            try
            {
                string code = "";
                string consecutive = "";
                if (element.IdType.Contains("M"))
                {
                    var catProd = _context.PgCatProducts.Where(x => !x.Code.Contains("MX016883"));
                    if (catProd.Any())
                    {
                        var prod = catProd.OrderByDescending(x => x.Code).FirstOrDefault();
                        code = catProd != null ? prod.Code : "60000000000000";
                        consecutive = (Int32.Parse(code) + 1).ToString();
                    }
                }
                else
                {
                    var catProd = _context.PgCatProducts.Where(x => x.Code.Contains("MX"));
                    if (catProd.Any())
                    {
                        var prod = catProd.OrderByDescending(x => x.Code).FirstOrDefault();
                        code = prod != null ? prod.Code : "MX01688300000";
                        while (code.Length < 13) code += "0";
                        consecutive = "MX0" + (Int32.Parse(code.Substring(2)) + 1);
                    }

                }
                element.Code = consecutive;
                element.IdType = element.IdType == null ? "" : element.IdType;
                element.IdFam = element.IdFam == null ? "" : element.IdFam;
                _context.PgCatProducts.Add(new PgCatProducts()
                {
                    Code = element.Code,
                    IdType = element.IdType,
                    IdFam = element.IdFam,
                    Description = element.Description,
                    Project = element.Project,
                    IdSt = element.IdSt,
                    Status = true
                });
                DateTime dateNow = DateTime.Now;
                _context.SaveChanges();
                _context.PgLogActivity.Add(new PgLogActivity()
                {
                    Activity = "Carga Manual",
                    Filename = element.Code,
                    Username = username,
                    Date = dateNow
                });
                _context.PgLogUploadHistory.Add(new PgLogUploadHistory()
                {
                    Filename = element.Code,
                    ModelName = "PgCatProducts",
                    TotalErrors = 0,
                    TotalRegisters = 1,
                    UploadDate = dateNow,
                    WrongRegisters = 0,
                    UploadedRegisters = 0,
                    Username = username,
                });
                _context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public bool changeStatus(string code)
        {
            try
            {
                PgCatProducts product = _context.PgCatProducts.FirstOrDefault(x=>x.Code == code);
                product.Status = !product.Status;
                _context.PgCatProducts.Update(product);
                _context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message,ex);
            }
        }

        public List<ViewInventories> getInventories(ViewInventories element, string username)
        {
            List< ViewInventories > list = new List< ViewInventories >();
            try
            {
                var result = _context.ViewInventories.FromSqlRaw("GetViewInventories {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
                                    (object)element.Code ?? DBNull.Value,
                                    (object)element.Family ?? DBNull.Value,
                                    (object)element.Material ?? DBNull.Value,
                                    (object)element.Project ?? DBNull.Value,
                                    (object)element.Status ?? DBNull.Value,
                                    (object)element.Description ?? DBNull.Value,
                                    (object)element.Active?? DBNull.Value,
                                    username).ToList();
                return result;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
