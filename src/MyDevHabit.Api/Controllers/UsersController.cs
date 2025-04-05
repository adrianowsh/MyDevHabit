using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyDevHabit.Api.Database;
using MyDevHabit.Api.DTOs.Users;

namespace MyDevHabit.Api.Controllers;

[Authorize]
[ApiController]
[Route("users")]
public sealed class UsersController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserByIid(string id)
    {
        UserDto? user = await dbContext.Users
            .Where(u => u.Id == id)
            .Select(UserQueries.ProjectToDto())
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}
