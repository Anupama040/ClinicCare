using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models;

public class Appointment
{
    public int Id { get; set; }

    [Required]
    public int PatientId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    [Required]
    [Display(Name = "Appointment date and time")]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Scheduled";

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Patient Patient { get; set; } = null!;

    public Doctor Doctor { get; set; } = null!;
}