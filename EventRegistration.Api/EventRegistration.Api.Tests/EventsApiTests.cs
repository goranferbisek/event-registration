using System.Net;
using System.Net.Http.Json;
using EventRegistration.Api.Dtos;
using EventRegistration.Api.Models;

namespace EventRegistration.Api.Tests;

[Trait("Category", "Integration")]
public class EventsApiTests
{
    private static (ApiFactory Factory, HttpClient Client) Create()
    {
        var factory = new ApiFactory();
        factory.Seed(
            new Event { Id = 1, Name = "Later", Location = "B", Capacity = 2, StartDateTime = DateTimeOffset.UtcNow.AddDays(10) },
            new Event { Id = 2, Name = "Sooner", Location = "A", Capacity = 5, StartDateTime = DateTimeOffset.UtcNow.AddDays(2) });
        return (factory, factory.CreateClient());
    }

    [Fact]
    public async Task GetEvents_ReturnsAllOrderedByStart()
    {
        var (factory, client) = Create();
        using var _ = factory;

        var response = await client.GetAsync("/events");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var events = await response.Content.ReadFromJsonAsync<List<EventResponse>>();
        Assert.Equal(["Sooner", "Later"], events!.Select(e => e.Name));
    }

    [Fact]
    public async Task GetEvent_ReturnsDetails()
    {
        var (factory, client) = Create();
        using var _ = factory;

        var response = await client.GetAsync("/events/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var ev = await response.Content.ReadFromJsonAsync<EventDetailsResponse>();
        Assert.Equal("Later", ev!.Name);
        Assert.Equal(2, ev.FreeSpots);
    }

    [Fact]
    public async Task GetEvent_Unknown_ReturnsNotFound()
    {
        var (factory, client) = Create();
        using var _ = factory;

        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/events/999")).StatusCode);
    }

    [Fact]
    public async Task Register_CreatesRegistration_AndReducesFreeSpots()
    {
        var (factory, client) = Create();
        using var _ = factory;
        var request = new RegisterRequest { FullName = "Jane Doe", Email = "Jane@Example.com" };

        var response = await client.PostAsJsonAsync("/events/1/registrations", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var details = await client.GetFromJsonAsync<EventDetailsResponse>("/events/1");
        Assert.Equal(1, details!.FreeSpots);

        var duplicate = await client.PostAsJsonAsync("/events/1/registrations", request);
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
    }
}
