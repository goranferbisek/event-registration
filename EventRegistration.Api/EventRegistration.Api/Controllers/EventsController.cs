using EventRegistration.Api.Data;
using EventRegistration.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventRegistration.Api.Controllers;

[ApiController]
[Route("events")]
public class EventsController(AppDbContext db) : ControllerBase
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
}
