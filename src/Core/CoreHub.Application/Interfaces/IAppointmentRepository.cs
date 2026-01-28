using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreHub.Domain.Entities;

namespace CoreHub.Application.Interfaces;

/// <summary>
/// Repository interface for Appointment entity
/// </summary>
public interface IAppointmentRepository : IRepository<Appointment>
{
    /// <summary>
    /// Gets appointments by patient
    /// </summary>
    Task<IEnumerable<Appointment>> GetByPatientAsync(Guid patientId);

    /// <summary>
    /// Gets appointments by practitioner
    /// </summary>
    Task<IEnumerable<Appointment>> GetByPractitionerAsync(Guid practitionerId);

    /// <summary>
    /// Gets appointments in date range
    /// </summary>
    Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets appointments by status
    /// </summary>
    Task<IEnumerable<Appointment>> GetByStatusAsync(string status);

    /// <summary>
    /// Gets upcoming appointments for a patient
    /// </summary>
    Task<IEnumerable<Appointment>> GetUpcomingByPatientAsync(Guid patientId);

    /// <summary>
    /// Checks for scheduling conflicts
    /// </summary>
    Task<bool> HasConflictAsync(Guid practitionerId, DateTime startTime, DateTime endTime, Guid? excludeAppointmentId = null);
}
