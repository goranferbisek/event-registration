namespace EventRegistration.Api.Models;

public class Event
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset StartDateTime { get; set; }
    public required string Location { get; set; }
    public int Capacity { get; set; }

    public ICollection<Registration> Registrations { get; set; } = [];
}
