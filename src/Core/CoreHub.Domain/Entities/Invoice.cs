namespace CoreHub.Domain.Entities;

/// <summary>
/// Invoice entity - billing and payments
/// </summary>
public class Invoice : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public Guid OrganizationId { get; set; }
    
    // Invoice Details
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Sent, Paid, Overdue, Cancelled
    
    // Financial Details
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountDue { get; set; }
    
    // Line Items (stored as JSON or separate table)
    public string LineItems { get; set; } = "[]"; // JSON array of line items
    
    // Payment Information
    public DateTime? PaidAt { get; set; }
    public string? PaymentMethod { get; set; } // Card, Cash, Bank Transfer, Insurance
    public string? PaymentReference { get; set; }
    
    // Insurance Claims
    public bool IsInsuranceClaim { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? ClaimNumber { get; set; }
    public DateTime? ClaimSubmittedAt { get; set; }
    public string? ClaimStatus { get; set; }
    
    // Communication
    public bool ReminderSent { get; set; }
    public DateTime? ReminderSentAt { get; set; }
    public DateTime? LastReminderSentAt { get; set; }
    public int ReminderCount { get; set; }
    
    // Notes
    public string? Notes { get; set; }
    public string? Terms { get; set; }
    
    // Navigation Properties
    public Patient? Patient { get; set; }
    public Appointment? Appointment { get; set; }
    public Organization? Organization { get; set; }
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
