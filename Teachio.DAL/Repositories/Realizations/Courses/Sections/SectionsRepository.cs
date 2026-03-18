using Teachio.DAL.Entities.Courses.Sections;
using Teachio.DAL.Persistence;
using Teachio.DAL.Repositories.Interfaces.Courses.Sections;
using Teachio.DAL.Repositories.Realizations.Base;

namespace Teachio.DAL.Repositories.Realizations.Courses.Sections;

/// <summary>
/// Represents the <see cref="SectionsRepository"/> type.
/// </summary>
public class SectionsRepository(TeachioDbContext dbContext)
    : RepositoryBase<Section>(dbContext), ISectionsRepository;
