using BaraoPsicologia.Application.Dto.Psychology;
using BaraoPsicologia.Application.Dto.Shared;
using BaraoPsicologia.Domain.Entities;
using BaraoPsicologia.Infra.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaraoPsicologia.API.Controllers;

[ApiController]
[Route("clinics")]
public sealed class ClinicsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ClinicsController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PagedResult<ClinicResponse>>> GetList
        ([FromQuery] BaseGetRequest request, CancellationToken ct)
    {
        var (page, pageSize) = ListQueryHelpers.ReadPaging(Request.Query);
        var q = _db.Clinics.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.SearchInput))
        {
            var s = request.SearchInput.Trim();
            q = q.Where(c => c.Name.Contains(s) || c.Address.Contains(s));
        }

        q = q.ApplyCreatedAtFilters(Request.Query);
        var total = await q.CountAsync(ct);
        var data = await q.OrderByDescending(c => c.Id)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(c => new ClinicResponse
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync(ct);

        return Ok(new PagedResult<ClinicResponse> { Data = data, TotalRecords = total });
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ClinicResponse>> GetById(long id, CancellationToken ct)
    {
        var c = await _db.Clinics.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (c == null)
            return NotFound(new { errors = new Dictionary<string, string[]> { ["id"] = new[] { "Clínica não encontrada." } } });

        return Ok(ToResponse(c));
    }

    [HttpPost]
    public async Task<ActionResult<ClinicResponse>> Create([FromBody] CreateClinicRequest body, CancellationToken ct)
    {
        var entity = new Clinic
        {
            Name = body.Name,
            Address = body.Address,
            CreatedAt = DateTime.UtcNow
        };
        _db.Clinics.Add(entity);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToResponse(entity));
    }

    [HttpPatch("{id:long}")]
    public async Task<ActionResult<ClinicResponse>> Patch(long id, [FromBody] PatchClinicRequest body, CancellationToken ct)
    {
        var entity = await _db.Clinics.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null)
            return NotFound(new { errors = new Dictionary<string, string[]> { ["id"] = new[] { "Clínica não encontrada." } } });

        if (body.Name != null) entity.Name = body.Name;
        if (body.Address != null) entity.Address = body.Address;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Ok(ToResponse(entity));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var entity = await _db.Clinics.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null)
            return NotFound();

        _db.Clinics.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static ClinicResponse ToResponse(Clinic c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
