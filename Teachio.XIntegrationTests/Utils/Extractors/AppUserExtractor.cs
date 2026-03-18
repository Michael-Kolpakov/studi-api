using Teachio.DAL.Entities.Users;

namespace Teachio.XIntegrationTests.Utils.Extractors;

/// <summary>
/// Represents the <see cref="AppUserExtractor"/> type.
/// </summary>
public static class AppUserExtractor
{
    /// <summary>
    /// Performs the <see cref="Extract"/> operation.
    /// </summary>
    /// <param name="userId">The identifier of <paramref name="userId"/>.</param>
    /// <param name="email">The <paramref name="email"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
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

    /// <summary>
    /// Deletes the target entity.
    /// </summary>
    /// <param name="user">The <paramref name="user"/> argument.</param>
    public static void Remove(AppUser user)
    {
        BaseExtractor.RemoveById<AppUser>(user.Id);
    }
}
