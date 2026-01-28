using System;
using System.Threading;
using System.Threading.Tasks;
using CoreHub.Application.Interfaces;
using CoreHub.Domain.Entities;
using MediatR;

namespace CoreHub.Application.Features.Patients.Queries;

/// <summary>
/// Query to get a single patient by ID
/// </summary>
public record GetPatientQuery(Guid Id) : IRequest<Patient?>;

/// <summary>
/// Handler for GetPatientQuery
/// </summary>
public class GetPatientQueryHandler : IRequestHandler<GetPatientQuery, Patient?>
{
    private readonly IPatientRepository _patientRepository;

    public GetPatientQueryHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
    }

    public async Task<Patient?> Handle(GetPatientQuery request, CancellationToken cancellationToken)
    {
        return await _patientRepository.GetWithAppointmentsAsync(request.Id);
    }
}
