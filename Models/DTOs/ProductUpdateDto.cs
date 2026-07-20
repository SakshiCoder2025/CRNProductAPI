using System.ComponentModel.DataAnnotations;

namespace CRNProductAPI.Models.DTOs
{
    public class ProductUpdateDto
    {
        [Required]
        [MaxLength(255)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ModifiedBy { get; set; } = string.Empty;
    }
}