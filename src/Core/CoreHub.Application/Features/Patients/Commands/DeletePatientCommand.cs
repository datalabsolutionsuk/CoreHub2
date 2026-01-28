using System;
using System.Threading;
using System.Threading.Tasks;
using CoreHub.Application.Interfaces;
using MediatR;

namespace CoreHub.Application.Features.Patients.Commands;

/// <summary>
/// Command to delete a patient
/// </summary>
public record DeletePatientCommand(Guid Id) : IRequest<Unit>;

/// <summary>
/// Handler for DeletePatientCommand
/// </summary>
public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, Unit>
{
    private readonly IPatientRepository _patientRepository;

    public DeletePatientCommandHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
    }

    public async Task<Unit> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(request.Id);
        if (patient == null)
        {
            throw new InvalidOperationException($"Patient with ID {request.Id} not found.");
        }

        await _patientRepository.DeleteAsync(patient);
        await _patientRepository.SaveChangesAsync();

        return Unit.Value;
    }
}
