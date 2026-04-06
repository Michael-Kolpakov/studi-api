using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Persistence;
using Teachio.DAL.Repositories.Interfaces.Courses.Courses;
using Teachio.DAL.Repositories.Realizations.Base;

namespace Teachio.DAL.Repositories.Realizations.Courses.Courses;

public class CoursesRepository(TeachioDbContext dbContext)
    : RepositoryBase<Course>(dbContext), ICoursesRepository;
