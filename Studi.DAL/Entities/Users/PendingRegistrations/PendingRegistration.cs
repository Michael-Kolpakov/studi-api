namespace Studi.DAL.Entities.Users.PendingRegistrations;

public class PendingRegistration
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string NormalizedEmail { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string PinHash { get; set; } = null!;

    public DateTime PinExpiresAtUtc { get; set; }

    public DateTime PinSentAtUtc { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
