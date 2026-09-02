using System.Threading.Tasks;

namespace ECommerce.Application.Common.Interfaces.Services;

public interface INotificationService
{
    Task SendAsync(string recipient, string subject, string message);
}
