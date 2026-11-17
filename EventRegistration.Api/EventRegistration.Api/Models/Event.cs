using System.ComponentModel.DataAnnotations;

namespace EventRegistration.Api.Models;

public class Event
{
    public int Id { get; set; }
    [MaxLength(200)]
    public required string Name { get; set; }
    public DateTimeOffset StartDateTime { get; set; }
    [MaxLength(500)]
    public required string Location { get; set; }
    public int Capacity { get; set; }

    public ICollection<Registration> Registrations { get; set; } = [];
}
