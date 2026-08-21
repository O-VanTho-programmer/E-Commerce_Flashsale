using System;

namespace ECommerce.Domain.Entities;

public class AuditLog : BaseEntity
{
    public int UserId { get; private set; }
    public string EntityName { get; private set; }
    public string EntityId { get; private set; }
    public string Action { get; private set; }
    public string OldValues { get; private set; }
    public string NewValues { get; private set; }
    public DateTime Timestamp { get; private set; }

    public User? User { get; private set; }

    private AuditLog()
    {
        EntityName = string.Empty;
        EntityId = string.Empty;
        Action = string.Empty;
        OldValues = string.Empty;
        NewValues = string.Empty;
    }

    public AuditLog(int userId, string entityName, string entityId, string action, string oldValues, string newValues)
    {
        UserId = userId;
        EntityName = entityName;
        EntityId = entityId;
        Action = action;
        OldValues = oldValues;
        NewValues = newValues;
        Timestamp = DateTime.UtcNow;
    }
}
