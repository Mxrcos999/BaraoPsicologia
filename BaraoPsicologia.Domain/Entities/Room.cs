namespace BaraoPsicologia.Domain.Entities;

public class Room : EntityBase
{ 
    public string Number { get; set; } = string.Empty;
    public long ClinicId { get; set; }
    public Clinic? Clinic { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
