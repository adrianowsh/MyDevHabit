namespace MyDevHabit.Api.Services.Settings;

public sealed class JwtAuthOptions
{
    public const string SectionName = "Jwt";

    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string Secret { get; init; }
    public int ExpirationInMinutes { get; init; } // in minutes
    public int RefreshTokenExpirationDays { get; init; }
}
