using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicManagement.ViewModels;

public class AppointmentViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please select a patient.")]
    [Display(Name = "Patient")]
    public int PatientId { get; set; }

    [Required(ErrorMessage = "Please select a doctor.")]
    [Display(Name = "Doctor")]
    public int DoctorId { get; set; }

    [Required(ErrorMessage = "Please select an appointment date and time.")]
    [Display(Name = "Appointment date and time")]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [StringLength(500)]
    public string? Notes { get; set; }

    public IEnumerable<SelectListItem> Patients { get; set; }
        = new List<SelectListItem>();

    public IEnumerable<SelectListItem> Doctors { get; set; }
        = new List<SelectListItem>();
}