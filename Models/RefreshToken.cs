namespace CRNProductAPI.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsRevoked { get; set; } = false;
    }
}