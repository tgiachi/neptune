namespace Neptune.Database.Core.Interfaces.Entities;

public interface IDbEntity
{
    Guid Id { get; set; }

    DateTime CreatedAt { get; set; }

    DateTime UpdatedAt { get; set; }
}
