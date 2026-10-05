using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models;

public class PrescriptionItem
{
    public int Id { get; set; }

    [Required]
    public int PrescriptionId { get; set; }

    [Required]
    [StringLength(150)]
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

    public Prescription Prescription { get; set; } = null!;
}