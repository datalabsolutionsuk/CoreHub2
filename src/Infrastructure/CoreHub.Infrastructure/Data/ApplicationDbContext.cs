using CoreHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreHub.Infrastructure.Data;

/// <summary>
/// Main database context for CoreHub CRM
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Practitioner> Practitioners => Set<Practitioner>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<ClinicalNote> ClinicalNotes => Set<ClinicalNote>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<NoteTemplate> NoteTemplates => Set<NoteTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        ConfigurePatient(modelBuilder);
        ConfigurePractitioner(modelBuilder);
        ConfigureAppointment(modelBuilder);
        ConfigureClinicalNote(modelBuilder);
        ConfigureInvoice(modelBuilder);
        ConfigurePayment(modelBuilder);
        ConfigureOrganization(modelBuilder);
        ConfigureLocation(modelBuilder);
        ConfigureDocument(modelBuilder);
        ConfigureNoteTemplate(modelBuilder);
    }

    private void ConfigurePatient(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => new { e.FirstName, e.LastName });
            
            // Relationships
            entity.HasOne(p => p.PrimaryPractitioner)
                  .WithMany(pr => pr.PrimaryPatients)
                  .HasForeignKey(p => p.PrimaryPractitionerId)
                  .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasMany(p => p.Appointments)
                  .WithOne(a => a.Patient)
                  .HasForeignKey(a => a.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(p => p.ClinicalNotes)
                  .WithOne(cn => cn.Patient)
                  .HasForeignKey(cn => cn.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(p => p.Invoices)
                  .WithOne(i => i.Patient)
                  .HasForeignKey(i => i.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigurePractitioner(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Practitioner>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Specialty).IsRequired().HasMaxLength(100);
            
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.OrganizationId);
            
            // Relationships
            entity.HasOne(pr => pr.Organization)
                  .WithMany(o => o.Practitioners)
                  .HasForeignKey(pr => pr.OrganizationId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureAppointment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.AppointmentType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);
            
            entity.HasIndex(e => e.StartTime);
            entity.HasIndex(e => new { e.PatientId, e.StartTime });
            entity.HasIndex(e => new { e.PractitionerId, e.StartTime });
            
            // Relationships
            entity.HasOne(a => a.Patient)
                  .WithMany(p => p.Appointments)
                  .HasForeignKey(a => a.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(a => a.Practitioner)
                  .WithMany(pr => pr.Appointments)
                  .HasForeignKey(a => a.PractitionerId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(a => a.Location)
                  .WithMany(l => l.Appointments)
                  .HasForeignKey(a => a.LocationId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private void ConfigureClinicalNote(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClinicalNote>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.NoteType).HasMaxLength(100);
            
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.NoteDate);
            
            // Relationships
            entity.HasOne(cn => cn.Patient)
                  .WithMany(p => p.ClinicalNotes)
                  .HasForeignKey(cn => cn.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(cn => cn.Practitioner)
                  .WithMany(pr => pr.ClinicalNotes)
                  .HasForeignKey(cn => cn.PractitionerId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(cn => cn.Template)
                  .WithMany(nt => nt.ClinicalNotes)
                  .HasForeignKey(cn => cn.TemplateId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private void ConfigureInvoice(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.SubTotal).HasPrecision(18, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
            entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
            entity.Property(e => e.AmountPaid).HasPrecision(18, 2);
            entity.Property(e => e.AmountDue).HasPrecision(18, 2);
            
            entity.HasIndex(e => e.InvoiceNumber).IsUnique();
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.InvoiceDate);
            
            // Relationships
            entity.HasOne(i => i.Patient)
                  .WithMany(p => p.Invoices)
                  .HasForeignKey(i => i.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(i => i.Organization)
                  .WithMany(o => o.Invoices)
                  .HasForeignKey(i => i.OrganizationId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasMany(i => i.Payments)
                  .WithOne(p => p.Invoice)
                  .HasForeignKey(p => p.InvoiceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigurePayment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.PaymentNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            
            entity.HasIndex(e => e.PaymentNumber).IsUnique();
            entity.HasIndex(e => e.InvoiceId);
            entity.HasIndex(e => e.PaymentDate);
        });
    }

    private void ConfigureOrganization(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.OrganizationType).HasMaxLength(100);
            entity.Property(e => e.SubscriptionTier).HasMaxLength(50);
            
            entity.HasIndex(e => e.Email);
            
            // Relationships
            entity.HasMany(o => o.Locations)
                  .WithOne(l => l.Organization)
                  .HasForeignKey(l => l.OrganizationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureLocation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.LocationType).HasMaxLength(50);
            
            entity.HasIndex(e => e.OrganizationId);
        });
    }

    private void ConfigureDocument(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FileType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.MimeType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DocumentType).HasMaxLength(100);
            
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.CreatedAt);
        });
    }

    private void ConfigureNoteTemplate(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NoteTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.NoteType).HasMaxLength(100);
            
            entity.HasIndex(e => e.OrganizationId);
            entity.HasIndex(e => e.IsGlobal);
            
            // Relationships
            entity.HasOne(nt => nt.Organization)
                  .WithMany()
                  .HasForeignKey(nt => nt.OrganizationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
