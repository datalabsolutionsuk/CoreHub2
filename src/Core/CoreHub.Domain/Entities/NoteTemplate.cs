namespace CoreHub.Domain.Entities;

/// <summary>
/// Note Template entity - reusable clinical note templates
/// </summary>
public class NoteTemplate : BaseEntity
{
    public Guid OrganizationId { get; set; }
    public Guid? PractitionerId { get; set; } // If practitioner-specific
    
    // Template Details
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Content { get; set; } = string.Empty; // Template structure/fields
    public string NoteType { get; set; } = "Progress Note";
    
    // Template Structure
    public string? FieldDefinitions { get; set; } // JSON array of fields
    public string? DefaultValues { get; set; } // JSON object
    
    // Categorization
    public string? Specialty { get; set; } // Psychology, Physiotherapy, etc.
    public string? Tags { get; set; }
    public string? Category { get; set; }
    
    // Usage & Sharing
    public bool IsGlobal { get; set; } // Available to all in organization
    public bool IsActive { get; set; } = true;
    public int UsageCount { get; set; }
    public DateTime? LastUsedAt { get; set; }
    
    // Versioning
    public int Version { get; set; } = 1;
    public Guid? ParentTemplateId { get; set; }
    
    // Navigation Properties
    public Organization? Organization { get; set; }
    public Practitioner? Practitioner { get; set; }
    public ICollection<ClinicalNote> ClinicalNotes { get; set; } = new List<ClinicalNote>();
}
