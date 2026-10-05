using ClinicManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<Doctor> Doctors => Set<Doctor>();

    public DbSet<Appointment> Appointments => Set<Appointment>();

    public DbSet<Prescription> Prescriptions
        => Set<Prescription>();

    public DbSet<PrescriptionItem> PrescriptionItems
        => Set<PrescriptionItem>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Appointment>()
            .HasOne(appointment => appointment.Patient)
            .WithMany(patient => patient.Appointments)
            .HasForeignKey(appointment => appointment.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(appointment => appointment.Doctor)
            .WithMany(doctor => doctor.Appointments)
            .HasForeignKey(appointment => appointment.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Prescription>()
            .HasOne(prescription => prescription.Appointment)
            .WithMany()
            .HasForeignKey(prescription => prescription.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Prescription>()
            .HasOne(prescription => prescription.Patient)
            .WithMany()
            .HasForeignKey(prescription => prescription.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Prescription>()
            .HasOne(prescription => prescription.Doctor)
            .WithMany()
            .HasForeignKey(prescription => prescription.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PrescriptionItem>()
            .HasOne(item => item.Prescription)
            .WithMany(prescription => prescription.Items)
            .HasForeignKey(item => item.PrescriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Prescription>()
            .HasIndex(prescription => prescription.AppointmentId)
            .IsUnique();
    }
}