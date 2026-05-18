using Teachio.DAL.Entities.Users.Users;

namespace Teachio.DAL.Entities.Users.AvatarFiles;

public class AvatarFile
{
    public Guid Id { get; set; }

    public string AvatarName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public string Resolution { get; set; } = null!;

    public Guid AppUserId { get; set; }

    public AppUser? AppUser { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
