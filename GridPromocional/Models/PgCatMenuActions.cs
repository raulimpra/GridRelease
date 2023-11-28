#nullable disable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GridPromocional.Models
{
    [Table("PG_cat_menu_actions")]
    public partial class PgCatMenuActions
    {
        public PgCatMenuActions()
        {
            PgRoleActions = new HashSet<PgRoleActions>();
        }

        [Key]
        [Column("ID_MA")]
        public int IdMa { get; set; }
        [Required]
        [Column("LEVEL1")]
        [StringLength(50)]
        [Unicode(false)]
        public string Level1 { get; set; }
        [Required]
        [Column("LEVEL2")]
        [StringLength(50)]
        [Unicode(false)]
        public string Level2 { get; set; }
        [Required]
        [Column("LEVEL3")]
        [StringLength(50)]
        [Unicode(false)]
        public string Level3 { get; set; }
        [Column("SORT")]
        public int Sort { get; set; }
        [Column("CONTROLLER")]
        [StringLength(50)]
        [Unicode(false)]
        public string Controller { get; set; }
        [Column("ACTION")]
        [StringLength(50)]
        [Unicode(false)]
        public string Action { get; set; }
        [Column("IS_MENU")]
        public bool IsMenu { get; set; }

        [InverseProperty("IdMaNavigation")]
        public virtual ICollection<PgRoleActions> PgRoleActions { get; set; }
    }
}