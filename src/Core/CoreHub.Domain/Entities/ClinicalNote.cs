namespace CoreHub.Domain.Entities;

/// <summary>
/// Clinical Notes entity - secure patient notes with templates
/// </summary>
public class ClinicalNote : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid PractitionerId { get; set; }
    public Guid? AppointmentId { get; set; }
    public Guid? TemplateId { get; set; }
    
    // Note Details
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; // Rich text or structured format
    public DateTime NoteDate { get; set; }
    public string NoteType { get; set; } = "Progress Note"; // Progress Note, Assessment, Treatment Plan, etc.
    
    // Clinical Information
    public string? Diagnosis { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? Observations { get; set; }
    public string? Recommendations { get; set; }
    public string? FollowUpRequired { get; set; }
    
    // Security & Compliance
    public bool IsLocked { get; set; } // Prevent editing after a certain period
    public DateTime? LockedAt { get; set; }
    public string? LockedBy { get; set; }
    public bool IsSigned { get; set; }
    public DateTime? SignedAt { get; set; }
    public string? SignedBy { get; set; }
    
    // Versioning & Audit
    public int Version { get; set; } = 1;
    public string? EditHistory { get; set; } // JSON array of edits
    
    // Attachments & Tags
    public string? Tags { get; set; }
    public bool HasAttachments { get; set; }
    
    // Navigation Properties
    public Patient? Patient { get; set; }
    public Practitioner? Practitioner { get; set; }
    public Appointment? Appointment { get; set; }
    public NoteTemplate? Template { get; set; }
    public ICollection<Document>? Attachments { get; set; }
}
