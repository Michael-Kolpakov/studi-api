using Studi.DAL.Entities.Courses.Sections;
using Studi.DAL.Persistence;
using Studi.DAL.Repositories.Interfaces.Courses.Sections;
using Studi.DAL.Repositories.Realizations.Base;

namespace Studi.DAL.Repositories.Realizations.Courses.Sections;

public class SectionsRepository(StudiDbContext dbContext)
    : BaseRepository<Section>(dbContext), ISectionsRepository;
