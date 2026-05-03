namespace BaraoPsicologia.Application.Dto.Psychology;

public sealed class ClinicResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class CreateClinicRequest
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public sealed class PatchClinicRequest
{
    public string? Name { get; set; }
    public string? Address { get; set; }
}
