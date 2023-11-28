﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GridPromocional.Models
{
    [Table("PG_log_activity")]
    public partial class PgLogActivity
    {
        [Key]
        [Column("ID_LA")]
        public int IdLa { get; set; }
        [Column("DATE", TypeName = "datetime")]
        public DateTime Date { get; set; }
        [Required]
        [Column("USERNAME")]
        [StringLength(100)]
        [Unicode(false)]
        public string Username { get; set; }
        [Required]
        [Column("ACTIVITY")]
        [StringLength(256)]
        [Unicode(false)]
        public string Activity { get; set; }
        [Column("FILENAME")]
        [StringLength(256)]
        [Unicode(false)]
        public string Filename { get; set; }
    }
}