using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreHub.Domain.Entities;

namespace CoreHub.Application.Interfaces;

/// <summary>
/// Repository interface for Invoice entity
/// </summary>
public interface IInvoiceRepository : IRepository<Invoice>
{
    /// <summary>
    /// Gets invoice by invoice number
    /// </summary>
    Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber);

    /// <summary>
    /// Gets invoices by patient
    /// </summary>
    Task<IEnumerable<Invoice>> GetByPatientAsync(Guid patientId);

    /// <summary>
    /// Gets invoices by status
    /// </summary>
    Task<IEnumerable<Invoice>> GetByStatusAsync(string status);

    /// <summary>
    /// Gets invoices with payments
    /// </summary>
    Task<Invoice?> GetWithPaymentsAsync(Guid id);

    /// <summary>
    /// Gets overdue invoices
    /// </summary>
    Task<IEnumerable<Invoice>> GetOverdueAsync();

    /// <summary>
    /// Gets invoices by date range
    /// </summary>
    Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets next invoice number
    /// </summary>
    Task<string> GetNextInvoiceNumberAsync();
}
