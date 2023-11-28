#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using GridPromocional.Resources;

namespace GridPromocional.Models
{
    [Table("PG_fact_inventories")]
    [DisplayName("Inventarios")]
    public partial class PgFactInventories
    {
        [Key]
        [Name("CODIGO")]
        [Column("CODE")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(20, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Code { get; set; }

        [Name("DESCRIPCION")]
        [Column("DESCRIPTION")]
        [RegularExpression("^[ -~¡-ÿ]*$", ErrorMessageResourceName = nameof(Messages.ErrorReAsciiUpercase), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(50, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Description { get; set; }

        [Name("TIPO")]
        [Column("ID_TYPE")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(1, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string IdType { get; set; }

        [Name("NEGOCIO")]
        [Column("BUSINESS")]
        [StringLength(15, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Business { get; set; }

        [Name("FAMILIA")]
        [Column("ID_FAM")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(3, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string IdFam { get; set; }

        [Name("UOM")]
        [Column("UOM")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(2, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Uom { get; set; }

        [Name("STATUS")]
        [Column("STATUS")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(2, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Status { get; set; }

        [Name("CANTIDAD")]
        [Column("QUANTITY")]
        [Range(1, 9999999999, ErrorMessageResourceName = nameof(Messages.ErrorRangeMin), ErrorMessageResourceType = typeof(Messages))]
        public int? Quantity { get; set; }

        [Name("LOTE")]
        [Column("LOT")]
        [StringLength(25, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Lot { get; set; }

        [Name("STATUS_LOTE")]
        [Column("LOT_STATUS")]
        [StringLength(15, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string LotStatus { get; set; }

        [Name("FECHA_CADUCIDAD")]
        [Column("EXPIRATION_DATE", TypeName = "date")]
        [Required(ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        public DateTime ExpirationDate { get; set; }

        [Name("IGNORAR_CADUCIDAD")]
        [Column("IGNORE_EXPIRATION")]
        public bool IgnoreExpiration { get; set; }

        [ForeignKey("Code")]
        [InverseProperty("PgFactInventories")]
        public virtual PgCatProducts CodeNavigation { get; set; }
        [ForeignKey("IdFam")]
        [InverseProperty("PgFactInventories")]
        public virtual PgCatFamily IdFamNavigation { get; set; }
        [ForeignKey("IdType")]
        [InverseProperty("PgFactInventories")]
        public virtual PgCatMaterialType IdTypeNavigation { get; set; }
    }
}
