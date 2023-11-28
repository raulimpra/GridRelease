#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using IndexAttribute = CsvHelper.Configuration.Attributes.IndexAttribute;

namespace GridPromocional.Models
{
    [DisplayName("Productos")]
    [Table("PG_stg_cat_products")]
    public partial class PgStgCatProducts : UploadError
    {
        [Key]
        [Ignore]
        [Column("ID_AP")]
        public int IdAp { get; set; }

        [Index(10)]
        [Name("Proyecto")]
        [Column("PROJECT")]
        [StringLength(256)]
        [Unicode(false)]
        public string Project { get; set; }

        [Index(11)]
        [Name("Descripción")]
        [Column("DESCRIPTION")]
        [StringLength(256)]
        [Unicode(false)]
        public string Description { get; set; }

        [Index(12)]
        [Name("Fam.")]
        [Column("ID_FAM")]
        [StringLength(256)]
        [Unicode(false)]
        public string IdFam { get; set; }

        [Index(13)]
        [Name("Tipo de Material")]
        [Column("ID_TYPE")]
        [StringLength(256)]
        [Unicode(false)]
        public string IdType { get; set; }
    }
}
