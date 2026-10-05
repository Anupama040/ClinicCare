using ClinicManagement.Data;
using ClinicManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var startOfWeek = today.AddDays(-6);

        var activityDates = await _context.Appointments
            .AsNoTracking()
            .Where(appointment => appointment.AppointmentDate >= startOfWeek
                && appointment.AppointmentDate < today.AddDays(1))
            .Select(appointment => appointment.AppointmentDate)
            .ToListAsync();

        var weeklyActivity = Enumerable.Range(0, 7)
            .Select(offset =>
            {
                var day = startOfWeek.AddDays(offset);
                return new DailyAppointmentCount
                {
                    Date = day,
                    Count = activityDates.Count(date => date.Date == day)
                };
            })
            .ToList();

        var model = new AdminDashboardViewModel
        {
            PatientCount = await _context.Patients.CountAsync(),
            DoctorCount = await _context.Doctors.CountAsync(),
            AppointmentCount = await _context.Appointments.CountAsync(),
            ScheduledAppointmentCount = await _context.Appointments.CountAsync(
                appointment => appointment.Status == "Scheduled"),
            RecentAppointments = await _context.Appointments
                .AsNoTracking()
                .Include(appointment => appointment.Patient)
                .Include(appointment => appointment.Doctor)
                .OrderByDescending(appointment => appointment.CreatedAt)
                .Take(6)
                .ToListAsync(),
            WeeklyActivity = weeklyActivity
        };

        return View(model);
    }
}