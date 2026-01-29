using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreHub.Application.Interfaces;
using CoreHub.Domain.Entities;
using CoreHub.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CoreHub.Infrastructure.Repositories;

/// <summary>
/// Invoice repository implementation
/// </summary>
public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) 
        : base(context, httpContextAccessor)
    {
    }

    public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber)
    {
        var query = ApplyOrganizationFilter(_dbSet);
        return await query
            .Include(i => i.Patient)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);
    }

    public async Task<IEnumerable<Invoice>> GetByPatientAsync(Guid patientId)
    {
        var query = ApplyOrganizationFilter(_dbSet);
        return await query
            .Where(i => i.PatientId == patientId)
            .Include(i => i.Payments)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByStatusAsync(string status)
    {
        var query = ApplyOrganizationFilter(_dbSet);
        return await query
            .Where(i => i.Status == status)
            .Include(i => i.Patient)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();
    }

    public async Task<Invoice?> GetWithPaymentsAsync(Guid id)
    {
        var query = ApplyOrganizationFilter(_dbSet);
        return await query
            .Include(i => i.Patient)
            .Include(i => i.Payments)
            .Include(i => i.Organization)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Invoice>> GetOverdueAsync()
    {
        var now = DateTime.UtcNow;
        var query = ApplyOrganizationFilter(_dbSet);
        return await query
            .Where(i => i.DueDate < now && 
                       i.Status != "Paid" && 
                       i.AmountDue > 0)
            .Include(i => i.Patient)
            .OrderBy(i => i.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var query = ApplyOrganizationFilter(_dbSet);
        return await query
            .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
            .Include(i => i.Patient)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();
    }

    public async Task<string> GetNextInvoiceNumberAsync()
    {
        var currentYear = DateTime.UtcNow.Year;
        var prefix = $"INV-{currentYear}-";

        var query = ApplyOrganizationFilter(_dbSet);
        var lastInvoice = await query
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
