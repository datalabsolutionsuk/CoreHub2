namespace CoreHub.Domain.Entities;

/// <summary>
/// Payment entity - payment tracking
/// </summary>
public class Payment : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public Guid PatientId { get; set; }
    public Guid OrganizationId { get; set; }
    
    // Payment Details
    public string PaymentNumber { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // Card, Cash, Bank Transfer, etc.
    public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Refunded
    
    // Payment Gateway Information
    public string? TransactionId { get; set; }
    public string? PaymentGateway { get; set; } // Stripe, PayPal, etc.
    public string? PaymentGatewayResponse { get; set; } // JSON
    
    // Card Information (last 4 digits only)
    public string? CardLastFour { get; set; }
    public string? CardType { get; set; }
    
    // Refund Information
    public bool IsRefund { get; set; }
    public Guid? RefundedPaymentId { get; set; }
    public string? RefundReason { get; set; }
    public DateTime? RefundedAt { get; set; }
    
    // Notes
    public string? Notes { get; set; }
    public string? ReceiptNumber { get; set; }
    
    // Navigation Properties
    public Invoice? Invoice { get; set; }
    public Patient? Patient { get; set; }
    public Organization? Organization { get; set; }
}
