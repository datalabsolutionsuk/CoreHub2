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
/// Patient repository implementation
/// </summary>
public class PatientRepository : Repository<Patient>, IPatientRepository
{
    public PatientRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Patient?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<Patient>> SearchByNameAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _dbSet
            .Where(p => p.FirstName.ToLower().Contains(term) ||
                       p.LastName.ToLower().Contains(term) ||
                       (p.MiddleName != null && p.MiddleName.ToLower().Contains(term)))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Patient>> GetByStatusAsync(string status)
    {
        return await _dbSet
            .Where(p => p.Status == status)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    public async Task<Patient?> GetWithAppointmentsAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Appointments)
            .Include(p => p.PrimaryPractitioner)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<(IEnumerable<Patient> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null, 
        string? status = null)
    {
        var query = _dbSet.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(p => 
                p.FirstName.ToLower().Contains(term) ||
                p.LastName.ToLower().Contains(term) ||
                p.Email.ToLower().Contains(term) ||
                (p.Phone != null && p.Phone.Contains(term)) ||
                (p.MobilePhone != null && p.MobilePhone.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(p => p.Status == status);
        }

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(p => p.PrimaryPractitioner)
            .ToListAsync();

        return (items, totalCount);
    }
}
