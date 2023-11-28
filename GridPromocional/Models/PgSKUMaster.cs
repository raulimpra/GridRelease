using CsvHelper.Configuration.Attributes;
using GridPromocional.Resources;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace GridPromocional.Models
{
    public class PgSKUMaster
    {
        [Key]
        [Name("Código")]
        [Column("CODE")]
        [DisplayName("Código")]
        [StringLength(20, ErrorMessageResourceName = nameof(Messages.ErrorStringLength), ErrorMessageResourceType = typeof(Messages))]
        [Unicode(false)]
        public string Code { get; set; }

        [Name("DateCreate")]
        [Column("DateCreate")]
        [DisplayName("Fecha de creacion")]
        public DateTime DateCreate { get; set; }

        [Name("DateGenerate")]
        [Column("DateGenerate")]
        [DisplayName("Fecha de generacion")]
        public DateTime? DateGenerate { get; set; }

        [Name("UserGenerate")]
        [Column("UserGenerate")]
        [DisplayName("Usuario genera")]
        public string? UserGenerate { get; set; }

        [Name("NumReport")]
        [Column("NumReport")]
        [DisplayName("Numero de reporte")]
        public int? NumReport { get; set; }
    }
}
