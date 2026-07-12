using System.ComponentModel.DataAnnotations;

namespace Flowenter.Domain.Models;

public abstract class BaseEntity
{
    public Guid? Id { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    [Required, StringLength(300)]
    public string? CreatedBy { get; set; } = Environment.UserName;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(200)]
    public string? UpdatedBy { get; set; }
    public ulong Revision { get; set; }
}
