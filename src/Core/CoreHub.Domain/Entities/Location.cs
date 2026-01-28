namespace CoreHub.Domain.Entities;

/// <summary>
/// Location entity - practice locations/rooms
/// </summary>
public class Location : BaseEntity
{
    public Guid OrganizationId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string LocationType { get; set; } = "Office"; // Office, Room, Virtual, etc.
    
    // Address Information
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    
    // Contact & Facilities
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int? Capacity { get; set; }
    public string? Facilities { get; set; } // JSON array
    public bool IsAccessible { get; set; }
    
    // Availability
    public bool IsActive { get; set; } = true;
    public string? WorkingHours { get; set; } // JSON format
    
    // Navigation Properties
    public Organization? Organization { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
