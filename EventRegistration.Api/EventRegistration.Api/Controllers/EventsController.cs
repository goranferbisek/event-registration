using EventRegistration.Api.Data;
using EventRegistration.Api.Dtos;
using EventRegistration.Api.Models;
using EventRegistration.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventRegistration.Api.Controllers;

[ApiController]
[Route("events")]
public class EventsController(AppDbContext db, IRegistrationService registrations) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Event>>> GetEvents(
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to)
    {
        if (from > to)
            return BadRequest("'from' must be earlier than or equal to 'to'.");

        var query = db.Events.AsNoTracking().AsQueryable();
        if (from is not null) query = query.Where(e => e.StartDateTime >= from);
        if (to is not null) query = query.Where(e => e.StartDateTime <= to);

        return await query.OrderBy(e => e.StartDateTime).ToListAsync();
    }

    [HttpPost("{id:int}/registrations")]
    public async Task<IActionResult> Register(int id, [FromBody] RegisterRequest request)
    {
        var result = await registrations.RegisterAsync(id, request.FullName, request.Email);
        if (result.Succeeded)
            return Created($"/events/{id}/registrations/{result.Registration!.Id}", new
            {
                result.Registration.Id,
                result.Registration.EventId,
                result.Registration.FullName,
                result.Registration.Email,
                result.Registration.RegisteredAt
            });

        var problem = new ProblemDetails { Detail = result.Message };
        switch (result.Error)
        {
            case RegistrationError.EventNotFound:
                problem.Status = StatusCodes.Status404NotFound; problem.Title = "Event not found"; break;
            case RegistrationError.EventInPast:
                problem.Status = StatusCodes.Status400BadRequest; problem.Title = "Event already started"; break;
            default:
                problem.Status = StatusCodes.Status409Conflict;
                problem.Title = result.Error == RegistrationError.EventFull ? "Event is full" : "Already registered";
                break;
        }
        return StatusCode(problem.Status.Value, problem);
    }
}
