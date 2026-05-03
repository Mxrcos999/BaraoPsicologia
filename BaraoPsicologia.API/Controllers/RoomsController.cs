using BaraoPsicologia.Application.Dto.Psychology;
using BaraoPsicologia.Application.Dto.Shared;
using BaraoPsicologia.Domain.Entities;
using BaraoPsicologia.Infra.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaraoPsicologia.API.Controllers;

[ApiController]
[Route("rooms")]
public sealed class RoomsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public RoomsController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<PagedResult<RoomResponse>>> GetList
        ([FromQuery] BaseGetRequest request, [FromQuery] long? clinicId, CancellationToken ct)
    {
        var (page, pageSize) = ListQueryHelpers.ReadPaging(Request.Query);
        var q = _db.Rooms.AsNoTracking();
        if (clinicId.HasValue)
            q = q.Where(r => r.ClinicId == clinicId.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchInput))
        {
            var s = request.SearchInput.Trim();
            q = q.Where(r => r.Number.Contains(s));
        }

        q = q.ApplyCreatedAtFilters(Request.Query);
        var total = await q.CountAsync(ct);
        var data = await q.OrderByDescending(r => r.Id)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(r => new RoomResponse
            {
                Id = r.Id,
                Number = r.Number,
                ClinicId = r.ClinicId,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .ToListAsync(ct);

        return Ok(new PagedResult<RoomResponse> { Data = data, TotalRecords = total });
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<RoomResponse>> GetById(long id, CancellationToken ct)
    {
        var r = await _db.Rooms.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (r == null)
            return NotFound(new { errors = new Dictionary<string, string[]> { ["id"] = new[] { "Sala não encontrada." } } });

        return Ok(ToResponse(r));
    }

    [HttpPost]
    public async Task<ActionResult<RoomResponse>> Create([FromBody] CreateRoomRequest body, CancellationToken ct)
    {
        var entity = new Room
        {
            Number = body.Number,
            ClinicId = body.ClinicId,
            CreatedAt = DateTime.UtcNow
        };
        _db.Rooms.Add(entity);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToResponse(entity));
    }

    [HttpPatch("{id:long}")]
    public async Task<ActionResult<RoomResponse>> Patch(long id, [FromBody] PatchRoomRequest body, CancellationToken ct)
    {
        var entity = await _db.Rooms.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null)
            return NotFound(new { errors = new Dictionary<string, string[]> { ["id"] = new[] { "Sala não encontrada." } } });

        if (body.Number != null) entity.Number = body.Number;
        if (body.ClinicId.HasValue) entity.ClinicId = body.ClinicId.Value;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Ok(ToResponse(entity));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var entity = await _db.Rooms.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null)
            return NotFound();

        _db.Rooms.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static RoomResponse ToResponse(Room r) => new()
    {
        Id = r.Id,
        Number = r.Number,
        ClinicId = r.ClinicId,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };
}
