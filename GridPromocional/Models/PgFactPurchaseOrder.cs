#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using GridPromocional.Resources;

namespace GridPromocional.Models
{
    [Table("PG_fact_purchase_order")]
    [DisplayName("Ordenes de compra")]
    public partial class PgFactPurchaseOrder
    {
        [Key]
        [Name("Purchasing document")]
        [Column("PURCHASING_DOCUMENT")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(15, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string PurchasingDocument { get; set; }

        [Key]
        [Column("CODE")]
        [Name("PO Item Description (Código)")]
        [StringLength(20, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [RegularExpression("^[ -~¡-ÿ]*$", ErrorMessageResourceName = nameof(Messages.ErrorReAsciiUpercase), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Code { get; set; }

        [Name("PO Item Description")]
        [Column("PO_ITEM_DESCRIPTION")]
        [StringLength(40, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [RegularExpression("^[ -~¡-ÿ]*$", ErrorMessageResourceName = nameof(Messages.ErrorReAsciiUpercase), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string PoItemDescription { get; set; }

        [Name("Creation Date")]
        [Column("CREATION_DATE", TypeName = "date")]
        public DateTime? CreationDate { get; set; }

        [Name("Vendor")]
        [Column("VENDOR")]
        [StringLength(9, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Vendor { get; set; }

        [Name("PO Delivery Date")]
        [Column("PO_DELIVERY_DATE", TypeName = "date")]
        public DateTime? PoDeliveryDate { get; set; }

        [Name("PO Quantity")]
        [Column("PO_QUANTITY", TypeName = "numeric(10, 2)")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [Range(1, 9999999999, ErrorMessageResourceName = nameof(Messages.ErrorRangeMaxMin), ErrorMessageResourceType = typeof(Messages))]
        public decimal? PoQuantity { get; set; }

        [ForeignKey("Code")]
        [InverseProperty("PgFactPurchaseOrder")]
        public virtual PgCatProducts CodeNavigation { get; set; }
    }
}
