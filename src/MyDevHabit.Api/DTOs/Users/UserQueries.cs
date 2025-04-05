using System.Linq.Expressions;
using MyDevHabit.Api.Entities;

namespace MyDevHabit.Api.DTOs.Users;

public static class UserQueries
{
    public static Expression<Func<User, UserDto>> ProjectToDto()
    {
        return user => new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            CreatedAtUtc = user.CreatedAtUtc,
            UpdatedAtUtc = user.UpdatedAtUtc,
        };
    }
}
