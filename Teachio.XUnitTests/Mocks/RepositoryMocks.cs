using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Teachio.DAL.Repositories.Interfaces.Base;
using Teachio.DAL.Utils.Helpers;

namespace Teachio.XUnitTests.Mocks;

public static class RepositoryMocks
{
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
                It.IsAny<Expression<Func<TEntity, object>>>(),
                It.IsAny<Expression<Func<TEntity, object>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
    }

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
