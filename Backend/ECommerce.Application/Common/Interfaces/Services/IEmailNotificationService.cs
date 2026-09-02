namespace ECommerce.Application.Common.Interfaces.Services;

public interface IEmailNotificationService : INotificationService
{
    // Cổng mở rộng riêng cho Email (ví dụ đính kèm file, HTML template...)
    // Task SendWithAttachmentAsync(string recipient, string subject, string message, byte[] attachment);
}
