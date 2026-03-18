using AutoMapper;
using Moq;

namespace Teachio.XUnitTests.Verifications;

/// <summary>
/// Represents the <see cref="MapperVerifications"/> type.
/// </summary>
public static class MapperVerifications
{
    /// <summary>
    /// Verifies that mapping from <typeparamref name="TSource"/> to <typeparamref name="TDestination"/> was called exactly once.
    /// </summary>
    /// <typeparam name="TSource">The source object type.</typeparam>
    /// <typeparam name="TDestination">The destination object type.</typeparam>
    /// <param name="mockMapper">The mapper mock under verification.</param>
    /// <param name="source">The source object expected in the map call.</param>
    /// <param name="destination">The destination object used by the verification helper.</param>
    public static void VerifyMapWasCalled<TSource, TDestination>(
        Mock<IMapper> mockMapper,
        TSource? source,
        TDestination? destination = null)
        where TSource : class
        where TDestination : class
    {
        mockMapper.Verify(mapper => mapper.Map<TDestination>(source), Times.Once);
    }

    /// <summary>
    /// Verifies that collection mapping from <typeparamref name="TSource"/> to <typeparamref name="TDestination"/> was called exactly once.
    /// </summary>
    /// <typeparam name="TSource">The source item type.</typeparam>
    /// <typeparam name="TDestination">The destination item type.</typeparam>
    /// <param name="mockMapper">The mapper mock under verification.</param>
    public static void VerifyMapCollectionWasCalled<TSource, TDestination>(
        Mock<IMapper> mockMapper)
        where TSource : class
        where TDestination : class
    {
        mockMapper.Verify(
            m => m.Map<IEnumerable<TDestination>>(It.IsAny<IEnumerable<TSource>>()),
            Times.Once);
    }

    /// <summary>
    /// Verifies that mapping from <typeparamref name="TSource"/> into an existing <typeparamref name="TDestination"/> instance was called exactly once.
    /// </summary>
    /// <typeparam name="TSource">The source object type.</typeparam>
    /// <typeparam name="TDestination">The destination object type.</typeparam>
    /// <param name="mockMapper">The mapper mock under verification.</param>
    /// <param name="source">The source object expected in the map call.</param>
    /// <param name="destination">The destination object expected in the map call.</param>
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
