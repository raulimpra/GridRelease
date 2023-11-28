#nullable disable
using System.ComponentModel.DataAnnotations;

namespace GridPromocional.Models
{
    public class ColumnValidation
    {
        public ValidationContext Context { get; set; }
        public List<ValidationAttribute> Attributes { get; set; }
        public List<ValidationResult> Results { get; set; }
        public bool IsUpperCase { get; set; }
        public int TruncateToStringLenght { get; set; }
    }
}
