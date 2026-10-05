using ClinicManagement.Data;
using ClinicManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var now = DateTime.Now;
        var today = now.Date;
        var startOfWeek = today.AddDays(-6);
        var appointments = _context.Appointments
            .AsNoTracking()
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.Doctor)
            .AsQueryable();
        int? visiblePatientCount = null;

        if (User.IsInRole("Patient"))
        {
            var email = User.Identity?.Name;
            var patient = string.IsNullOrWhiteSpace(email)
                ? null
                : await _context.Patients
                    .AsNoTracking()
                    .FirstOrDefaultAsync(item => item.Email == email);

            visiblePatientCount = patient is null ? 0 : 1;
            appointments = patient is null
                ? appointments.Where(_ => false)
                : appointments.Where(item => item.PatientId == patient.Id);
        }
        else if (User.IsInRole("Doctor"))
        {
            var email = User.Identity?.Name;
            var doctor = string.IsNullOrWhiteSpace(email)
                ? null
                : await _context.Doctors
                    .AsNoTracking()
                    .FirstOrDefaultAsync(item => item.Email == email);

            appointments = doctor is null
                ? appointments.Where(_ => false)
                : appointments.Where(item => item.DoctorId == doctor.Id);
        }
        else if (!User.IsInRole("Admin")
            && !User.IsInRole("Receptionist"))
        {
            appointments = appointments.Where(_ => false);
        }

        var weeklyAppointments = await appointments
            .Where(item => item.AppointmentDate >= startOfWeek
                && item.AppointmentDate < today.AddDays(1))
            .Select(item => item.AppointmentDate)
            .ToListAsync();

        var weekActivity = Enumerable.Range(0, 7)
            .Select(offset =>
            {
                var day = startOfWeek.AddDays(offset);
                return new DailyAppointmentCount
                {
                    Date = day,
                    Count = weeklyAppointments.Count(date => date.Date == day)
                };
            })
            .ToList();

        var upcoming = await appointments
            .Where(item => item.Status == "Scheduled"
                && item.AppointmentDate >= now)
            .OrderBy(item => item.AppointmentDate)
            .Take(5)
            .ToListAsync();

        var model = new DashboardViewModel
        {
            AppointmentsToday = await appointments.CountAsync(item =>
                item.AppointmentDate >= today
                && item.AppointmentDate < today.AddDays(1)),
            UpcomingAppointments = await appointments.CountAsync(item =>
                item.Status == "Scheduled" && item.AppointmentDate >= now),
            PatientCount = visiblePatientCount
                ?? await _context.Patients.CountAsync(),
            AvailableDoctorCount = await _context.Doctors.CountAsync(
                doctor => doctor.IsAvailable),
            Upcoming = upcoming,
            WeeklyActivity = weekActivity
        };

        return View(model);
    }
}