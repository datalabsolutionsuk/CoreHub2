namespace CoreHub.Domain.Entities;

/// <summary>
/// Document entity - file attachments and forms
/// </summary>
public class Document : BaseEntity
{
    public Guid? PatientId { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? ClinicalNoteId { get; set; }
    public Guid? AppointmentId { get; set; }
    
    // Document Details
    public string FileName { get; set; } = string.Empty;
    public string? OriginalFileName { get; set; }
    public string FileType { get; set; } = string.Empty; // PDF, DOC, JPG, etc.
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    
    // Document Type & Category
    public string DocumentType { get; set; } = "General"; // Form, Consent, Report, Image, etc.
    public string? Category { get; set; }
    public string? Description { get; set; }
    
    // Security
    public bool IsEncrypted { get; set; }
    public bool IsPublic { get; set; }
    public string? AccessLevel { get; set; } // Private, Patient, Practitioner, Organization
    
    // Versioning
    public int Version { get; set; } = 1;
    public Guid? ParentDocumentId { get; set; }
    
    // Metadata
    public string? Tags { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsArchived { get; set; }
    
    // Form-specific
    public bool IsForm { get; set; }
    public string? FormData { get; set; } // JSON
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    // Navigation Properties
    public Patient? Patient { get; set; }
    public Organization? Organization { get; set; }
    public ClinicalNote? ClinicalNote { get; set; }
    public Appointment? Appointment { get; set; }
}
