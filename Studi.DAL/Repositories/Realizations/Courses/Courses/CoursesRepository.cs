using Studi.DAL.Entities.Courses.Courses;
using Studi.DAL.Persistence;
using Studi.DAL.Repositories.Interfaces.Courses.Courses;
using Studi.DAL.Repositories.Realizations.Base;

namespace Studi.DAL.Repositories.Realizations.Courses.Courses;

public class CoursesRepository(StudiDbContext dbContext)
    : BaseRepository<Course>(dbContext), ICoursesRepository;
