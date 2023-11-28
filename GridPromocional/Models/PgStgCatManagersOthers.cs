#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using IndexAttribute = CsvHelper.Configuration.Attributes.IndexAttribute;

namespace GridPromocional.Models
{
    [DisplayName("Gerentes y Adicionales")]
    [Table("PG_stg_cat_managers_others")]
    public partial class PgStgCatManagersOthers : UploadError
    {
        [Key]
        [Ignore]
        [Column("ID_GA")]
        public int IdGa { get; set; }

        [Index(10)]
        [Name("CDGEMP")]
        [Column("CDGEMP")]
        [StringLength(256)]
        [Unicode(false)]
        public string Cdgemp { get; set; }

        [Index(11)]
        [Name("NMBEMP")]
        [Column("NMBEMP")]
        [StringLength(256)]
        [Unicode(false)]
        public string Nmbemp { get; set; }

        [Index(12)]
        [Name("CLLEMP")]
        [Column("CLLEMP")]
        [StringLength(256)]
        [Unicode(false)]
        public string Cllemp { get; set; }

        [Index(13)]
        [Name("COLEMP")]
        [Column("COLEMP")]
        [StringLength(256)]
        [Unicode(false)]
        public string Colemp { get; set; }

        [Index(14)]
        [Name("PBLEMP")]
        [Column("PBLEMP")]
        [StringLength(256)]
        [Unicode(false)]
        public string Pblemp { get; set; }

        [Index(15)]
        [Name("CDPEMP")]
        [Column("CDPEMP")]
        [StringLength(256)]
        [Unicode(false)]
        public string Cdpemp { get; set; }

        [Index(16)]
        [Name("ESTEMP")]
        [Column("ESTEMP")]
        [StringLength(256)]
        [Unicode(false)]
        public string Estemp { get; set; }

        [Index(17)]
        [Name("TELEFONO")]
        [Column("TELEFONO")]
        [StringLength(256)]
        [Unicode(false)]
        public string Telefono { get; set; }

        [Index(18)]
        [Name("CDGCMP")]
        [Column("CDGCMP")]
        [StringLength(256)]
        [Unicode(false)]
        public string Cdgcmp { get; set; }

        [Index(19)]
        [Name("DIAING")]
        [Column("DIAING")]
        [StringLength(256)]
        [Unicode(false)]
        public string Diaing { get; set; }

        [Index(20)]
        [Name("RDTF")]
        [Column("RDTF")]
        [StringLength(256)]
        [Unicode(false)]
        public string Rdtf { get; set; }

        [Index(21)]
        [Name("ESTADO")]
        [Column("ESTADO")]
        [StringLength(256)]
        [Unicode(false)]
        public string Estado { get; set; }

        [Index(22)]
        [Name("DLGEMP")]
        [Column("DLGEMP")]
        [StringLength(256)]
        [Unicode(false)]
        public string Dlgemp { get; set; }
    }
}
