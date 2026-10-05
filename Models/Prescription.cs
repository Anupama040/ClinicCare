using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models;

public class Prescription
{
    public int Id { get; set; }

    [Required]
    public int AppointmentId { get; set; }

    [Required]
    public int PatientId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    [StringLength(1000)]
    public string? Diagnosis { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Appointment Appointment { get; set; } = null!;

    public Patient Patient { get; set; } = null!;

    public Doctor Doctor { get; set; } = null!;

    public ICollection<PrescriptionItem> Items { get; set; }
        = new List<PrescriptionItem>();
}