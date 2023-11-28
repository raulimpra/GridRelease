#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GridPromocional.Models
{
    [Table("PG_user_family")]
    public partial class PgUserFamily
    {
        [Column("ID")]
        public int Id { get; set; }

        [Key]
        [Column("ID_FAM")]
        [StringLength(3)]
        [Unicode(false)]
        public string IdFam { get; set; }

        [Key]
        [Column("COLEMP")]
        [StringLength(20)]
        [Unicode(false)]
        public string Colemp { get; set; }
    }
}
