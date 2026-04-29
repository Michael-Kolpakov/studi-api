namespace Teachio.DAL.Utils.Constants;

public static class DatabaseConstants
{
    #region DatabaseNames

    public const string IntegrationTestsInMemoryDatabase = "teachio-integrationtests-db";

    #endregion

    #region Schemas

    public const string CoursesSchema = "courses";

    public const string UsersSchema = "users";

    #endregion

    #region TableNames

    public const string UserCourseTableName = "UserCourse";

    #endregion

    #region SqlDefaults

    public const string UtcNowSql = "GETUTCDATE()";

    #endregion
}
