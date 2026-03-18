using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.XUnitTests.Verifications;

/// <summary>
/// Represents the <see cref="RepositoryVerifications"/> type.
/// </summary>
public static class RepositoryVerifications
{
    /// <summary>
    /// Verifies that <c>GetFirstOrDefaultAsync</c> was called exactly once for the selected repository.
    /// </summary>
    /// <typeparam name="TEntity">The repository entity type.</typeparam>
    /// <param name="mockRepositoryWrapper">The repository wrapper mock under verification.</param>
    /// <param name="repositorySelector">The selector that resolves the target repository from the wrapper.</param>
    public static void VarifyGetFirstOrDefaultAsyncWasCalled<TEntity>(
        Mock<IRepositoryWrapper> mockRepositoryWrapper,
        Expression<Func<IRepositoryWrapper, IRepositoryBase<TEntity>>> repositorySelector)
        where TEntity : class
    {
        var repository = GetRepository(mockRepositoryWrapper, repositorySelector);

        Mock.Get(repository).Verify(
            repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Verifies that <c>GetSingleOrDefaultAsync</c> was called exactly once for the selected repository.
    /// </summary>
    /// <typeparam name="TEntity">The repository entity type.</typeparam>
    /// <param name="mockRepositoryWrapper">The repository wrapper mock under verification.</param>
    /// <param name="repositorySelector">The selector that resolves the target repository from the wrapper.</param>
    public static void VerifyGetSingleOrDefaultAsyncWasCalled<TEntity>(
        Mock<IRepositoryWrapper> mockRepositoryWrapper,
        Expression<Func<IRepositoryWrapper, IRepositoryBase<TEntity>>> repositorySelector)
        where TEntity : class
    {
        var repository = GetRepository(mockRepositoryWrapper, repositorySelector);

        Mock.Get(repository).Verify(
            repo => repo.GetSingleOrDefaultAsync(
                It.IsAny<Expression<Func<TEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Verifies that <c>CreateAsync</c> was called exactly once for the selected repository.
    /// </summary>
    /// <typeparam name="TEntity">The repository entity type.</typeparam>
    /// <param name="mockRepositoryWrapper">The repository wrapper mock under verification.</param>
    /// <param name="repositorySelector">The selector that resolves the target repository from the wrapper.</param>
    public static void VerifyCreateAsyncWasCalled<TEntity>(
        Mock<IRepositoryWrapper> mockRepositoryWrapper,
        Expression<Func<IRepositoryWrapper, IRepositoryBase<TEntity>>> repositorySelector)
        where TEntity : class
    {
        var repository = GetRepository(mockRepositoryWrapper, repositorySelector);

        Mock.Get(repository).Verify(
            repo => repo.CreateAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Verifies that <c>GetAllPaginatedAsync</c> was called exactly once for the selected repository.
    /// </summary>
    /// <typeparam name="TEntity">The repository entity type.</typeparam>
    /// <param name="mockRepositoryWrapper">The repository wrapper mock under verification.</param>
    /// <param name="repositorySelector">The selector that resolves the target repository from the wrapper.</param>
    public static void VerifyGetAllPaginatedAsyncWasCalled<TEntity>(
        Mock<IRepositoryWrapper> mockRepositoryWrapper,
        Expression<Func<IRepositoryWrapper, IRepositoryBase<TEntity>>> repositorySelector)
        where TEntity : class
    {
        var repository = GetRepository(mockRepositoryWrapper, repositorySelector);

        Mock.Get(repository).Verify(
            repo => repo.GetAllPaginatedAsync(
                It.IsAny<ushort>(),
                It.IsAny<ushort>(),
                It.IsAny<Expression<Func<TEntity, TEntity>>>(),
                It.IsAny<Expression<Func<TEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>(),
                It.IsAny<Expression<Func<TEntity, object>>>(),
                It.IsAny<Expression<Func<TEntity, object>>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Verifies that <c>Delete</c> was called exactly once for the selected repository.
    /// </summary>
    /// <typeparam name="TEntity">The repository entity type.</typeparam>
    /// <param name="mockRepositoryWrapper">The repository wrapper mock under verification.</param>
    /// <param name="repositorySelector">The selector that resolves the target repository from the wrapper.</param>
    public static void VerifyDeleteWasCalled<TEntity>(
        Mock<IRepositoryWrapper> mockRepositoryWrapper,
        Expression<Func<IRepositoryWrapper, IRepositoryBase<TEntity>>> repositorySelector)
        where TEntity : class
    {
        var repository = GetRepository(mockRepositoryWrapper, repositorySelector);

        Mock.Get(repository).Verify(
            repo => repo.Delete(It.IsAny<TEntity>()),
            Times.Once);
    }

    /// <summary>
    /// Verifies that <c>Update</c> was called exactly once for the selected repository.
    /// </summary>
    /// <typeparam name="TEntity">The repository entity type.</typeparam>
    /// <param name="mockRepositoryWrapper">The repository wrapper mock under verification.</param>
    /// <param name="repositorySelector">The selector that resolves the target repository from the wrapper.</param>
    public static void VerifyUpdateWasCalled<TEntity>(
        Mock<IRepositoryWrapper> mockRepositoryWrapper,
        Expression<Func<IRepositoryWrapper, IRepositoryBase<TEntity>>> repositorySelector)
        where TEntity : class
    {
        var repository = GetRepository(mockRepositoryWrapper, repositorySelector);

        Mock.Get(repository).Verify(
            repo => repo.Update(It.IsAny<TEntity>()),
            Times.Once);
    }

    /// <summary>
    /// Verifies that <c>SaveChangesAsync</c> was called exactly once.
    /// </summary>
    /// <param name="mockRepositoryWrapper">The repository wrapper mock under verification.</param>
    public static void VerifySaveChangesAsyncWasCalled(
        Mock<IRepositoryWrapper> mockRepositoryWrapper)
    {
        mockRepositoryWrapper.Verify(
            repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
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
