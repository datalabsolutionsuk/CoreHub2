using AutoMapper;
using CoreHub.Application.DTOs;
using CoreHub.Application.Features.Patients.Commands;
using CoreHub.Domain.Entities;

namespace CoreHub.Application.Mappings;

/// <summary>
/// AutoMapper profile for Patient entity mappings
/// </summary>
public class PatientMappingProfile : Profile
{
    public PatientMappingProfile()
    {
        // Entity to DTO
        CreateMap<Patient, PatientDto>()
            .ForMember(dest => dest.PrimaryPractitionerName,
                opt => opt.MapFrom(src => src.PrimaryPractitioner != null
                    ? $"{src.PrimaryPractitioner.FirstName} {src.PrimaryPractitioner.LastName}"
                    : null));

        // CreateDTO to Command
        CreateMap<CreatePatientDto, CreatePatientCommand>();

        // UpdateDTO to Command
        CreateMap<UpdatePatientDto, UpdatePatientCommand>();

        // Command to Entity (for creation)
        CreateMap<CreatePatientCommand, Patient>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.PrimaryPractitioner, opt => opt.Ignore())
            .ForMember(dest => dest.Appointments, opt => opt.Ignore())
            .ForMember(dest => dest.ClinicalNotes, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices, opt => opt.Ignore())
            .ForMember(dest => dest.Documents, opt => opt.Ignore());
    }
}
