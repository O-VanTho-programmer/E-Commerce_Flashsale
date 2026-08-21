using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class FlashSale : BaseEntity
{
    public string Name { get; private set; }
    public DateTime StartAt { get; private set; }
    public DateTime EndAt { get; private set; }
    public Enums.FlashSaleStatus Status { get; private set; }

    public ICollection<FlashSaleItem> FlashSaleItems { get; private set; } = new List<FlashSaleItem>();

    private FlashSale()
    {
        Name = string.Empty;
    }

    public FlashSale(string name, DateTime startAt, DateTime endAt)
    {
        Name = name;
        StartAt = startAt;
        EndAt = endAt;
        Status = Enums.FlashSaleStatus.Scheduled;
    }

    public void UpdateStatus(Enums.FlashSaleStatus status)
    {
        Status = status;
    }
}
