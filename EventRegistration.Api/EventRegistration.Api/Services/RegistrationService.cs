using EventRegistration.Api.Data;
using EventRegistration.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EventRegistration.Api.Services;

public enum RegistrationError { None, EventNotFound, EventInPast, EventFull, AlreadyRegistered }

public record RegistrationResult(Registration? Registration, RegistrationError Error, string? Message = null)
{
    public bool Succeeded => Error == RegistrationError.None;
}

public interface IRegistrationService
{
    Task<RegistrationResult> RegisterAsync(int eventId, string fullName, string email);
}

public class RegistrationService(AppDbContext db, TimeProvider clock) : IRegistrationService
{
    public async Task<RegistrationResult> RegisterAsync(int eventId, string fullName, string email)
    {
        var ev = await db.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        if (ev is null)
            return new(null, RegistrationError.EventNotFound, $"Event {eventId} was not found.");

        var now = clock.GetUtcNow();
        if (ev.StartDateTime <= now)
            return new(null, RegistrationError.EventInPast, "Cannot register for an event that has already started.");

        var normalizedEmail = email.Trim().ToLowerInvariant();

        if (await db.Registrations.AnyAsync(r => r.EventId == eventId && r.Email == normalizedEmail))
            return new(null, RegistrationError.AlreadyRegistered, "This email is already registered for the event.");

        if (await db.Registrations.CountAsync(r => r.EventId == eventId) >= ev.Capacity)
            return new(null, RegistrationError.EventFull, "The event is full.");

        var registration = new Registration
        {
            EventId = eventId,
            FullName = fullName.Trim(),
            Email = normalizedEmail,
            RegisteredAt = now
        };
        db.Registrations.Add(registration);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return new(null, RegistrationError.AlreadyRegistered, "This email is already registered for the event.");
        }

        return new(registration, RegistrationError.None);
    }
}
