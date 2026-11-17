namespace EventRegistration.Api.Dtos;

public record EventDetailsResponse(
    int Id,
    string Name,
    DateTimeOffset StartDateTime,
    string Location,
    int Capacity,
    int FreeSpots);
