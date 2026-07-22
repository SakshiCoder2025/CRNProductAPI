using CRNProductAPI.Models.DTOs;

namespace CRNProductAPI.Authentication
{
    public interface ITokenService
    {
        Task<AuthResponseDto> GenerateTokensAsync(string username);
        Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}