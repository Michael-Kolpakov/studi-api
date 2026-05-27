namespace Studi.DAL.Utils.Constants;

public static class DatabaseConstants
{
    #region DatabaseNames

    public const string IntegrationTestsInMemoryDatabase = "studi-integrationtests-db";

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

    #region Collations

    public const string CaseInsensitiveCollation = "SQL_Latin1_General_CP1_CI_AS";

    #endregion
}
