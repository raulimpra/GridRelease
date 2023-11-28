#nullable disable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GridPromocional.Models
{
    [Table("PG_role_actions")]
    public partial class PgRoleActions
    {
        [Key]
        [Column("ROLE_NAME")]
        [StringLength(256)]
        public string RoleName { get; set; }
        [Key]
        [Column("ID_MA")]
        public int IdMa { get; set; }

        [ForeignKey("IdMa")]
        [InverseProperty("PgRoleActions")]
        public virtual PgCatMenuActions IdMaNavigation { get; set; }
    }
}