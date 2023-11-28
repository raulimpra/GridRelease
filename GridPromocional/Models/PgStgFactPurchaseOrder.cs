#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;
using GridPromocional.Resources;
using Microsoft.EntityFrameworkCore;
using IndexAttribute = CsvHelper.Configuration.Attributes.IndexAttribute;

namespace GridPromocional.Models
{
    [DisplayName("Ordenes de compra")]
    [Table("PG_stg_fact_purchase_order")]
    public partial class PgStgFactPurchaseOrder : UploadError
    {
        [Key]
        [Ignore]
        [Column("ID_PO")]
        public int IdPo { get; set; }

        [Index(10)]
        [Name("Purchasing document")]
        [Column("PURCHASING_DOCUMENT")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string PurchasingDocument { get; set; }

        [Index(11)]
        [Name("PO Item Description")]
        [Column("PO_ITEM_DESCRIPTION")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string PoItemDescription { get; set; }

        [Index(12)]
        [Name("Creation Date")]
        [Column("CREATION_DATE")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string CreationDate { get; set; }

        [Index(13)]
        [Name("Vendor")]
        [Column("VENDOR")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Vendor { get; set; }

        [Index(14)]
        [Name("PO Delivery Date")]
        [Column("PO_DELIVERY_DATE")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string PoDeliveryDate { get; set; }

        [Index(15)]
        [Name("PO Quantity")]
        [Column("PO_QUANTITY")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string PoQuantity { get; set; }
    }
}
