using System.ComponentModel.DataAnnotations;

namespace CRNProductAPI.Models.DTOs
{
    public class ProductCreateDto
    {
        [Required]
        [MaxLength(255)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string CreatedBy { get; set; } = string.Empty;
    }
}