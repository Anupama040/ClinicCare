using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.ViewModels;

public class PrescriptionItemViewModel
{
    [Required]
    [StringLength(150)]
    [Display(Name = "Medicine name")]
    public string MedicineName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Dosage { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Frequency { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Duration { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Instructions { get; set; }
}