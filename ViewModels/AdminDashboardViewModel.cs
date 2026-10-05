using ClinicManagement.Models;

namespace ClinicManagement.ViewModels;

public class AdminDashboardViewModel
{
    public int PatientCount { get; set; }

    public int DoctorCount { get; set; }

    public int AppointmentCount { get; set; }

    public int ScheduledAppointmentCount { get; set; }

    public IReadOnlyList<Appointment> RecentAppointments { get; set; }
        = Array.Empty<Appointment>();

    public IReadOnlyList<DailyAppointmentCount> WeeklyActivity { get; set; }
        = Array.Empty<DailyAppointmentCount>();
}
