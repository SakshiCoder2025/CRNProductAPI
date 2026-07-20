using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using CRNProductAPI.Data;
using CRNProductAPI.Interfaces;
using CRNProductAPI.Models;
using CRNProductAPI.Models.DTOs;

namespace CRNProductAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IOptions<JwtSettings> jwtSettings, ApplicationDbContext context, ILogger<TokenService> logger)
        {
            _jwtSettings = jwtSettings.Value;
            _context = context;
            _logger = logger;
        }

        public async Task<AuthResponseDto> GenerateTokensAsync(string username)
        {
            try
            {
                var accessToken = GenerateAccessToken(username);
                var refreshToken = GenerateRefreshTokenString();

                var refreshTokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    Username = username,
                    ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
                    CreatedOn = DateTime.UtcNow,
                    IsRevoked = false
                };

                await _context.RefreshTokens.AddAsync(refreshTokenEntity);
                await _context.SaveChangesAsync();

                return new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating tokens for user {Username}", username);
                throw;
            }
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var storedToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked);

                if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
                {
                    _logger.LogWarning("Invalid or expired refresh token used");
                    return null;
                }

                // Purana token revoke karo (rotation)
                storedToken.IsRevoked = true;
                await _context.SaveChangesAsync();

                // Naya token pair generate karo
                return await GenerateTokensAsync(storedToken.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while refreshing token");
                throw;
            }
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            try
            {
                var storedToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(t => t.Token == refreshToken);

                if (storedToken != null)
                {
                    storedToken.IsRevoked = true;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while revoking token");
                throw;
            }
        }

        private string GenerateAccessToken(string username)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshTokenString()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}