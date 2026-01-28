using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreHub.Application.Interfaces;
using CoreHub.Domain.Entities;
using CoreHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoreHub.Infrastructure.Repositories;

/// <summary>
/// Invoice repository implementation
/// </summary>
public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber)
    {
        return await _dbSet
            .Include(i => i.Patient)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);
    }

    public async Task<IEnumerable<Invoice>> GetByPatientAsync(Guid patientId)
    {
        return await _dbSet
            .Where(i => i.PatientId == patientId)
            .Include(i => i.Payments)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByStatusAsync(string status)
    {
        return await _dbSet
            .Where(i => i.Status == status)
            .Include(i => i.Patient)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();
    }

    public async Task<Invoice?> GetWithPaymentsAsync(Guid id)
    {
        return await _dbSet
            .Include(i => i.Patient)
            .Include(i => i.Payments)
            .Include(i => i.Organization)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Invoice>> GetOverdueAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(i => i.DueDate < now && 
                       i.Status != "Paid" && 
                       i.AmountDue > 0)
            .Include(i => i.Patient)
            .OrderBy(i => i.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
            .Include(i => i.Patient)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();
    }

    public async Task<string> GetNextInvoiceNumberAsync()
    {
        var currentYear = DateTime.UtcNow.Year;
        var prefix = $"INV-{currentYear}-";

        var lastInvoice = await _dbSet
            .Where(i => i.InvoiceNumber.StartsWith(prefix))
            .OrderByDescending(i => i.InvoiceNumber)
            .FirstOrDefaultAsync();

        if (lastInvoice == null)
        {
            return $"{prefix}0001";
        }

        // Extract the number part and increment
        var numberPart = lastInvoice.InvoiceNumber.Substring(prefix.Length);
        if (int.TryParse(numberPart, out int number))
        {
            return $"{prefix}{(number + 1):D4}";
        }

        return $"{prefix}0001";
    }
}
