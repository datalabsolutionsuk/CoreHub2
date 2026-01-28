using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreHub.Application.Interfaces
{
    public interface IPatientRepository
    {
        Task<Patient> GetByIdAsync(Guid id);
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient> AddAsync(Patient patient);
        Task UpdateAsync(Patient patient);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
