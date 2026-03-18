using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.DAL.Repositories.Interfaces.Courses.Courses;

/// <summary>
/// Defines the contract for <see cref="ICoursesRepository"/>.
/// </summary>
public interface ICoursesRepository : IRepositoryBase<Course>;
