#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using GridPromocional.Resources;

namespace GridPromocional.Models
{
    [Table("PG_cat_sales_force")]
    [DisplayName("Representantes")]
    public partial class PgCatSalesForce
    {
        [Name("Customer")]
        [Column("CUSTOMER")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(15, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Customer { get; set; }

        [Name("Search Term 1")]
        [Column("SEARCH_TERM1")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(8, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string SearchTerm1 { get; set; }

        [Key]
        [Name("Seerch Term 2")]
        [Column("SEARCH_TERM2")]
        [StringLength(10, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string SearchTerm2 { get; set; }

        [Name("Name 1")]
        [Column("NAME1")]
        [StringLength(35, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
		[RegularExpression("^[ -~¡-ÿ]*$", ErrorMessageResourceName = nameof(Messages.ErrorReAsciiUpercase), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Name1 { get; set; }

        [Name("Name 2")]
        [Column("NAME2")]
        [StringLength(35, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
		[RegularExpression("^[ -~¡-ÿ]*$", ErrorMessageResourceName = nameof(Messages.ErrorReAsciiUpercase), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Name2 { get; set; }

        [Name("Sales Organization")]
        [Column("SALES_ORGANIZATION")]
        [StringLength(4, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string SalesOrganization { get; set; }

        [Name("Street")]
        [Column("STREET")]
        [StringLength(35, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
		[RegularExpression("^[ -~¡-ÿ]*$", ErrorMessageResourceName = nameof(Messages.ErrorReAsciiUpercase), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Street { get; set; }

        [Name("Street 2")]
        [Column("STREET2")]
        [StringLength(35, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Street2 { get; set; }

        [Name("Region")]
        [Column("REGION")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(3, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Region { get; set; }

        [Name("Postal Code")]
        [Column("POSTAL_CODE")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        // [StringLength(5, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [RegularExpression(@"^\d{5}$", ErrorMessageResourceName = nameof(Messages.ErrorZipCode), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string PostalCode { get; set; }

        [Name("City")]
        [Column("CITY")]
        [StringLength(35, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
		[RegularExpression("^[ -~¡-ÿ]*$", ErrorMessageResourceName = nameof(Messages.ErrorReAsciiUpercase), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string City { get; set; }

        [Name("Telephone")]
        [Column("TELEPHONE")]
        // [StringLength(10, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [RegularExpression(@"^\d{10}$", ErrorMessageResourceName = nameof(Messages.ErrorTelephone), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Telephone { get; set; }

        [Name("Company Code")]
        [Column("COMPANY_CODE")]
        [StringLength(4, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string CompanyCode { get; set; }

        [Name("Created by")]
        [Column("CREATED_BY")]
        [StringLength(8, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string CreatedBy { get; set; }
    }
}
