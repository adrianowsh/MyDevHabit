namespace MyDevHabit.Api.Entities;

public sealed class User
{
    private User(
        string email,
        string name)
    {
        Id = $"u_{Guid.CreateVersion7()}";
        Email = email;
        Name = name;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public string Id { get; }
    public string Email { get; }
    public string Name { get; }
    public DateTime CreatedAtUtc { get; }
    public DateTime? UpdatedAtUtc { get; set; }

    // <summary>
    // We'll use this to identify the user in the identity provider
    // This could be any identity provider like Azure AD, Okta, Auth0, etc.
    // </summary>
    public string IdentityId { get; private set; } = string.Empty;

    public static User CreateUser(
        string email,
        string name
        )
    {
        User user = new(email, name);

        return user;
    }

    public void SetIdentityUserId(string identityId)
    {
        IdentityId = identityId;
    }

    public void SerUpdatedAtUtc() => UpdatedAtUtc = DateTime.UtcNow;
}
