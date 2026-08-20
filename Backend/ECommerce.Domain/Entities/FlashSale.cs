using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class FlashSale : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public Enums.FlashSaleStatus Status { get; set; }

    public ICollection<FlashSaleItem> FlashSaleItems { get; set; } = new List<FlashSaleItem>();
}
