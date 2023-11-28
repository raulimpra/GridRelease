#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using GridPromocional.Resources;

namespace GridPromocional.Models
{
    [Table("PG_cat_products")]
    [DisplayName("Productos")]
    [Microsoft.EntityFrameworkCore.Index(nameof(Project), IsUnique = true)]
    public partial class PgCatProducts
    {
        public PgCatProducts()
        {
            Code = "MX01688300XXX";
            PgFactPurchaseOrder = new HashSet<PgFactPurchaseOrder>();
        }

        [Key]
        [Name("Código")]
        [Column("CODE")]
        [DisplayName("Código")]
        [StringLength(20, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Code { get; set; }

        [Name("Descripción")]
        [Column("DESCRIPTION")]
        [DisplayName("Descripción")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(100, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Description { get; set; }

        [Name("Fam.")]
        [Column("ID_FAM")]
        [DisplayName("Familia")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(3, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string IdFam { get; set; }

        [Name("Tipo de Material")]
        [Column("ID_TYPE")]
        [DisplayName("Tipo de Material")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(1, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string IdType { get; set; }

        [Name("EstadoProducto")]
        [Column("ID_ST")]
        [DisplayName("EstadoProducto")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(1, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string IdSt { get; set; }

        [Name("Proyecto")]
        [Column("PROJECT")]
        [DisplayName("Proyecto")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(50, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Project { get; set; }

        [Name("Estado")]
        [Column("STATUS")]
        [DisplayName("Estado")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        public bool Status { get; set; }

        [ForeignKey("IdFam")]
        [InverseProperty("PgCatProducts")]
        public virtual PgCatFamily IdFamNavigation { get; set; }
        [ForeignKey("IdSt")]
        [InverseProperty("PgCatProducts")]
        public virtual PgCatStatusProducts IdStNavigation { get; set; }
        [ForeignKey("IdType")]
        [InverseProperty("PgCatProducts")]
        public virtual PgCatMaterialType IdTypeNavigation { get; set; }
        [InverseProperty("CodeNavigation")]
        public virtual PgFactInventories PgFactInventories { get; set; }
        [InverseProperty("CodeNavigation")]
        public virtual ICollection<PgFactPurchaseOrder> PgFactPurchaseOrder { get; set; }
    }
}
