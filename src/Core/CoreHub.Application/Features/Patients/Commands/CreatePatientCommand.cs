using System;
using System.Threading;
using System.Threading.Tasks;
using CoreHub.Application.Interfaces;
using CoreHub.Domain.Entities;
using MediatR;

namespace CoreHub.Application.Features.Patients.Commands;

/// <summary>
/// Command to create a new patient
/// </summary>
public record CreatePatientCommand : IRequest<Guid>
{
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
/// Handler for CreatePatientCommand
/// </summary>
public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Guid>
{
    private readonly IPatientRepository _patientRepository;

    public CreatePatientCommandHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
    }

    public async Task<Guid> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        var existingPatient = await _patientRepository.GetByEmailAsync(request.Email);
        if (existingPatient != null)
        {
            throw new InvalidOperationException($"Patient with email {request.Email} already exists.");
        }

        // Create new patient
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Email = request.Email,
            Phone = request.Phone,
            MobilePhone = request.MobilePhone,
            Address = request.Address,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country,
            EmergencyContactName = request.EmergencyContactName,
            EmergencyContactPhone = request.EmergencyContactPhone,
            EmergencyContactRelationship = request.EmergencyContactRelationship,
            BloodType = request.BloodType,
            Allergies = request.Allergies,
            MedicalHistory = request.MedicalHistory,
            CurrentMedications = request.CurrentMedications,
            InsuranceProvider = request.InsuranceProvider,
            InsurancePolicyNumber = request.InsurancePolicyNumber,
            InsuranceExpiryDate = request.InsuranceExpiryDate,
            PrimaryPractitionerId = request.PrimaryPractitionerId,
            ReferralSource = request.ReferralSource,
            Notes = request.Notes,
            Tags = request.Tags,
            Status = request.Status
        };

        await _patientRepository.AddAsync(patient);
        await _patientRepository.SaveChangesAsync();

        return patient.Id;
    }
}
