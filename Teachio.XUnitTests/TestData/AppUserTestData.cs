using Teachio.DAL.Entities.Users;

namespace Teachio.XUnitTests.TestData;

/// <summary>
/// Represents the <see cref="AppUserTestData"/> type.
/// </summary>
public static class AppUserTestData
{
    #region Entities

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="userId">The identifier of <paramref name="userId"/>.</param>
    /// <param name="name">The <paramref name="name"/> argument.</param>
    /// <param name="surname">The <paramref name="surname"/> argument.</param>
    /// <param name="email">The <paramref name="email"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public static AppUser GetUser(
        Guid? userId = null,
        string name = "John",
        string surname = "Doe",
        string email = "john.doe@example.com")
    {
        return new AppUser()
        {
            Id = userId ?? Guid.NewGuid(),
            Name = name,
            Surname = surname,
            Email = email,
            UserName = email
        };
    }

    #endregion
}
