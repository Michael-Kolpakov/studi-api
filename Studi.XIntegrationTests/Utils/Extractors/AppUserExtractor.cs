using Studi.DAL.Entities.Users.Users;

namespace Studi.XIntegrationTests.Utils.Extractors;

public static class AppUserExtractor
{
    public static AppUser Extract(Guid userId, string? email = null)
    {
        var testUser = TestDataProvider.GetTestData<AppUser>();
        testUser.Id = userId;

        if (email is not null)
        {
            testUser.Email = email;
        }

        return BaseExtractor.Extract(testUser, user => user.Id == userId);
    }

    public static void Remove(AppUser user)
    {
        BaseExtractor.RemoveById<AppUser>(user.Id);
    }
}
