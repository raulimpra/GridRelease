using CsvHelper.Configuration.Attributes;
using GridPromocional.Resources;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace GridPromocional.Models.Errors
{
    public partial class PgCatProductsErrors: PgCatProducts
    {
        public PgCatProductsErrors()
        {
            CodeError = "";
            DescriptionError = "";
            FamilyError = "";
            MaterialTypeError = "";
            StatusError = "";
            ProjectError = "";
        }
        public string CodeError { get; set; }
        public string DescriptionError { get; set; }
        public string FamilyError { get; set; }
        public string MaterialTypeError { get; set; }
        public string StatusError { get; set; }
        public string ProjectError { get; set; }
    }
}
