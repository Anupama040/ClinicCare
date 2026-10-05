using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.ViewModels;

public class PrescriptionViewModel
{
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public string PatientName { get; set; } = string.Empty;

    public string DoctorName { get; set; } = string.Empty;

    public DateTime AppointmentDate { get; set; }

    [StringLength(1000)]
    [Display(Name = "Diagnosis")]
    public string? Diagnosis { get; set; }

    [Required]
    [MinLength(
        1,
        ErrorMessage = "Add at least one medicine.")]
    public List<PrescriptionItemViewModel> Items { get; set; }
        = new();
}