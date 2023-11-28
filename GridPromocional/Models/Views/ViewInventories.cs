#nullable disable

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GridPromocional.Models.Views
{
    public class ViewInventories
    {
        [Key]
        [Column("CODE")]
        [StringLength(20)]
        [Unicode(false)]
        public string Code { get; set; }
        [Column("DESCRIPTION")]
        [StringLength(50)]
        [Unicode(false)]
        public string Description { get; set; }
        [Column("FAMILY")]
        [StringLength(50)]
        [Unicode(false)]
        public string Family { get; set; }
        [Column("MATERIAL")]
        [StringLength(50)]
        [Unicode(false)]
        public string Material { get; set; }
        [Column("PROJECT")]
        [StringLength(50)]
        [Unicode(false)]
        public string Project { get; set; }
        [Column("QUANTITY")]
        public int Quantity { get; set; }
        [Column("STATUS")]
        [StringLength(50)]
        [Unicode(false)]
        public string Status { get; set; }
        [Column("ACTIVE")]
        public bool? Active { get; set; }
    }
}
