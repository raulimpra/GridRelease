using DocumentFormat.OpenXml.InkML;
using GridPromocional.Data;
using GridPromocional.Models;
using GridPromocional.Models.Views;
using GridPromocional.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace GridPromocional.Services.Implementation
{
    public class CatalogServices : ICatalogServices
    {
        private readonly GridContext _context;
        public CatalogServices(GridContext context)
        {
            _context = context;
        }
        public ViewProducts getCatalogs(string codemp = null, string material = "", string status = "")
        {
            try
            {
                var materialList = _context.PgCatMaterialType.ToList();
                materialList.Remove(materialList.FirstOrDefault(x => x.IdType == material));
                var statusList = _context.PgCatStatusProducts.ToList();
                statusList.Remove(statusList.FirstOrDefault(x => x.IdSt == status));
                return new ViewProducts()
                {
                    family = getFamiliesByUser(codemp),
                    status = statusList,
                    material = materialList
                };
            }
            catch(Exception ex)
            {
                return new ViewProducts()
                {
                    family = new List<PgCatFamily>(),
                    status = new List<PgCatStatusProducts>(),
                    material = new List<PgCatMaterialType>()
                };
            }
        }

        public List<PgCatFamily> getFamiliesByUser(string codemp)
        {
            List<PgCatFamily> list = new List<PgCatFamily>();
            try
            {
                var result = _context.PgCatFamily.FromSqlRaw("getPGCatFamilyByUser {0}",
                (object)codemp ?? DBNull.Value).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
