using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreHub.Domain.Entities;

namespace CoreHub.Application.Interfaces;

/// <summary>
/// Repository interface for Practitioner entity
/// </summary>
public interface IPractitionerRepository : IRepository<Practitioner>
{
    /// <summary>
    /// Gets practitioner by email
    /// </summary>
    Task<Practitioner?> GetByEmailAsync(string email);

    /// <summary>
    /// Gets practitioners by specialty
    /// </summary>
    Task<IEnumerable<Practitioner>> GetBySpecialtyAsync(string specialty);

    /// <summary>
    /// Gets practitioners by organization
    /// </summary>
    Task<IEnumerable<Practitioner>> GetByOrganizationAsync(Guid organizationId);

    /// <summary>
    /// Gets active practitioners
    /// </summary>
    Task<IEnumerable<Practitioner>> GetActiveAsync();
}
