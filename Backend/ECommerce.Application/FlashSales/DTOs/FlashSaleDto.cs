using System;
using System.Collections.Generic;

namespace ECommerce.Application.FlashSales.DTOs;

public class FlashSaleDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string Status { get; set; }
    
    public List<FlashSaleItemDto> Items { get; set; } = new();
}

public class FlashSaleItemDto
{
    public int ProductVariantId { get; set; }
    public string ProductName { get; set; }
    public string Sku { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal SalePrice { get; set; }
    public int SaleStock { get; set; }
    public int SoldCount { get; set; }
    public int AvailableStock => SaleStock - SoldCount;
}
