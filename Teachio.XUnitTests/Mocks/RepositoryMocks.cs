using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Teachio.DAL.Repositories.Interfaces.Base;
using Teachio.DAL.Utils.Helpers;

namespace Teachio.XUnitTests.Mocks;

/// <summary>
/// Represents the <see cref="RepositoryMocks"/> type.
/// </summary>
public static class RepositoryMocks
{
    /// <summary>
    /// Configures a repository mock to return a single entity or <see langword="null"/> for <c>GetSingleOrDefaultAsync</c>.
    /// </summary>
    /// <typeparam name="TEntity">The repository entity type.</typeparam>
    /// <param name="mockRepositoryWrapper">The repository wrapper mock to configure.</param>
    /// <param name="repositorySelector">The selector that resolves the target repository from the wrapper.</param>
    /// <param name="entity">The entity instance to return.</param>
    public static void SetupGetSingleOrDefaultAsyncMock<TEntity>(
        Mock<IRepositoryWrapper> mockRepositoryWrapper,
        Expression<Func<IRepositoryWrapper, IRepositoryBase<TEntity>>> repositorySelector,
        TEntity? entity)
        where TEntity : class
    {
        var repository = GetRepository(mockRepositoryWrapper, repositorySelector);

        Mock.Get(repository)
            .Setup(repo => repo.GetSingleOrDefaultAsync(
                It.IsAny<Expression<Func<TEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
    }

    /// <summary>
    /// Configures a repository mock to return the first entity or <see langword="null"/> for <c>GetFirstOrDefaultAsync</c>.
    /// </summary>
    /// <typeparam name="TEntity">The repository entity type.</typeparam>
    /// <param name="mockRepositoryWrapper">The repository wrapper mock to configure.</param>
    /// <param name="repositorySelector">The selector that resolves the target repository from the wrapper.</param>
    /// <param name="entity">The entity instance to return.</param>
    public static void SetupGetFirstOrDefaultAsyncMock<TEntity>(
        Mock<IRepositoryWrapper> mockRepositoryWrapper,
        Expression<Func<IRepositoryWrapper, IRepositoryBase<TEntity>>> repositorySelector,
        TEntity? entity)
        where TEntity : class
    {
        var repository = GetRepository(mockRepositoryWrapper, repositorySelector);

        Mock.Get(repository)
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
    }

    /// <summary>
    /// Configures a repository mock to return the created entity for <c>CreateAsync</c>.
    /// </summary>
    /// <typeparam name="TEntity">The repository entity type.</typeparam>
    /// <param name="mockRepositoryWrapper">The repository wrapper mock to configure.</param>
    /// <param name="repositorySelector">The selector that resolves the target repository from the wrapper.</param>
    /// <param name="entity">The entity instance to return from the create call.</param>
    public static void SetupCreateAsyncMock<TEntity>(
        Mock<IRepositoryWrapper> mockRepositoryWrapper,
        Expression<Func<IRepositoryWrapper, IRepositoryBase<TEntity>>> repositorySelector,
        TEntity entity)
        where TEntity : class
    {
        var repository = GetRepository(mockRepositoryWrapper, repositorySelector);

        Mock.Get(repository)
            .Setup(repo => repo.CreateAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
    }

    /// <summary>
    /// Configures a repository mock to return the specified collection for <c>GetAllAsync</c>.
    /// </summary>
    /// <typeparam name="TEntity">The repository entity type.</typeparam>
    /// <param name="mockRepositoryWrapper">The repository wrapper mock to configure.</param>
    /// <param name="repositorySelector">The selector that resolves the target repository from the wrapper.</param>
    /// <param name="entities">The collection to return.</param>
    public static void SetupGetAllAsyncMock<TEntity>(
        Mock<IRepositoryWrapper> mockRepositoryWrapper,
        Expression<Func<IRepositoryWrapper, IRepositoryBase<TEntity>>> repositorySelector,
        IEnumerable<TEntity> entities)
        where TEntity : class
    {
        var repository = GetRepository(mockRepositoryWrapper, repositorySelector);

        Mock.Get(repository)
            .Setup(repo => repo.GetAllAsync(
                It.IsAny<Expression<Func<TEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
    }

    /// <summary>
    /// Configures a repository mock to return the specified paginated response for <c>GetAllPaginatedAsync</c>.
    /// </summary>
    /// <typeparam name="TEntity">The repository entity type.</typeparam>
    /// <param name="mockRepositoryWrapper">The repository wrapper mock to configure.</param>
    /// <param name="repositorySelector">The selector that resolves the target repository from the wrapper.</param>
    /// <param name="entities">The paginated response to return.</param>
    public static void SetupGetAllPaginatedAsyncMock<TEntity>(
        Mock<IRepositoryWrapper> mockRepositoryWrapper,
        Expression<Func<IRepositoryWrapper, IRepositoryBase<TEntity>>> repositorySelector,
        PaginationResponse<TEntity> entities)
        where TEntity : class
    {
        var repository = GetRepository(mockRepositoryWrapper, repositorySelector);

        Mock.Get(repository)
            .Setup(repo => repo.GetAllPaginatedAsync(
                It.IsAny<ushort>(),
                It.IsAny<ushort>(),
                It.IsAny<Expression<Func<TEntity, TEntity>>>(),
                It.IsAny<Expression<Func<TEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>(),
                It.IsAny<Expression<Func<TEntity, object>>>(),
                It.IsAny<Expression<Func<TEntity, object>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
    }

    /// <summary>
    /// Configures a repository mock to return the specified entity for <c>Update</c>.
    /// </summary>
    /// <typeparam name="TEntity">The repository entity type.</typeparam>
    /// <param name="mockRepositoryWrapper">The repository wrapper mock to configure.</param>
    /// <param name="repositorySelector">The selector that resolves the target repository from the wrapper.</param>
    /// <param name="entity">The entity to return from the update call.</param>
    public static void SetupUpdateMock<TEntity>(
        Mock<IRepositoryWrapper> mockRepositoryWrapper,
        Expression<Func<IRepositoryWrapper, IRepositoryBase<TEntity>>> repositorySelector,
        TEntity entity)
        where TEntity : class
    {
        var repository = GetRepository(mockRepositoryWrapper, repositorySelector);

        Mock.Get(repository)
            .Setup(repo => repo.Update(It.IsAny<TEntity>()))
            .Returns(entity);
    }

    /// <summary>
    /// Configures a repository mock to return the specified entity for <c>Delete</c>.
    /// </summary>
    /// <typeparam name="TEntity">The repository entity type.</typeparam>
    /// <param name="mockRepositoryWrapper">The repository wrapper mock to configure.</param>
    /// <param name="repositorySelector">The selector that resolves the target repository from the wrapper.</param>
    /// <param name="entity">The entity to return from the delete call.</param>
    public static void SetupDeleteMock<TEntity>(
        Mock<IRepositoryWrapper> mockRepositoryWrapper,
        Expression<Func<IRepositoryWrapper, IRepositoryBase<TEntity>>> repositorySelector,
        TEntity entity)
        where TEntity : class
    {
        var repository = GetRepository(mockRepositoryWrapper, repositorySelector);

        Mock.Get(repository)
            .Setup(repo => repo.Delete(It.IsAny<TEntity>()))
            .Returns(entity);
    }

    private static IRepositoryBase<TEntity> GetRepository<TEntity>(
        Mock<IRepositoryWrapper> mockRepositoryWrapper,
        Expression<Func<IRepositoryWrapper, IRepositoryBase<TEntity>>> repositorySelector)
        where TEntity : class
    {
        var func = repositorySelector.Compile();

        return func(mockRepositoryWrapper.Object);
    }
}
