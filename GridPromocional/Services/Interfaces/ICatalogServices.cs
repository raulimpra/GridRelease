using GridPromocional.Models;
using GridPromocional.Models.Views;

namespace GridPromocional.Services.Interfaces
{
    public interface ICatalogServices
    {
        public ViewProducts getCatalogs(string codemp = null, string material = "", string status = "");

        public List<PgCatFamily> getFamiliesByUser(string codemp);
    }
}
