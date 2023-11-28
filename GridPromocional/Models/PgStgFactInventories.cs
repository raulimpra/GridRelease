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
    [DisplayName("Inventarios")]
    [Table("PG_stg_fact_inventories")]
    public partial class PgStgFactInventories : UploadError
    {
        [Key]
        [Ignore]
        [Column("ID_IP")]
        public int IdIp { get; set; }

        [Index(10)]
        [Name("CODIGO")]
        [Column("CODE")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Code { get; set; }

        [Index(11)]
        [Name("DESCRIPCION")]
        [Column("DESCRIPTION")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Description { get; set; }

        [Index(12)]
        [Name("TIPO")]
        [Column("ID_TYPE")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string IdType { get; set; }

        [Index(13)]
        [Name("NEGOCIO")]
        [Column("BUSINESS")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Business { get; set; }

        [Index(14)]
        [Name("FAMILIA")]
        [Column("ID_FAM")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string IdFam { get; set; }

        [Index(15)]
        [Name("UOM")]
        [Column("UOM")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Uom { get; set; }

        [Index(16)]
        [Name("STATUS")]
        [Column("STATUS")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Status { get; set; }

        [Index(17)]
        [Name("CANTIDAD")]
        [Column("QUANTITY")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Quantity { get; set; }

        [Index(18)]
        [Name("LOTE")]
        [Column("LOT")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Lot { get; set; }

        [Index(19)]
        [Name("STATUS_LOTE")]
        [Column("LOT_STATUS")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string LotStatus { get; set; }

        [Index(20)]
        [Name("FECHA_CADUCIDAD")]
        [Column("EXPIRATION_DATE")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string ExpirationDate { get; set; }

        [Index(21)]
        [Optional]
        [Name("IGNORAR_CADUCIDAD")]
        [Column("IGNORE_EXPIRATION")]
        [StringLength(256, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string IgnoreExpiration { get; set; }
    }
}
