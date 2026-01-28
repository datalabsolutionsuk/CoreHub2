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
/// Practitioner repository implementation
/// </summary>
public class PractitionerRepository : Repository<Practitioner>, IPractitionerRepository
{
    public PractitionerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Practitioner?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<Practitioner>> GetBySpecialtyAsync(string specialty)
    {
        return await _dbSet
            .Where(p => p.Specialty.ToLower() == specialty.ToLower())
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Practitioner>> GetByOrganizationAsync(Guid organizationId)
    {
        return await _dbSet
            .Where(p => p.OrganizationId == organizationId)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Practitioner>> GetActiveAsync()
    {
        return await _dbSet
            .Where(p => p.IsActive)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }
}
