using Teachio.DAL.Entities.Users.Users;

namespace Teachio.XUnitTests.TestData;

public static class AppUserTestData
{
    #region Entities

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
