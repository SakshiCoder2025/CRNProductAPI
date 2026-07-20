using System.ComponentModel.DataAnnotations;

namespace CRNProductAPI.Models.DTOs
{
    public class RefreshRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}