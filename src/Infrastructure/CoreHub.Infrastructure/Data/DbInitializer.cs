using CoreHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoreHub.Infrastructure.Data;

/// <summary>
/// Database initializer for seeding test data
/// </summary>
public class DbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(ApplicationDbContext context, ILogger<DbInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Seeds the database with initial test data
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            // Check if data already exists
            if (await _context.Organizations.AnyAsync())
            {
                _logger.LogInformation("Database already seeded. Skipping seed operation.");
                return;
            }

            _logger.LogInformation("Starting database seed...");

            // Create Organization
            var organization = new Organization
            {
                Id = Guid.NewGuid(),
                Name = "Demo Healthcare Clinic",
                LegalName = "Demo Healthcare Clinic Pty Ltd",
                Email = "demo@healthcare.com",
                Phone = "+61 2 9000 0000",
                Address = "123 Health Street",
                City = "Sydney",
                State = "NSW",
                PostalCode = "2000",
                Country = "Australia",
                OrganizationType = "Clinic",
                Specialty = "Multi-disciplinary",
                TimeZone = "Australia/Sydney",
                Currency = "AUD",
                DateFormat = "dd/MM/yyyy",
                TimeFormat = "HH:mm",
                SubscriptionTier = "Professional",
                SubscriptionStartDate = DateTime.UtcNow.AddMonths(-6),
                SubscriptionEndDate = DateTime.UtcNow.AddMonths(6),
                IsActive = true,
                IsHIPAACompliant = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Organizations.AddAsync(organization);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created organization: {Name}", organization.Name);

            // Create Practitioners
            var drSarah = new Practitioner
            {
                Id = Guid.NewGuid(),
                FirstName = "Sarah",
                LastName = "Smith",
                Title = "Dr.",
                Email = "sarah.smith@demo.com",
                Phone = "+61 2 9000 0001",
                MobilePhone = "+61 400 000 001",
                Specialty = "Psychology",
                LicenseNumber = "PSY-2024-001",
                LicenseExpiryDate = DateTime.UtcNow.AddYears(2),
                Qualifications = "PhD Clinical Psychology, MPsych, BA (Hons)",
                Bio = "Dr. Sarah Smith is a clinical psychologist with over 15 years of experience in cognitive behavioral therapy and trauma-informed care.",
                OrganizationId = organization.Id,
                Designation = "Senior Clinical Psychologist",
                IsActive = true,
                JoinedDate = DateTime.UtcNow.AddYears(-3),
                DefaultAppointmentDuration = 50,
                PreferredColor = "#4CAF50",
                Role = "Practitioner",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var drJohn = new Practitioner
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Jones",
                Title = "Dr.",
                Email = "john.jones@demo.com",
                Phone = "+61 2 9000 0002",
                MobilePhone = "+61 400 000 002",
                Specialty = "Physiotherapy",
                LicenseNumber = "PHY-2024-001",
                LicenseExpiryDate = DateTime.UtcNow.AddYears(2),
                Qualifications = "DPT, MSc Sports Medicine, BSc Physiotherapy",
                Bio = "Dr. John Jones specializes in sports rehabilitation and musculoskeletal disorders with 10 years of clinical experience.",
                OrganizationId = organization.Id,
                Designation = "Senior Physiotherapist",
                IsActive = true,
                JoinedDate = DateTime.UtcNow.AddYears(-2),
                DefaultAppointmentDuration = 45,
                PreferredColor = "#2196F3",
                Role = "Practitioner",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Practitioners.AddRangeAsync(drSarah, drJohn);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created practitioners: Dr. Sarah Smith, Dr. John Jones");

            // Create Patients
            var patients = new List<Patient>
            {
                new Patient
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Emily",
                    LastName = "Thompson",
                    Email = "emily.thompson@email.com",
                    Phone = "+61 2 9100 0001",
                    MobilePhone = "+61 411 000 001",
                    DateOfBirth = new DateTime(1985, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                    Gender = "Female",
                    Address = "45 Wellness Avenue",
                    City = "Sydney",
                    State = "NSW",
                    PostalCode = "2010",
                    Country = "Australia",
                    BloodType = "A+",
                    Allergies = "Penicillin",
                    MedicalHistory = "Anxiety disorder, managed with therapy",
                    EmergencyContactName = "Michael Thompson",
                    EmergencyContactPhone = "+61 411 000 011",
                    EmergencyContactRelationship = "Spouse",
                    InsuranceProvider = "Medibank Private",
                    InsurancePolicyNumber = "MBP-2024-001234",
                    InsuranceExpiryDate = DateTime.UtcNow.AddYears(1),
                    PrimaryPractitionerId = drSarah.Id,
                    ReferralSource = "GP Referral",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Patient
                {
                    Id = Guid.NewGuid(),
                    FirstName = "James",
                    LastName = "Wilson",
                    Email = "james.wilson@email.com",
                    Phone = "+61 2 9100 0002",
                    MobilePhone = "+61 411 000 002",
                    DateOfBirth = new DateTime(1978, 7, 22, 0, 0, 0, DateTimeKind.Utc),
                    Gender = "Male",
                    Address = "78 Fitness Road",
                    City = "Bondi",
                    State = "NSW",
                    PostalCode = "2026",
                    Country = "Australia",
                    BloodType = "O-",
                    Allergies = "None known",
                    MedicalHistory = "ACL reconstruction (2020), recovering well",
                    CurrentMedications = "Glucosamine supplements",
                    EmergencyContactName = "Sarah Wilson",
                    EmergencyContactPhone = "+61 411 000 012",
                    EmergencyContactRelationship = "Wife",
                    InsuranceProvider = "Bupa",
                    InsurancePolicyNumber = "BUPA-2024-005678",
                    InsuranceExpiryDate = DateTime.UtcNow.AddMonths(8),
                    PrimaryPractitionerId = drJohn.Id,
                    ReferralSource = "Sports Club",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Patient
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Olivia",
                    LastName = "Martinez",
                    Email = "olivia.martinez@email.com",
                    Phone = "+61 2 9100 0003",
                    MobilePhone = "+61 411 000 003",
                    DateOfBirth = new DateTime(1992, 11, 8, 0, 0, 0, DateTimeKind.Utc),
                    Gender = "Female",
                    Address = "22 Calm Street",
                    City = "Surry Hills",
                    State = "NSW",
                    PostalCode = "2010",
                    Country = "Australia",
                    BloodType = "B+",
                    Allergies = "Latex",
                    MedicalHistory = "Depression, currently in remission",
                    CurrentMedications = "Sertraline 50mg daily",
                    EmergencyContactName = "Carlos Martinez",
                    EmergencyContactPhone = "+61 411 000 013",
                    EmergencyContactRelationship = "Father",
                    InsuranceProvider = "HCF",
                    InsurancePolicyNumber = "HCF-2024-009012",
                    InsuranceExpiryDate = DateTime.UtcNow.AddMonths(10),
                    PrimaryPractitionerId = drSarah.Id,
                    ReferralSource = "Online Search",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Patient
                {
                    Id = Guid.NewGuid(),
                    FirstName = "William",
                    LastName = "Chen",
                    Email = "william.chen@email.com",
                    Phone = "+61 2 9100 0004",
                    MobilePhone = "+61 411 000 004",
                    DateOfBirth = new DateTime(1965, 5, 30, 0, 0, 0, DateTimeKind.Utc),
                    Gender = "Male",
                    Address = "156 Recovery Lane",
                    City = "Chatswood",
                    State = "NSW",
                    PostalCode = "2067",
                    Country = "Australia",
                    BloodType = "AB+",
                    Allergies = "Aspirin, Ibuprofen",
                    MedicalHistory = "Chronic lower back pain, Type 2 Diabetes",
                    CurrentMedications = "Metformin 500mg twice daily, Paracetamol as needed",
                    EmergencyContactName = "Linda Chen",
                    EmergencyContactPhone = "+61 411 000 014",
                    EmergencyContactRelationship = "Wife",
                    InsuranceProvider = "NIB",
                    InsurancePolicyNumber = "NIB-2024-003456",
                    InsuranceExpiryDate = DateTime.UtcNow.AddMonths(5),
                    PrimaryPractitionerId = drJohn.Id,
                    ReferralSource = "GP Referral",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Patient
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Sophie",
                    LastName = "Anderson",
                    Email = "sophie.anderson@email.com",
                    Phone = "+61 2 9100 0005",
                    MobilePhone = "+61 411 000 005",
                    DateOfBirth = new DateTime(2000, 9, 12, 0, 0, 0, DateTimeKind.Utc),
                    Gender = "Female",
                    Address = "89 Student Avenue",
                    City = "Randwick",
                    State = "NSW",
                    PostalCode = "2031",
                    Country = "Australia",
                    BloodType = "A-",
                    Allergies = "None known",
                    MedicalHistory = "University student experiencing exam-related stress",
                    EmergencyContactName = "David Anderson",
                    EmergencyContactPhone = "+61 411 000 015",
                    EmergencyContactRelationship = "Father",
                    InsuranceProvider = "Medibank Private",
                    InsurancePolicyNumber = "MBP-2024-007890",
                    InsuranceExpiryDate = DateTime.UtcNow.AddMonths(14),
                    PrimaryPractitionerId = drSarah.Id,
                    ReferralSource = "University Health Service",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await _context.Patients.AddRangeAsync(patients);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created {Count} patients", patients.Count);

            // Create Appointments (1 past, 1 today, 1 future)
            var today = DateTime.UtcNow.Date;
            var appointments = new List<Appointment>
            {
                // Past appointment (yesterday) - Completed
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = patients[0].Id,
                    PractitionerId = drSarah.Id,
                    StartTime = today.AddDays(-1).AddHours(10), // Yesterday 10:00 AM UTC
                    EndTime = today.AddDays(-1).AddHours(10).AddMinutes(50),
                    DurationMinutes = 50,
                    AppointmentType = "Follow-up Session",
                    Status = "Completed",
                    ReminderSent = true,
                    ReminderSentAt = today.AddDays(-2),
                    ConfirmationSent = true,
                    ConfirmationSentAt = today.AddDays(-1).AddHours(-1),
                    IsTelehealth = false,
                    Notes = "Patient showed good progress. Continue current treatment plan.",
                    EstimatedCost = 180.00m,
                    IsRecurring = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                // Today's appointment - Confirmed
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = patients[1].Id,
                    PractitionerId = drJohn.Id,
                    StartTime = today.AddHours(14), // Today 2:00 PM UTC
                    EndTime = today.AddHours(14).AddMinutes(45),
                    DurationMinutes = 45,
                    AppointmentType = "Physiotherapy Session",
                    Status = "Confirmed",
                    ReminderSent = true,
                    ReminderSentAt = today.AddDays(-1),
                    ConfirmationSent = true,
                    ConfirmationSentAt = today.AddHours(-2),
                    IsTelehealth = false,
                    Notes = "Focus on knee rehabilitation exercises.",
                    EstimatedCost = 150.00m,
                    IsRecurring = true,
                    RecurrencePattern = "{\"frequency\":\"weekly\",\"dayOfWeek\":\"Wednesday\",\"endAfter\":8}",
                    CreatedAt = DateTime.UtcNow.AddDays(-14),
                    UpdatedAt = DateTime.UtcNow
                },
                // Future appointment (tomorrow) - Scheduled
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = patients[2].Id,
                    PractitionerId = drSarah.Id,
                    StartTime = today.AddDays(1).AddHours(11), // Tomorrow 11:00 AM UTC
                    EndTime = today.AddDays(1).AddHours(11).AddMinutes(50),
                    DurationMinutes = 50,
                    AppointmentType = "Initial Consultation",
                    Status = "Scheduled",
                    ReminderSent = false,
                    ConfirmationSent = false,
                    IsTelehealth = true,
                    TelehealthLink = "https://telehealth.demo.com/room/abc123",
                    TelehealthProvider = "Zoom",
                    Notes = "New patient - initial assessment for anxiety management.",
                    EstimatedCost = 220.00m,
                    IsRecurring = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await _context.Appointments.AddRangeAsync(appointments);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created {Count} appointments (1 past, 1 today, 1 future)", appointments.Count);

            _logger.LogInformation("Database seed completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
}
