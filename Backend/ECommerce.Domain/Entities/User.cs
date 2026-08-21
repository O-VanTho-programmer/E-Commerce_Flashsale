using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public Enums.UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Cart? Cart { get; private set; }
    public ICollection<Order> Orders { get; private set; } = new List<Order>();
    public ICollection<AuditLog> AuditLogs { get; private set; } = new List<AuditLog>();

    private User()
    {
        Email = string.Empty;
        PasswordHash = string.Empty;
    }

    public User(string email, string passwordHash, Enums.UserRole role)
    {
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }
}
