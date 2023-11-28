using GridPromocional.Models;
using GridPromocional.Models.Views;

namespace GridPromocional.Services.Interfaces
{
    public interface ISKUMasterServices
    {
        public bool newProductSKU(string Code);
        public List<SKUMasterView> getSKUsNoGenerate(int? numReport = null);
        public List<SKUMasterView> generateSKUs(string codemp);
        public List<SKUMasterView> exportSKUs();
    }
}
