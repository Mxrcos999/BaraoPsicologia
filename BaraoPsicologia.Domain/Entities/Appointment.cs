namespace BaraoPsicologia.Domain.Entities
{
    public sealed class Appointment : EntityBase
    {
        public Patient Patient { get; set; }
        public long PatientId { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public Room Room { get; set; }
        public long RoomId { get; set; }
        public int Status { get; set; }
    }
}
