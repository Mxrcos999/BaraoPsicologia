namespace BaraoPsicologia.Application.Dto.Psychology;

public sealed class PatientResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DateBirth { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ParentDegree { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class CreatePatientRequest
{
    public string Name { get; set; } = string.Empty;
    public DateTime DateBirth { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ParentDegree { get; set; } = string.Empty;
}

public sealed class PatchPatientRequest
{
    public string? Name { get; set; }
    public DateTime? DateBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ParentName { get; set; }
    public string? ParentDegree { get; set; }
}
