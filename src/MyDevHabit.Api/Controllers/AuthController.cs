using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyDevHabit.Api.Database;
using MyDevHabit.Api.DTOs.Auth;
using MyDevHabit.Api.DTOs.Users;
using MyDevHabit.Api.Entities;
using MyDevHabit.Api.Services;
using MyDevHabit.Api.Services.Settings;

namespace MyDevHabit.Api.Controllers;


[ApiController]
[Route("auth")]
[AllowAnonymous]
public sealed class AuthController(
    UserManager<IdentityUser> userManager,
    ApplicationDbContext applicationDbContext,
    ApplicationIdentityDbContext identityDbContext,
    TokenProvider tokenProvider,
    IOptions<JwtAuthOptions> options) : ControllerBase
{
    private readonly JwtAuthOptions jwtAuthOptions = options.Value;

    [HttpPost("register")]
    public async Task<ActionResult<AccessTokensDto>> Register(RegisterUserDto registerUserDto)
    {

        await using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();

        applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());

        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        // create identity user
        var identityUser = new IdentityUser
        {
            Email = registerUserDto.Email,
            UserName = registerUserDto.Name,
            EmailConfirmed = true
        };

        IdentityResult createResult = await userManager.CreateAsync(identityUser, registerUserDto.Password);

        if (!createResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                {
                    "errors", createResult.Errors.Select(e => e.Description)
                }
            };

            return Problem(
                detail: "Unable to register user, please try again",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );

        }

        // create app user

        User user = registerUserDto.ToEntity();

        user.SetIdentityUserId(identityUser.Id);

        applicationDbContext.Users.Add(user);

        await applicationDbContext.SaveChangesAsync();

        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email);

        AccessTokensDto accessTokens = tokenProvider.Create(tokenRequest);

        RefreshToken refreshToken = new()
        {
            UserId = identityUser.Id,
            Token = accessTokens.RefreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(jwtAuthOptions.RefreshTokenExpirationDays)
        };

        identityDbContext.RefreshTokens.Add(refreshToken);

        await identityDbContext.SaveChangesAsync();

        await transaction.CommitAsync();

        return Ok(accessTokens);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AccessTokensDto>> Login(LoginUserDto loginUserDto)
    {
        IdentityUser? user = await userManager.FindByEmailAsync(loginUserDto.Email);

        if (user is null)
        {
            return Problem(
                detail: "Invalid email or password",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        bool result = await userManager.CheckPasswordAsync(user, loginUserDto.Password);

        if (!result)
        {
            return Problem(
                detail: "Invalid email or password",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        AccessTokensDto accessTokens = tokenProvider.Create(new TokenRequest(user.Id, user.Email!));

        RefreshToken refreshToken = new()
        {
            UserId = user.Id,
            Token = accessTokens.RefreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(jwtAuthOptions.RefreshTokenExpirationDays)
        };

        identityDbContext.RefreshTokens.Add(refreshToken);

        await identityDbContext.SaveChangesAsync();

        return Ok(accessTokens);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AccessTokensDto>> Refresh(
        RefreshTokenDto refreshTokenDto)
    {

        RefreshToken? refreshToken = await identityDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.RefreshToken);

        if (refreshToken is null)
        {
            return Unauthorized();
        }

        if (refreshToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            return Unauthorized();
        }

        //IList<string> roles = await userManager.GetRolesAsync(refreshToken.User);

        var tokenRequest = new TokenRequest(refreshToken.User.Id, refreshToken.User.Email!);

        AccessTokensDto accessTokens = tokenProvider.Create(tokenRequest);

        refreshToken.Token = accessTokens.RefreshToken;

        refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(jwtAuthOptions.RefreshTokenExpirationDays);

        await identityDbContext.SaveChangesAsync();

        return Ok(accessTokens);
    }
}
