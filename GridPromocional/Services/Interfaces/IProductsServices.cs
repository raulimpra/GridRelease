using GridPromocional.Models;
using GridPromocional.Models.Views;

namespace GridPromocional.Services.Interfaces
{
    public interface IProductsServices
    {
        public bool AddProducts(PgCatProducts element, string username);
        public List<ViewInventories> getInventories(ViewInventories element, string username);
        public bool changeStatus(string code);
    }
}
