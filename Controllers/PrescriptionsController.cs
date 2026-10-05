using ClinicManagement.Data;
using ClinicManagement.Models;
using ClinicManagement.Services;
using ClinicManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Controllers;

[Authorize(Roles = "Admin,Doctor")]
public class PrescriptionsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly PrescriptionPdfService _pdfService;

    public PrescriptionsController(
        ApplicationDbContext context,
        PrescriptionPdfService pdfService)
    {
        _context = context;
        _pdfService = pdfService;
    }

    [HttpGet]
    public async Task<IActionResult> Create(
        int appointmentId)
    {
        var appointment = await _context.Appointments
            .AsNoTracking()
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.Doctor)
            .FirstOrDefaultAsync(
                appointment => appointment.Id == appointmentId);

        if (appointment is null)
        {
            return NotFound();
        }

        if (appointment.Status != "Completed")
        {
            TempData["ErrorMessage"] =
                "A prescription can be created only for a completed appointment.";

            return RedirectToAction(
                "Details",
                "Appointments",
                new
                {
                    id = appointmentId
                });
        }

        var existingPrescription =
            await _context.Prescriptions
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    prescription =>
                        prescription.AppointmentId == appointmentId);

        if (existingPrescription is not null)
        {
            TempData["ErrorMessage"] =
                "A prescription already exists for this appointment.";

            return RedirectToAction(
                nameof(Details),
                new
                {
                    prescriptionId = existingPrescription.Id
                });
        }

        var model = new PrescriptionViewModel
        {
            AppointmentId = appointment.Id,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            PatientName =
                $"{appointment.Patient.FirstName} "
                + $"{appointment.Patient.LastName}",
            DoctorName =
                $"Dr. {appointment.Doctor.FirstName} "
                + $"{appointment.Doctor.LastName}",
            AppointmentDate = appointment.AppointmentDate,
            Items =
            [
                new PrescriptionItemViewModel()
            ]
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        PrescriptionViewModel model)
    {
        var appointment = await _context.Appointments
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.Doctor)
            .FirstOrDefaultAsync(
                appointment =>
                    appointment.Id == model.AppointmentId);

        if (appointment is null)
        {
            return NotFound();
        }

        if (appointment.Status != "Completed")
        {
            ModelState.AddModelError(
                string.Empty,
                "A prescription can be created only for a completed appointment.");
        }

        var existingPrescription =
            await _context.Prescriptions
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    prescription =>
                        prescription.AppointmentId
                        == model.AppointmentId);

        if (existingPrescription is not null)
        {
            return RedirectToAction(
                nameof(Details),
                new
                {
                    prescriptionId = existingPrescription.Id
                });
        }

        if (model.Items is null || model.Items.Count == 0)
        {
            ModelState.AddModelError(
                nameof(model.Items),
                "Add at least one medicine.");
        }

        if (!ModelState.IsValid)
        {
            model.PatientName =
                $"{appointment.Patient.FirstName} "
                + $"{appointment.Patient.LastName}";

            model.DoctorName =
                $"Dr. {appointment.Doctor.FirstName} "
                + $"{appointment.Doctor.LastName}";

            model.AppointmentDate =
                appointment.AppointmentDate;

            if (model.Items is null || model.Items.Count == 0)
            {
                model.Items =
                [
                    new PrescriptionItemViewModel()
                ];
            }

            return View(model);
        }

        var prescription = new Prescription
        {
            AppointmentId = appointment.Id,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            Diagnosis = model.Diagnosis,
            CreatedAt = DateTime.UtcNow,
            Items =
                (model.Items
                    ?? new List<PrescriptionItemViewModel>())
                .Select(item => new PrescriptionItem
                {
                    MedicineName = item.MedicineName,
                    Dosage = item.Dosage,
                    Frequency = item.Frequency,
                    Duration = item.Duration,
                    Instructions = item.Instructions
                })
                .ToList()
        };

        _context.Prescriptions.Add(prescription);

        await _context.SaveChangesAsync();

        return RedirectToAction(
            nameof(Details),
            new
            {
                prescriptionId = prescription.Id
            });
    }

    [HttpGet]
    public async Task<IActionResult> Details(
        int prescriptionId)
    {
        var prescription = await _context.Prescriptions
            .AsNoTracking()
            .Include(prescription => prescription.Patient)
            .Include(prescription => prescription.Doctor)
            .Include(prescription => prescription.Appointment)
            .Include(prescription => prescription.Items)
            .FirstOrDefaultAsync(
                prescription =>
                    prescription.Id == prescriptionId);

        if (prescription is null)
        {
            return NotFound();
        }

        return View(prescription);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadPdf(
        int prescriptionId)
    {
        var prescription = await _context.Prescriptions
            .AsNoTracking()
            .Include(prescription => prescription.Patient)
            .Include(prescription => prescription.Doctor)
            .Include(prescription => prescription.Appointment)
            .Include(prescription => prescription.Items)
            .FirstOrDefaultAsync(
                prescription =>
                    prescription.Id == prescriptionId);

        if (prescription is null)
        {
            return NotFound();
        }

        var pdfBytes = _pdfService.Generate(prescription);

        var fileName =
            $"Prescription-{prescription.Id}.pdf";

        return File(
            pdfBytes,
            "application/pdf",
            fileName);
    }
}