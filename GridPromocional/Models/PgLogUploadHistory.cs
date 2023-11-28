#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GridPromocional.Models
{
    [DisplayName("Historial")]
    [Table("PG_log_upload_history")]
    public partial class PgLogUploadHistory
    {
        [Key]
        [Column("ID_UH")]
        public int IdUh { get; set; }

        [Required]
        [Column("USERNAME")]
        [DisplayName("Usuario")]
        [StringLength(100)]
        [Unicode(false)]
        public string Username { get; set; }

        [Required]
        [Column("MODEL_NAME")]
        [DisplayName("Modelo")]
        [StringLength(50)]
        [Unicode(false)]
        public string ModelName { get; set; }

        [Required]
        [Column("FILENAME")]
        [DisplayName("Archivo")]
        [StringLength(256)]
        [Unicode(false)]
        public string Filename { get; set; }

        [DisplayName("Fecha")]
        [Column("UPLOAD_DATE", TypeName = "datetime")]
        public DateTime UploadDate { get; set; }

        [DisplayName("Registros")]
        [Column("TOTAL_REGISTERS")]
        public int TotalRegisters { get; set; }

        [DisplayName("Correctos")]
        [Column("UPLOADED_REGISTERS")]
        public int UploadedRegisters { get; set; }

        [DisplayName("Fallidos")]
        [Column("WRONG_REGISTERS")]
        public int WrongRegisters { get; set; }

        [DisplayName("Errores")]
        [Column("TOTAL_ERRORS")]
        public int TotalErrors { get; set; }
    }
}
