namespace GridPromocional.Models.Views
{
    public class ViewProducts
    {
        public ViewProducts()
        {
            family = new List<PgCatFamily>();
            material = new List<PgCatMaterialType>();
            status = new List<PgCatStatusProducts>();
        }
        public List<PgCatFamily> family { get; set; }
        public List<PgCatMaterialType> material { get; set; }
        public List<PgCatStatusProducts> status { get; set; }
    }
}
