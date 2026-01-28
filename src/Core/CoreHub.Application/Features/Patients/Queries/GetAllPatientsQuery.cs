using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreHub.Application.Interfaces;
using CoreHub.Domain.Entities;
using MediatR;

namespace CoreHub.Application.Features.Patients.Queries;

/// <summary>
/// Query to get all patients with pagination and filtering
/// </summary>
public record GetAllPatientsQuery : IRequest<(IEnumerable<Patient> Items, int TotalCount, int PageNumber, int PageSize)>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? Status { get; init; }
}

/// <summary>
/// Handler for GetAllPatientsQuery
/// </summary>
public class GetAllPatientsQueryHandler : IRequestHandler<GetAllPatientsQuery, (IEnumerable<Patient> Items, int TotalCount, int PageNumber, int PageSize)>
{
    private readonly IPatientRepository _patientRepository;

    public GetAllPatientsQueryHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
    }

    public async Task<(IEnumerable<Patient> Items, int TotalCount, int PageNumber, int PageSize)> Handle(
        GetAllPatientsQuery request, 
        CancellationToken cancellationToken)
    {
        // Validate pagination parameters
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : (request.PageSize > 100 ? 100 : request.PageSize);

        var (items, totalCount) = await _patientRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            request.SearchTerm,
            request.Status
        );

        return (items, totalCount, pageNumber, pageSize);
    }
}
