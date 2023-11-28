using System.ComponentModel.DataAnnotations.Schema;

namespace GridPromocional.Models
{
    public class UserFamiliesReportRecord
    {
        public string? CDGEMP { get; set; } = null!;
        public string? COLEMP { get; set; } = null!;
        public string? NMBEMP { get; set; } = null!;
        public string? Perfil { get; set; } = null!;
        public string Familia { get; set; }
    }
}
