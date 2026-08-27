using System;

namespace ECommerce.Domain.Entities;

public class ExternalOrderSyncLog : BaseEntity
{
    public string PlatformName { get; private set; }
    public string ExternalOrderId { get; private set; }
    public string Status { get; private set; }
    public string Payload { get; private set; }
    public DateTime ProcessedAt { get; private set; }

    private ExternalOrderSyncLog()
    {
        PlatformName = string.Empty;
        ExternalOrderId = string.Empty;
        Status = string.Empty;
        Payload = string.Empty;
    }

    public ExternalOrderSyncLog(string platformName, string externalOrderId, string status, string payload)
    {
        PlatformName = platformName;
        ExternalOrderId = externalOrderId;
        Status = status;
        Payload = payload;
        ProcessedAt = DateTime.UtcNow;
    }
}
