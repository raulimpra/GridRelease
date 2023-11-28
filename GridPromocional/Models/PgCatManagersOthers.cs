#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using GridPromocional.Resources;

namespace GridPromocional.Models
{
    [Table("PG_cat_managers_others")]
    [DisplayName("Gerentes y Adicionales")]
    public partial class PgCatManagersOthers
    {
        [Name("CDGEMP")]
        [Column("CDGEMP")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(15, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Cdgemp { get; set; }

        [Name("NMBEMP")]
        [Column("NMBEMP")]
        [StringLength(40, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
		[RegularExpression("^[ -~¡-ÿ]*$", ErrorMessageResourceName = nameof(Messages.ErrorReAsciiUpercase), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Nmbemp { get; set; }

        [Name("CLLEMP")]
        [Column("CLLEMP")]
		[RegularExpression("^[ -~¡-ÿ]*$", ErrorMessageResourceName = nameof(Messages.ErrorReAsciiUpercase), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(40, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Cllemp { get; set; }

        [Key]
        [Name("COLEMP")]
        [Column("COLEMP")]
        [StringLength(12, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Colemp { get; set; }

        [Name("PBLEMP")]
        [Column("PBLEMP")]
		[RegularExpression("^[ -~¡-ÿ]*$", ErrorMessageResourceName = nameof(Messages.ErrorReAsciiUpercase), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(40, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Pblemp { get; set; }

        [Name("CDPEMP")]
        [Column("CDPEMP")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        // [StringLength(5, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [RegularExpression(@"^\d{5}$", ErrorMessageResourceName = nameof(Messages.ErrorZipCode), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Cdpemp { get; set; }

        [Name("ESTEMP")]
        [Column("ESTEMP")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(1, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Estemp { get; set; }

        [Name("TELEFONO")]
        [Column("TELEFONO")]
        // [StringLength(10, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [RegularExpression(@"^\d{10}$", ErrorMessageResourceName = nameof(Messages.ErrorTelephone), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Telefono { get; set; }

        [Name("CDGCMP")]
        [Column("CDGCMP")]
        [StringLength(4, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Cdgcmp { get; set; }

        [Name("DIAING")]
        [Column("DIAING")]
        [StringLength(8, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Diaing { get; set; }

        [Name("RDTF")]
        [Column("RDTF")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Messages.ErrorRequired), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(8, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Rdtf { get; set; }

        [Name("ESTADO")]
        [Column("ESTADO")]
        [StringLength(40, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
		[RegularExpression("^[ -~¡-ÿ]*$", ErrorMessageResourceName = nameof(Messages.ErrorReAsciiUpercase), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Estado { get; set; }

        [Name("DLGEMP")]
        [Column("DLGEMP")]
        [StringLength(40, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
		[RegularExpression("^[ -~¡-ÿ]*$", ErrorMessageResourceName = nameof(Messages.ErrorReAsciiUpercase), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Dlgemp { get; set; }

        [ForeignKey("Estemp")]
        [InverseProperty("PgCatManagersOthers")]
        public virtual PgCatStatusEmployee EstempNavigation { get; set; }
    }
}
