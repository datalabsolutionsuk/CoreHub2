namespace CoreHub.Domain.Entities;

/// <summary>
/// Practitioner/Healthcare Provider entity
/// </summary>
public class Practitioner : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? MobilePhone { get; set; }
    
    // Professional Information
    public string Specialty { get; set; } = string.Empty; // Psychology, Physiotherapy, etc.
    public string? LicenseNumber { get; set; }
    public DateTime? LicenseExpiryDate { get; set; }
    public string? Qualifications { get; set; }
    public string? Bio { get; set; }
    
    // Practice Information
    public Guid OrganizationId { get; set; }
    public string? Title { get; set; } // Dr., Mr., Mrs., etc.
    public string? Designation { get; set; } // Senior Psychologist, etc.
    public bool IsActive { get; set; } = true;
    public DateTime? JoinedDate { get; set; }
    
    // Schedule & Availability
    public string? WorkingHours { get; set; } // JSON or structured format
    public int DefaultAppointmentDuration { get; set; } = 60; // minutes
    public string? PreferredColor { get; set; } // For calendar display
    
    // User Account
    public string? UserId { get; set; } // Link to Identity user
    public string Role { get; set; } = "Practitioner"; // Admin, Practitioner, Receptionist
    
    // Navigation Properties
    public Organization? Organization { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<ClinicalNote> ClinicalNotes { get; set; } = new List<ClinicalNote>();
    public ICollection<Patient> PrimaryPatients { get; set; } = new List<Patient>();
}
