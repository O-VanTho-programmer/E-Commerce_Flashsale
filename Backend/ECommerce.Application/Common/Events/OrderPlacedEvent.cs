namespace ECommerce.Application.Common.Events;

public record OrderPlacedEventItem(int ProductVariantId, int Quantity);
public record OrderPlacedEvent(int OrderId, int UserId, List<OrderPlacedEventItem> Items);