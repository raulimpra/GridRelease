﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GridPromocional.Models
{
    [Table("PG_cat_family")]
    public partial class PgCatFamily
    {
        public PgCatFamily()
        {
            PgCatProducts = new HashSet<PgCatProducts>();
            PgFactDistributionOrders = new HashSet<PgFactDistributionOrders>();
            PgFactInventories = new HashSet<PgFactInventories>();
            PgSelFamily = new HashSet<PgSelFamily>();
        }

        [Key]
        [Column("ID_FAM")]
        [StringLength(3)]
        [Unicode(false)]
        public string IdFam { get; set; }
        [Required]
        [Column("FAMILY")]
        [StringLength(50)]
        [Unicode(false)]
        public string Family { get; set; }

        [InverseProperty("IdFamNavigation")]
        public virtual ICollection<PgCatProducts> PgCatProducts { get; set; }
        [InverseProperty("IdFamNavigation")]
        public virtual ICollection<PgFactDistributionOrders> PgFactDistributionOrders { get; set; }
        [InverseProperty("IdFamNavigation")]
        public virtual ICollection<PgFactInventories> PgFactInventories { get; set; }
        [InverseProperty("IdFamNavigation")]
        public virtual ICollection<PgSelFamily> PgSelFamily { get; set; }
    }
}