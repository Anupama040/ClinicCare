using ClinicManagement.Data;
using ClinicManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Controllers;

[Authorize(Roles = "Admin,Receptionist")]
public class DoctorsController : Controller
{
    private readonly ApplicationDbContext _context;

    public DoctorsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var doctors = await _context.Doctors
            .AsNoTracking()
            .OrderBy(doctor => doctor.LastName)
            .ThenBy(doctor => doctor.FirstName)
            .ToListAsync();

        return View(doctors);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var doctor = await _context.Doctors
            .AsNoTracking()
            .Include(doctor => doctor.Appointments)
            .FirstOrDefaultAsync(doctor => doctor.Id == id);

        if (doctor is null)
        {
            return NotFound();
        }

        return View(doctor);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Doctor doctor)
    {
        if (!ModelState.IsValid)
        {
            return View(doctor);
        }

        doctor.CreatedAt = DateTime.UtcNow;

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Doctor added successfully.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var doctor = await _context.Doctors
            .FindAsync(id);

        if (doctor is null)
        {
            return NotFound();
        }

        return View(doctor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        Doctor doctor)
    {
        if (id != doctor.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(doctor);
        }

        try
        {
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Doctor updated successfully.";
        }
        catch (DbUpdateConcurrencyException)
        {
            var doctorExists = await _context.Doctors
                .AnyAsync(existingDoctor => existingDoctor.Id == id);

            if (!doctorExists)
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

        var doctor = await _context.Doctors
            .AsNoTracking()
            .FirstOrDefaultAsync(doctor => doctor.Id == id);

        if (doctor is null)
        {
            return NotFound();
        }

        return View(doctor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var doctor = await _context.Doctors
            .FindAsync(id);

        if (doctor is null)
        {
            return NotFound();
        }

        var hasAppointments = await _context.Appointments
            .AnyAsync(appointment => appointment.DoctorId == id);

        if (hasAppointments)
        {
            TempData["ErrorMessage"] =
                "This doctor cannot be deleted because appointments exist.";

            return RedirectToAction(nameof(Index));
        }

        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Doctor deleted successfully.";

        return RedirectToAction(nameof(Index));
    }
}