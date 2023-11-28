#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using IndexAttribute = CsvHelper.Configuration.Attributes.IndexAttribute;

namespace GridPromocional.Models
{
    [DisplayName("Representantes")]
    [Table("PG_stg_cat_sales_force")]
    public partial class PgStgCatSalesForce : UploadError
    {
        [Key]
        [Ignore]
        [Column("ID_SF")]
        public int IdSf { get; set; }

        [Index(10)]
        [Name("Customer")]
        [Column("CUSTOMER")]
        [StringLength(256)]
        [Unicode(false)]
        public string Customer { get; set; }

        [Index(11)]
        [Name("Search Term 1")]
        [Column("SEARCH_TERM1")]
        [StringLength(256)]
        [Unicode(false)]
        public string SearchTerm1 { get; set; }

        [Index(12)]
        [Name("Seerch Term 2")]
        [Column("SEARCH_TERM2")]
        [StringLength(256)]
        [Unicode(false)]
        public string SearchTerm2 { get; set; }

        [Index(13)]
        [Name("Name 1")]
        [Column("NAME1")]
        [StringLength(256)]
        [Unicode(false)]
        public string Name1 { get; set; }

        [Index(14)]
        [Name("Name 2")]
        [Column("NAME2")]
        [StringLength(256)]
        [Unicode(false)]
        public string Name2 { get; set; }

        [Index(15)]
        [Name("Sales Organization")]
        [Column("SALES_ORGANIZATION")]
        [StringLength(256)]
        [Unicode(false)]
        public string SalesOrganization { get; set; }

        [Index(16)]
        [Name("Street")]
        [Column("STREET")]
        [StringLength(256)]
        [Unicode(false)]
        public string Street { get; set; }

        [Index(17)]
        [Name("Street 2")]
        [Column("STREET2")]
        [StringLength(256)]
        [Unicode(false)]
        public string Street2 { get; set; }

        [Index(18)]
        [Name("Region")]
        [Column("REGION")]
        [StringLength(256)]
        [Unicode(false)]
        public string Region { get; set; }

        [Index(19)]
        [Name("Postal Code")]
        [Column("POSTAL_CODE")]
        [StringLength(256)]
        [Unicode(false)]
        public string PostalCode { get; set; }

        [Index(20)]
        [Name("City")]
        [Column("CITY")]
        [StringLength(256)]
        [Unicode(false)]
        public string City { get; set; }

        [Index(21)]
        [Name("Telephone")]
        [Column("TELEPHONE")]
        [StringLength(256)]
        [Unicode(false)]
        public string Telephone { get; set; }

        [Index(22)]
        [Name("Company Code")]
        [Column("COMPANY_CODE")]
        [StringLength(256)]
        [Unicode(false)]
        public string CompanyCode { get; set; }

        [Index(23)]
        [Name("Created by")]
        [Column("CREATED_BY")]
        [StringLength(256)]
        [Unicode(false)]
        public string CreatedBy { get; set; }
    }
}
