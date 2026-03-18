using AutoMapper;
using Moq;

namespace Teachio.XUnitTests.Mocks;

/// <summary>
/// Represents the <see cref="MapperMocks"/> type.
/// </summary>
public static class MapperMocks
{
    /// <summary>
    /// Configures mapper behavior for mapping a single source object to a destination object.
    /// </summary>
    /// <typeparam name="TSource">The source object type.</typeparam>
    /// <typeparam name="TDestination">The destination object type.</typeparam>
    /// <param name="mockMapper">The mapper mock to configure.</param>
    /// <param name="source">The source object expected in the map call.</param>
    /// <param name="destination">The destination object to return.</param>
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

    /// <summary>
    /// Configures mapper behavior for mapping a source collection to a destination collection.
    /// </summary>
    /// <typeparam name="TSource">The source item type.</typeparam>
    /// <typeparam name="TDestination">The destination item type.</typeparam>
    /// <param name="mockMapper">The mapper mock to configure.</param>
    /// <param name="mappedCollection">The destination collection to return.</param>
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

    /// <summary>
    /// Configures mapper behavior for mapping a source object into an existing destination object.
    /// </summary>
    /// <typeparam name="TSource">The source object type.</typeparam>
    /// <typeparam name="TDestination">The destination object type.</typeparam>
    /// <param name="mockMapper">The mapper mock to configure.</param>
    /// <param name="source">The source object expected in the map call.</param>
    /// <param name="destination">The existing destination object to return.</param>
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
