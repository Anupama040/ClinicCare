using ClinicManagement.Data;
using ClinicManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Controllers;

[Authorize(Roles = "Admin,Receptionist,Doctor")]
public class PatientsController : Controller
{
    private readonly ApplicationDbContext _context;

    public PatientsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var patients = await _context.Patients
            .AsNoTracking()
            .OrderBy(patient => patient.LastName)
            .ThenBy(patient => patient.FirstName)
            .ToListAsync();

        return View(patients);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var patient = await _context.Patients
            .AsNoTracking()
            .Include(patient => patient.Appointments)
            .FirstOrDefaultAsync(patient => patient.Id == id);

        if (patient is null)
        {
            return NotFound();
        }

        return View(patient);
    }

    [Authorize(Roles = "Admin,Receptionist")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Create(Patient patient)
    {
        if (!ModelState.IsValid)
        {
            return View(patient);
        }

        patient.CreatedAt = DateTime.UtcNow;

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Patient registered successfully.";

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var patient = await _context.Patients
            .FindAsync(id);

        if (patient is null)
        {
            return NotFound();
        }

        return View(patient);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Edit(
        int id,
        Patient patient)
    {
        if (id != patient.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(patient);
        }

        try
        {
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Patient updated successfully.";
        }
        catch (DbUpdateConcurrencyException)
        {
            var patientExists = await _context.Patients
                .AnyAsync(existingPatient => existingPatient.Id == id);

            if (!patientExists)
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(patient => patient.Id == id);

        if (patient is null)
        {
            return NotFound();
        }

        return View(patient);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var patient = await _context.Patients
            .FindAsync(id);

        if (patient is null)
        {
            return NotFound();
        }

        var hasAppointments = await _context.Appointments
            .AnyAsync(appointment => appointment.PatientId == id);

        if (hasAppointments)
        {
            TempData["ErrorMessage"] =
                "This patient cannot be deleted because appointments exist.";

            return RedirectToAction(nameof(Index));
        }

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Patient deleted successfully.";

        return RedirectToAction(nameof(Index));
    }
}