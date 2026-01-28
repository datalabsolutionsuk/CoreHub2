using System;
using System.Threading;
using System.Threading.Tasks;
using CoreHub.Application.Interfaces;
using MediatR;

namespace CoreHub.Application.Features.Patients.Commands;

/// <summary>
/// Command to update an existing patient
/// </summary>
public record UpdatePatientCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? MiddleName { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? Gender { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? MobilePhone { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? PostalCode { get; init; }
    public string? Country { get; init; }
    public string? EmergencyContactName { get; init; }
    public string? EmergencyContactPhone { get; init; }
    public string? EmergencyContactRelationship { get; init; }
    public string? BloodType { get; init; }
    public string? Allergies { get; init; }
    public string? MedicalHistory { get; init; }
    public string? CurrentMedications { get; init; }
    public string? InsuranceProvider { get; init; }
    public string? InsurancePolicyNumber { get; init; }
    public DateTime? InsuranceExpiryDate { get; init; }
    public Guid? PrimaryPractitionerId { get; init; }
    public string? ReferralSource { get; init; }
    public string? Notes { get; init; }
    public string? Tags { get; init; }
    public string Status { get; init; } = "Active";
}

/// <summary>
/// Handler for UpdatePatientCommand
/// </summary>
public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, Unit>
{
    private readonly IPatientRepository _patientRepository;

    public UpdatePatientCommandHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
    }

    public async Task<Unit> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        // Get existing patient
        var patient = await _patientRepository.GetByIdAsync(request.Id);
        if (patient == null)
        {
            throw new InvalidOperationException($"Patient with ID {request.Id} not found.");
        }

        // Check if email is being changed and if it's already in use
        if (patient.Email != request.Email)
        {
            var existingPatient = await _patientRepository.GetByEmailAsync(request.Email);
            if (existingPatient != null && existingPatient.Id != request.Id)
            {
                throw new InvalidOperationException($"Patient with email {request.Email} already exists.");
            }
        }

        // Update patient properties
        patient.FirstName = request.FirstName;
        patient.LastName = request.LastName;
        patient.MiddleName = request.MiddleName;
        patient.DateOfBirth = request.DateOfBirth;
        patient.Gender = request.Gender;
        patient.Email = request.Email;
        patient.Phone = request.Phone;
        patient.MobilePhone = request.MobilePhone;
        patient.Address = request.Address;
        patient.City = request.City;
        patient.State = request.State;
        patient.PostalCode = request.PostalCode;
        patient.Country = request.Country;
        patient.EmergencyContactName = request.EmergencyContactName;
        patient.EmergencyContactPhone = request.EmergencyContactPhone;
        patient.EmergencyContactRelationship = request.EmergencyContactRelationship;
        patient.BloodType = request.BloodType;
        patient.Allergies = request.Allergies;
        patient.MedicalHistory = request.MedicalHistory;
        patient.CurrentMedications = request.CurrentMedications;
        patient.InsuranceProvider = request.InsuranceProvider;
        patient.InsurancePolicyNumber = request.InsurancePolicyNumber;
        patient.InsuranceExpiryDate = request.InsuranceExpiryDate;
        patient.PrimaryPractitionerId = request.PrimaryPractitionerId;
        patient.ReferralSource = request.ReferralSource;
        patient.Notes = request.Notes;
        patient.Tags = request.Tags;
        patient.Status = request.Status;

        await _patientRepository.UpdateAsync(patient);
        await _patientRepository.SaveChangesAsync();

        return Unit.Value;
    }
}
