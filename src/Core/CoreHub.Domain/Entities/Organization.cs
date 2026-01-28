namespace CoreHub.Domain.Entities;

/// <summary>
/// Organization/Practice entity - multi-tenancy support
/// </summary>
public class Organization : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? TaxId { get; set; }
    
    // Contact Information
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    
    // Organization Type
    public string OrganizationType { get; set; } = "Practice"; // Practice, Clinic, Hospital, etc.
    public string? Specialty { get; set; } // Psychology, Physiotherapy, General Practice, etc.
    
    // Branding
    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    
    // Settings
    public string? TimeZone { get; set; }
    public string? Currency { get; set; }
    public string? DateFormat { get; set; }
    public string? TimeFormat { get; set; }
    
    // Subscription & Billing
    public string SubscriptionTier { get; set; } = "Trial"; // Trial, Basic, Professional, Enterprise
    public DateTime? SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Compliance & Security
    public bool IsISO27001Certified { get; set; }
    public bool IsHIPAACompliant { get; set; }
    public string? ComplianceCertificates { get; set; } // JSON array
    
    // Navigation Properties
    public ICollection<Location> Locations { get; set; } = new List<Location>();
    public ICollection<Practitioner> Practitioners { get; set; } = new List<Practitioner>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
