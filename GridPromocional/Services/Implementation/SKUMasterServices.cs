using GridPromocional.Data;
using GridPromocional.Models;
using GridPromocional.Models.Views;
using GridPromocional.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace GridPromocional.Services.Implementation
{
    public class SKUMasterServices : ISKUMasterServices
    {
        private readonly GridContext _context;
        public SKUMasterServices(GridContext context)
        {
            _context = context;
        }
        public bool newProductSKU(string Code)
        {
            try
            {
                _context.Add(new PgSKUMaster() { Code = Code });
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public List<SKUMasterView> getSKUsNoGenerate(int? numReport = null)
        {
            try
            {
                var result = _context.SKUMasterView.FromSqlRaw("GetSKUMaster {0}",
                                    (object)numReport ?? DBNull.Value).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public List<SKUMasterView> exportSKUs()
        {
            List<PgSKUMaster> skusNoGenerated = new List<PgSKUMaster>();
            try
            {
                var lastReport = _context.PgSKUMaster.OrderBy(x => x.NumReport).FirstOrDefault();
                int numReport = lastReport.NumReport ?? 0;

                var result = getSKUsNoGenerate(numReport);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public List<SKUMasterView> generateSKUs(string codemp)
        {
            List<PgSKUMaster> skusNoGenerated = new List<PgSKUMaster>();
            try
            {
                var result = _context.SKUMasterView.FromSqlRaw("GetSKUMaster").ToList();
                skusNoGenerated = _context.PgSKUMaster.Where(x=>x.DateGenerate == null).ToList();

                var lastReport = _context.PgSKUMaster.OrderBy(x => x.NumReport).FirstOrDefault();
                int numReport = lastReport.NumReport ?? 0;
                foreach (var skus in skusNoGenerated)
                {
                    skus.UserGenerate = codemp;
                    skus.DateGenerate = DateTime.Now;
                    skus.NumReport = numReport + 1;
                    _context.PgSKUMaster.Update(skus);
                }
                _context.SaveChanges();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
