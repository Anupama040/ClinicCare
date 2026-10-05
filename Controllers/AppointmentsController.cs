using ClinicManagement.Data;
using ClinicManagement.Models;
using ClinicManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Controllers;

[Authorize]
public class AppointmentsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AppointmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin,Doctor,Receptionist")]
    public async Task<IActionResult> Index()
    {
        var appointments = await _context.Appointments
            .AsNoTracking()
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.Doctor)
            .OrderBy(appointment => appointment.AppointmentDate)
            .ToListAsync();

        return View(appointments);
    }

    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> MyAppointments()
    {
        var email = User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(email))
        {
            return Challenge();
        }

        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(patient => patient.Email == email);

        if (patient is null)
        {
            return View(new List<Appointment>());
        }

        var appointments = await _context.Appointments
            .AsNoTracking()
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.Doctor)
            .Where(appointment => appointment.PatientId == patient.Id)
            .OrderBy(appointment => appointment.AppointmentDate)
            .ToListAsync();

        return View(appointments);
    }

    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Create()
    {
        var model = new AppointmentViewModel
        {
            AppointmentDate = DateTime.Now.AddHours(1)
        };

        await LoadDropdownsAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Create(
        AppointmentViewModel model)
    {
        if (model.AppointmentDate <= DateTime.Now)
        {
            ModelState.AddModelError(
                nameof(model.AppointmentDate),
                "Appointment must be scheduled for a future time.");
        }

        if (model.AppointmentDate.TimeOfDay < TimeSpan.FromHours(9)
            || model.AppointmentDate.TimeOfDay > TimeSpan.FromHours(18))
        {
            ModelState.AddModelError(
                nameof(model.AppointmentDate),
                "Appointments must be between 9:00 AM and 6:00 PM.");
        }

        var doctorExists = await _context.Doctors
            .AnyAsync(doctor =>
                doctor.Id == model.DoctorId
                && doctor.IsAvailable);

        if (!doctorExists)
        {
            ModelState.AddModelError(
                nameof(model.DoctorId),
                "The selected doctor is not available.");
        }

        var doctorHasConflict = await _context.Appointments
            .AnyAsync(appointment =>
                appointment.DoctorId == model.DoctorId
                && appointment.AppointmentDate == model.AppointmentDate
                && appointment.Status == "Scheduled");

        if (doctorHasConflict)
        {
            ModelState.AddModelError(
                nameof(model.AppointmentDate),
                "The doctor already has an appointment at this time.");
        }

        var patientHasConflict = await _context.Appointments
            .AnyAsync(appointment =>
                appointment.PatientId == model.PatientId
                && appointment.AppointmentDate == model.AppointmentDate
                && appointment.Status == "Scheduled");

        if (patientHasConflict)
        {
            ModelState.AddModelError(
                nameof(model.AppointmentDate),
                "The patient already has an appointment at this time.");
        }

        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync(model);

            return View(model);
        }

        var appointment = new Appointment
        {
            PatientId = model.PatientId,
            DoctorId = model.DoctorId,
            AppointmentDate = model.AppointmentDate,
            Notes = model.Notes,
            Status = "Scheduled",
            CreatedAt = DateTime.UtcNow
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Appointment scheduled successfully.";

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Doctor,Receptionist")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments
            .AsNoTracking()
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.Doctor)
            .FirstOrDefaultAsync(appointment => appointment.Id == id);

        if (appointment is null)
        {
            return NotFound();
        }

        return View(appointment);
    }

    [Authorize(Roles = "Admin,Doctor,Receptionist")]
    public async Task<IActionResult> Complete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments
            .FindAsync(id);

        if (appointment is null)
        {
            return NotFound();
        }

        appointment.Status = "Completed";

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Appointment marked as completed.";

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Cancel(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments
            .FindAsync(id);

        if (appointment is null)
        {
            return NotFound();
        }

        appointment.Status = "Cancelled";

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Appointment cancelled successfully.";

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdownsAsync(
        AppointmentViewModel model)
    {
        model.Patients = await _context.Patients
            .AsNoTracking()
            .OrderBy(patient => patient.LastName)
            .Select(patient => new SelectListItem
            {
                Value = patient.Id.ToString(),
                Text = patient.FirstName
                    + " "
                    + patient.LastName
                    + " - "
                    + patient.Phone
            })
            .ToListAsync();

        model.Doctors = await _context.Doctors
            .AsNoTracking()
            .Where(doctor => doctor.IsAvailable)
            .OrderBy(doctor => doctor.LastName)
            .Select(doctor => new SelectListItem
            {
                Value = doctor.Id.ToString(),
                Text = "Dr. "
                    + doctor.FirstName
                    + " "
                    + doctor.LastName
                    + " - "
                    + doctor.Specialization
            })
            .ToListAsync();
    }
}