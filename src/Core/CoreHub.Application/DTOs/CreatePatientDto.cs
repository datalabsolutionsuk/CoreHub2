using System;
using System.ComponentModel.DataAnnotations;

namespace CoreHub.Application.DTOs;

/// <summary>
/// DTO for creating a new patient
/// </summary>
public class CreatePatientDto
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Middle name cannot exceed 100 characters")]
    public string? MiddleName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(50)]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number")]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Phone(ErrorMessage = "Invalid mobile phone number")]
    [StringLength(20)]
    public string? MobilePhone { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? State { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [StringLength(200)]
    public string? EmergencyContactName { get; set; }

    [Phone]
    [StringLength(20)]
    public string? EmergencyContactPhone { get; set; }

    [StringLength(100)]
    public string? EmergencyContactRelationship { get; set; }

    [StringLength(10)]
    public string? BloodType { get; set; }

    public string? Allergies { get; set; }
    public string? MedicalHistory { get; set; }
    public string? CurrentMedications { get; set; }

    [StringLength(200)]
    public string? InsuranceProvider { get; set; }

    [StringLength(100)]
    public string? InsurancePolicyNumber { get; set; }

    public DateTime? InsuranceExpiryDate { get; set; }
    public Guid? PrimaryPractitionerId { get; set; }

    [StringLength(200)]
    public string? ReferralSource { get; set; }

    public string? Notes { get; set; }
    public string? Tags { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = "Active";
}
