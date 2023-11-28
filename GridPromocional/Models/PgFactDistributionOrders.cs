﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GridPromocional.Models
{
    [Table("PG_fact_distribution_orders")]
    public partial class PgFactDistributionOrders
    {
        [Key]
        [Column("DATE", TypeName = "datetime")]
        public DateTime Date { get; set; }
        [Key]
        [Column("USERNAME")]
        [StringLength(100)]
        [Unicode(false)]
        public string Username { get; set; }
        [Key]
        [Column("SKU")]
        [StringLength(20)]
        [Unicode(false)]
        public string Sku { get; set; }
        [Required]
        [Column("ID_TYPE")]
        [StringLength(1)]
        [Unicode(false)]
        public string IdType { get; set; }
        [Required]
        [Column("ID_FAM")]
        [StringLength(3)]
        [Unicode(false)]
        public string IdFam { get; set; }
        [Required]
        [Column("SALES_FORCE")]
        [StringLength(10)]
        [Unicode(false)]
        public string SalesForce { get; set; }
        [Column("ID_ST")]
        public int IdSt { get; set; }
        [Column("REQUESTED")]
        public int? Requested { get; set; }
        [Column("ORDNUM")]
        [StringLength(13)]
        [Unicode(false)]
        public string Ordnum { get; set; }

        [ForeignKey("IdFam")]
        [InverseProperty("PgFactDistributionOrders")]
        public virtual PgCatFamily IdFamNavigation { get; set; }
        [ForeignKey("IdSt")]
        [InverseProperty("PgFactDistributionOrders")]
        public virtual PgCatStatusDo IdStNavigation { get; set; }
        [ForeignKey("IdType")]
        [InverseProperty("PgFactDistributionOrders")]
        public virtual PgCatMaterialType IdTypeNavigation { get; set; }
    }
}