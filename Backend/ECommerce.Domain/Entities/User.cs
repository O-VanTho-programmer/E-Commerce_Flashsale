using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Enums.UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }

    public Cart? Cart { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
