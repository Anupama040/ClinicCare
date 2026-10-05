using ClinicManagement.Models;

namespace ClinicManagement.ViewModels;

public class DashboardViewModel
{
    public int AppointmentsToday { get; set; }

    public int UpcomingAppointments { get; set; }

    public int PatientCount { get; set; }

    public int AvailableDoctorCount { get; set; }

    public IReadOnlyList<Appointment> Upcoming { get; set; }
        = Array.Empty<Appointment>();

    public IReadOnlyList<DailyAppointmentCount> WeeklyActivity { get; set; }
        = Array.Empty<DailyAppointmentCount>();
}

public class DailyAppointmentCount
{
    public DateTime Date { get; set; }

    public int Count { get; set; }
}
