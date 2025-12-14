using AutoMapper;
using Moq;

namespace Teachio.XUnitTests.Verifications;

public static class MapperVerifications
{
    public static void VerifyMapWasCalled<TSource, TDestination>(
        Mock<IMapper> mockMapper,
        TSource? source,
        TDestination? destination = null)
        where TSource : class
        where TDestination : class
    {
        mockMapper.Verify(mapper => mapper.Map<TDestination>(source), Times.Once);
    }

    public static void VerifyMapCollectionWasCalled<TSource, TDestination>(
        Mock<IMapper> mockMapper)
        where TSource : class
        where TDestination : class
    {
        mockMapper.Verify(
            m => m.Map<IEnumerable<TDestination>>(It.IsAny<IEnumerable<TSource>>()),
            Times.Once);
    }

    public static void VerifyMapToExistingWasCalled<TSource, TDestination>(
        Mock<IMapper> mockMapper,
        TSource? source,
        TDestination? destination)
        where TSource : class
        where TDestination : class
    {
        mockMapper.Verify(
            mapper => mapper.Map(source, destination),
            Times.Once);
    }
}
