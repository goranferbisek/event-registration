namespace EventRegistration.Api.Dtos;

public record EventResponse(
    int Id,
    string Name,
    DateTimeOffset StartDateTime,
    string Location,
    int Capacity,
    int RegistrationCount);
