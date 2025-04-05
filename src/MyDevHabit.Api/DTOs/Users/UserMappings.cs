using MyDevHabit.Api.DTOs.Auth;
using MyDevHabit.Api.Entities;

namespace MyDevHabit.Api.DTOs.Users;

public static class UserMappings
{
    public static User ToEntity(this RegisterUserDto dto)
    {
        return User.CreateUser(email: dto.Email, name: dto.Name);
    }

}
