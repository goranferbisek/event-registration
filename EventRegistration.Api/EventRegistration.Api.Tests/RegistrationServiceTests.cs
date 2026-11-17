using EventRegistration.Api.Data;
using EventRegistration.Api.Models;
using EventRegistration.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace EventRegistration.Api.Tests;

public class RegistrationServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 17, 12, 0, 0, TimeSpan.Zero);

    private class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static RegistrationService CreateService(AppDbContext db) => new(db, new FixedTimeProvider(Now));

    private static async Task<Event> AddEvent(AppDbContext db, int capacity = 2, DateTimeOffset? start = null)
    {
        var ev = new Event
        {
            Name = "Test event",
            Location = "Somewhere",
            Capacity = capacity,
            StartDateTime = start ?? Now.AddDays(7)
        };
        db.Events.Add(ev);
        await db.SaveChangesAsync();
        return ev;
    }

    [Fact]
    public async Task Register_ValidRequest_Succeeds()
    {
        using var db = CreateDb();
        var ev = await AddEvent(db);

        var result = await CreateService(db).RegisterAsync(ev.Id, "Ann", "ann@example.com");

        Assert.True(result.Succeeded);
        Assert.Equal(1, await db.Registrations.CountAsync());
        Assert.Equal(Now, result.Registration!.RegisteredAt);
    }

    [Fact]
    public async Task Register_UnknownEvent_ReturnsEventNotFound()
    {
        using var db = CreateDb();

        var result = await CreateService(db).RegisterAsync(999, "Ann", "ann@example.com");

        Assert.Equal(RegistrationError.EventNotFound, result.Error);
    }

    [Fact]
    public async Task Register_EventIsFull_ReturnsEventFull()
    {
        using var db = CreateDb();
        var ev = await AddEvent(db, capacity: 1);
        var service = CreateService(db);
        await service.RegisterAsync(ev.Id, "Ann", "ann@example.com");

        var result = await service.RegisterAsync(ev.Id, "Bob", "bob@example.com");

        Assert.Equal(RegistrationError.EventFull, result.Error);
        Assert.Equal(1, await db.Registrations.CountAsync());
    }

    [Fact]
    public async Task Register_LastFreeSpot_Succeeds()
    {
        using var db = CreateDb();
        var ev = await AddEvent(db, capacity: 2);
        var service = CreateService(db);
        await service.RegisterAsync(ev.Id, "Ann", "ann@example.com");

        var result = await service.RegisterAsync(ev.Id, "Bob", "bob@example.com");

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task Register_SameEmailTwice_ReturnsAlreadyRegistered()
    {
        using var db = CreateDb();
        var ev = await AddEvent(db);
        var service = CreateService(db);
        await service.RegisterAsync(ev.Id, "Ann", "ann@example.com");

        var result = await service.RegisterAsync(ev.Id, "Ann again", "ann@example.com");

        Assert.Equal(RegistrationError.AlreadyRegistered, result.Error);
        Assert.Equal(1, await db.Registrations.CountAsync());
    }

    [Fact]
    public async Task Register_SameEmailOnDifferentEvents_Succeeds()
    {
        using var db = CreateDb();
        var ev1 = await AddEvent(db);
        var ev2 = await AddEvent(db);
        var service = CreateService(db);
        await service.RegisterAsync(ev1.Id, "Ann", "ann@example.com");

        var result = await service.RegisterAsync(ev2.Id, "Ann", "ann@example.com");

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task Register_EventInPast_ReturnsEventInPast()
    {
        using var db = CreateDb();
        var ev = await AddEvent(db, start: Now.AddDays(-1));

        var result = await CreateService(db).RegisterAsync(ev.Id, "Ann", "ann@example.com");

        Assert.Equal(RegistrationError.EventInPast, result.Error);
        Assert.Equal(0, await db.Registrations.CountAsync());
    }
}
