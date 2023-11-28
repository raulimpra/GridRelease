using System.ComponentModel.DataAnnotations.Schema;

namespace GridPromocional.Models
{
    public class UserRecord
    {
        public string? CDGEMP { get; set; } = null!;
        public string? COLEMP { get; set; } = null!;
        public string? NMBEMP { get; set; } = null!;
        public string? Perfil { get; set; } = null!;
        [Column("Has_Families")]
        public int HasFamilies { get; set; }
    }
}
