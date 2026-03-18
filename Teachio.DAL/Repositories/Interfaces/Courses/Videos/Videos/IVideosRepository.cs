using Teachio.DAL.Entities.Courses.Videos.Videos;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.DAL.Repositories.Interfaces.Courses.Videos.Videos;

/// <summary>
/// Defines the contract for <see cref="IVideosRepository"/>.
/// </summary>
public interface IVideosRepository : IRepositoryBase<Video>;
