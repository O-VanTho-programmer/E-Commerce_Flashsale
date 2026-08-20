using System;

namespace ECommerce.Domain.Entities;

public class AuditLog : BaseEntity
{
    public int UserId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string OldValues { get; set; } = string.Empty;
    public string NewValues { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }

    public User? User { get; set; }
}
