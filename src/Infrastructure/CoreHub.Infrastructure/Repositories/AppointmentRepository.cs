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
/// Appointment repository implementation
/// </summary>
public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) 
        : base(context, httpContextAccessor)
    {
    }

    public async Task<IEnumerable<Appointment>> GetByPatientAsync(Guid patientId)
    {
        var query = ApplyOrganizationFilter(_dbSet);
        return await query
            .Where(a => a.PatientId == patientId)
            .Include(a => a.Practitioner)
            .Include(a => a.Location)
            .OrderByDescending(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByPractitionerAsync(Guid practitionerId)
    {
        var query = ApplyOrganizationFilter(_dbSet);
        return await query
            .Where(a => a.PractitionerId == practitionerId)
            .Include(a => a.Patient)
            .Include(a => a.Location)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var query = ApplyOrganizationFilter(_dbSet);
        return await query
            .Where(a => a.StartTime >= startDate && a.StartTime <= endDate)
            .Include(a => a.Patient)
            .Include(a => a.Practitioner)
            .Include(a => a.Location)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByStatusAsync(string status)
    {
        var query = ApplyOrganizationFilter(_dbSet);
        return await query
            .Where(a => a.Status == status)
            .Include(a => a.Patient)
            .Include(a => a.Practitioner)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetUpcomingByPatientAsync(Guid patientId)
    {
        var now = DateTime.UtcNow;
        var query = ApplyOrganizationFilter(_dbSet);
        return await query
            .Where(a => a.PatientId == patientId && a.StartTime > now)
            .Include(a => a.Practitioner)
            .Include(a => a.Location)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<bool> HasConflictAsync(
        Guid practitionerId, 
        DateTime startTime, 
        DateTime endTime, 
        Guid? excludeAppointmentId = null)
    {
        var query = ApplyOrganizationFilter(_dbSet)
            .Where(a => a.PractitionerId == practitionerId &&
                       a.Status != "Cancelled" &&
                       ((a.StartTime >= startTime && a.StartTime < endTime) ||
                        (a.EndTime > startTime && a.EndTime <= endTime) ||
                        (a.StartTime <= startTime && a.EndTime >= endTime)));

        if (excludeAppointmentId.HasValue)
        {
            query = query.Where(a => a.Id != excludeAppointmentId.Value);
        }

        return await query.AnyAsync();
    }
}
