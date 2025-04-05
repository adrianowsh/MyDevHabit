using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MyDevHabit.Api.DTOs.Auth;
using MyDevHabit.Api.Services.Settings;

namespace MyDevHabit.Api.Services;

public sealed class TokenProvider(IOptions<JwtAuthOptions> options)
{
    private readonly JwtAuthOptions _options = options.Value;

    public AccessTokensDto Create(TokenRequest tokenRequest)
    {
        return new AccessTokensDto(
            GenerateAccessToken(tokenRequest),
            GenerateRefreshToken()
        );
    }

    private string GenerateAccessToken(TokenRequest tokenRequest)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, tokenRequest.UserId),
            new(JwtRegisteredClaimNames.Email, tokenRequest.Email),
            //new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_options.ExpirationInMinutes),
            SigningCredentials = credentials,
            Issuer = _options.Issuer,
            Audience = _options.Audience
        };

        JsonWebTokenHandler handler = new();

        string accessToken = handler.CreateToken(tokenDescriptor)!;

        return accessToken;
    }

    private static string GenerateRefreshToken()
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(32);

        return Convert.ToBase64String(randomBytes);
    }

}
