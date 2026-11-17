using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EventRegistration.Api.Models;

[Index(nameof(EventId), nameof(Email), IsUnique = true)]
public class Registration
{
    public int Id { get; set; }
    public int EventId { get; set; }
    [MaxLength(200)]
    public required string FullName { get; set; }
    [MaxLength(254)]
    public required string Email { get; set; }
    public DateTimeOffset RegisteredAt { get; set; }

    public Event Event { get; set; } = null!;
}
