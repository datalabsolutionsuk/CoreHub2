namespace CoreHub.Domain.Entities;

/// <summary>
/// Patient/Client entity - core of the CRM
/// </summary>
public class Patient : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? MobilePhone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    
    // Emergency Contact
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyContactRelationship { get; set; }
    
    // Medical Information
    public string? BloodType { get; set; }
    public string? Allergies { get; set; }
    public string? MedicalHistory { get; set; }
    public string? CurrentMedications { get; set; }
    
    // Insurance Information
    public string? InsuranceProvider { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public DateTime? InsuranceExpiryDate { get; set; }
    
    // Practice-specific
    public Guid? PrimaryPractitionerId { get; set; }
    public string? ReferralSource { get; set; }
    public string? Notes { get; set; }
    public string? Tags { get; set; }
    public string Status { get; set; } = "Active"; // Active, Inactive, Archived
    
    // Navigation Properties
    public Practitioner? PrimaryPractitioner { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<ClinicalNote> ClinicalNotes { get; set; } = new List<ClinicalNote>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
