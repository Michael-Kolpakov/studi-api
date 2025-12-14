using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.XUnitTests.Verifications;

public static class RepositoryVerifications
{
    public static void VarifyGetFirstOrDefaultAsyncWasCalled<TEntity>(
        Mock<IRepositoryWrapper> mockRepositoryWrapper,
        Expression<Func<IRepositoryWrapper, IRepositoryBase<TEntity>>> repositorySelector)
        where TEntity : class
    {
        var repository = GetRepository(mockRepositoryWrapper, repositorySelector);

        Mock.Get(repository).Verify(
            repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>()),
            Times.Once);
    }

    public static void VerifyGetSingleOrDefaultAsyncWasCalled<TEntity>(
        Mock<IRepositoryWrapper> mockRepositoryWrapper,
        Expression<Func<IRepositoryWrapper, IRepositoryBase<TEntity>>> repositorySelector)
        where TEntity : class
    {
        var repository = GetRepository(mockRepositoryWrapper, repositorySelector);

        Mock.Get(repository).Verify(
            repo => repo.GetSingleOrDefaultAsync(
                It.IsAny<Expression<Func<TEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>()),
            Times.Once);
    }

    public static void VerifyCreateAsyncWasCalled<TEntity>(
        Mock<IRepositoryWrapper> mockRepositoryWrapper,
        Expression<Func<IRepositoryWrapper, IRepositoryBase<TEntity>>> repositorySelector)
        where TEntity : class
    {
        var repository = GetRepository(mockRepositoryWrapper, repositorySelector);

        Mock.Get(repository).Verify(
            repo => repo.CreateAsync(It.IsAny<TEntity>()),
            Times.Once);
    }

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
                It.IsAny<Expression<Func<TEntity, object>>>()),
            Times.Once);
    }

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

    public static void VerifySaveChangesAsyncWasCalled(
        Mock<IRepositoryWrapper> mockRepositoryWrapper)
    {
        mockRepositoryWrapper.Verify(
            repo => repo.SaveChangesAsync(),
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
