using AutoMapper;
using Moq;

namespace Teachio.XUnitTests.Mocks;

public static class MapperMocks
{
    public static void MockMap<TSource, TDestination>(
        Mock<IMapper> mockMapper,
        TSource source,
        TDestination destination)
        where TSource : class
        where TDestination : class
    {
        mockMapper
            .Setup(mapper => mapper.Map<TDestination>(source))
            .Returns(destination);
    }

    public static void MockMapCollection<TSource, TDestination>(
        Mock<IMapper> mockMapper,
        IEnumerable<TDestination> mappedCollection)
        where TSource : class
        where TDestination : class
    {
        mockMapper
            .Setup(m => m.Map<IEnumerable<TDestination>>(It.IsAny<IEnumerable<TSource>>()))
            .Returns(mappedCollection);
    }

    public static void MockMapToExisting<TSource, TDestination>(
        Mock<IMapper> mockMapper,
        TSource source,
        TDestination destination)
        where TSource : class
        where TDestination : class
    {
        mockMapper
            .Setup(mapper => mapper.Map(source, destination))
            .Returns(destination);
    }
}
