using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreHub.Domain.Entities;

namespace CoreHub.Application.Interfaces;

/// <summary>
/// Repository interface for Patient entity with specific operations
/// </summary>
public interface IPatientRepository : IRepository<Patient>
{
    /// <summary>
    /// Gets patients by email address
    /// </summary>
    Task<Patient?> GetByEmailAsync(string email);

    /// <summary>
    /// Searches patients by name (first, last, or middle)
    /// </summary>
    Task<IEnumerable<Patient>> SearchByNameAsync(string searchTerm);

    /// <summary>
    /// Gets patients by status (Active, Inactive, Archived)
    /// </summary>
    Task<IEnumerable<Patient>> GetByStatusAsync(string status);

    /// <summary>
    /// Gets patients with their appointments
    /// </summary>
    Task<Patient?> GetWithAppointmentsAsync(Guid id);

    /// <summary>
    /// Gets patients with pagination
    /// </summary>
    Task<(IEnumerable<Patient> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null, 
        string? status = null);
}
