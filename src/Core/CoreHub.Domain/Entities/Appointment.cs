namespace CoreHub.Domain.Entities;

/// <summary>
/// Appointment entity - calendar management
/// </summary>
public class Appointment : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid PractitionerId { get; set; }
    public Guid? LocationId { get; set; }
    
    // Appointment Details
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public string AppointmentType { get; set; } = string.Empty; // Initial Consultation, Follow-up, etc.
    public string Status { get; set; } = "Scheduled"; // Scheduled, Confirmed, In Progress, Completed, Cancelled, No Show
    
    // Communication
    public bool ReminderSent { get; set; }
    public DateTime? ReminderSentAt { get; set; }
    public bool ConfirmationSent { get; set; }
    public DateTime? ConfirmationSentAt { get; set; }
    
    // Telehealth
    public bool IsTelehealth { get; set; }
    public string? TelehealthLink { get; set; }
    public string? TelehealthProvider { get; set; } // Zoom, Teams, etc.
    
    // Notes & Billing
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public decimal? EstimatedCost { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; } // JSON format
    public Guid? ParentAppointmentId { get; set; } // For recurring appointments
    
    // Waitlist
    public bool IsWaitlisted { get; set; }
    public int? WaitlistPriority { get; set; }
    
    // Navigation Properties
    public Patient? Patient { get; set; }
    public Practitioner? Practitioner { get; set; }
    public Location? Location { get; set; }
    public ICollection<ClinicalNote>? ClinicalNotes { get; set; }
    public Invoice? Invoice { get; set; }
}
