#nullable disable

using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GridPromocional.Models
{
    public partial class UploadError
    {
        [Optional]
        [Required]
        [Name("Usuario")]
        [Index(1)]
        [Column("USERNAME")]
        [StringLength(450)]
        public string Username { get; set; }

        [Ignore]
        [Required]
        [Column("MODEL_NAME")]
        [StringLength(50)]
        public string ModelName { get; set; }

        [Optional]
        [Required]
        [Name("Archivo")]
        [Index(2)]
        [Column("FILENAME")]
        [StringLength(256)]
        public string Filename { get; set; }

        [Optional]
        [Name("Fecha")]
        [Index(3)]
        [Column("DATE", TypeName = "datetime")]
        public DateTime? UploadDate { get; set; }

        [Optional]
        [Name("Renglón")]
        [Index(4)]
        [Column("ROW")]
        public int? Row { get; set; }

        [Optional]
        [Name("Columna")]
        [Index(5)]
        [Column("COLUMN")]
        [StringLength(256)]
        public string Column { get; set; }

        [Optional]
        [Name("Error")]
        [Index(6)]
        [Column("ERROR")]
        [StringLength(512)]
        public string Error { get; set; }

        [Ignore]
        [Column("EXCEPTION")]
        public string Exception { get; set; }
    }
}