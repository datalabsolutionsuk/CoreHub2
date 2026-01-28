using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreHub.Application.Features.Patients.Queries;
using CoreHub.Application.Interfaces;
using CoreHub.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CoreHub.Tests.Features.Patients.Queries;

public class GetPatientQueryHandlerTests
{
    private readonly Mock<IPatientRepository> _mockRepository;
    private readonly GetPatientQueryHandler _handler;

    public GetPatientQueryHandlerTests()
    {
        _mockRepository = new Mock<IPatientRepository>();
        _handler = new GetPatientQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingPatientId_ReturnsPatientWithAppointments()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var expectedPatient = new Patient
        {
            Id = patientId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "555-1234",
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            Appointments = new List<Appointment>
            {
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    AppointmentType = "Consultation",
                    StartTime = DateTime.UtcNow.AddDays(1),
                    EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
                    Status = "Scheduled"
                }
            }
        };

        _mockRepository.Setup(r => r.GetWithAppointmentsAsync(patientId))
            .ReturnsAsync(expectedPatient);

        var query = new GetPatientQuery(patientId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(patientId);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("john.doe@example.com");
        result.Appointments.Should().HaveCount(1);
        _mockRepository.Verify(r => r.GetWithAppointmentsAsync(patientId), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingPatientId_ReturnsNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        _mockRepository.Setup(r => r.GetWithAppointmentsAsync(nonExistingId))
            .ReturnsAsync((Patient?)null);

        var query = new GetPatientQuery(nonExistingId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetWithAppointmentsAsync(nonExistingId), Times.Once);
    }

    [Fact]
    public async Task Handle_PatientWithNoAppointments_ReturnsPatientWithEmptyAppointments()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var expectedPatient = new Patient
        {
            Id = patientId,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            Appointments = new List<Appointment>()
        };

        _mockRepository.Setup(r => r.GetWithAppointmentsAsync(patientId))
            .ReturnsAsync(expectedPatient);

        var query = new GetPatientQuery(patientId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Appointments.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_PatientWithPrimaryPractitioner_ReturnsPatientWithPractitioner()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var practitionerId = Guid.NewGuid();
        var expectedPatient = new Patient
        {
            Id = patientId,
            FirstName = "Bob",
            LastName = "Wilson",
            Email = "bob.wilson@example.com",
            Status = "Active",
            PrimaryPractitionerId = practitionerId,
            PrimaryPractitioner = new Practitioner
            {
                Id = practitionerId,
                FirstName = "Dr. Sarah",
                LastName = "Johnson",
                Email = "dr.sarah@clinic.com",
                Specialty = "General Practice"
            },
            CreatedAt = DateTime.UtcNow,
            Appointments = new List<Appointment>()
        };

        _mockRepository.Setup(r => r.GetWithAppointmentsAsync(patientId))
            .ReturnsAsync(expectedPatient);

        var query = new GetPatientQuery(patientId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.PrimaryPractitioner.Should().NotBeNull();
        result.PrimaryPractitioner!.FirstName.Should().Be("Dr. Sarah");
        result.PrimaryPractitioner.Specialty.Should().Be("General Practice");
    }

    [Fact]
    public void Constructor_NullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GetPatientQueryHandler(null!));
    }

    [Fact]
    public async Task Handle_MultipleAppointments_ReturnsAllAppointments()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var expectedPatient = new Patient
        {
            Id = patientId,
            FirstName = "Alice",
            LastName = "Brown",
            Email = "alice.brown@example.com",
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            Appointments = new List<Appointment>
            {
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    AppointmentType = "Consultation",
                    StartTime = DateTime.UtcNow.AddDays(1),
                    Status = "Scheduled"
                },
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    AppointmentType = "Follow-up",
                    StartTime = DateTime.UtcNow.AddDays(7),
                    Status = "Scheduled"
                },
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    AppointmentType = "Review",
                    StartTime = DateTime.UtcNow.AddDays(-7),
                    Status = "Completed"
                }
            }
        };

        _mockRepository.Setup(r => r.GetWithAppointmentsAsync(patientId))
            .ReturnsAsync(expectedPatient);

        var query = new GetPatientQuery(patientId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Appointments.Should().HaveCount(3);
        result.Appointments.Should().Contain(a => a.AppointmentType == "Consultation");
        result.Appointments.Should().Contain(a => a.AppointmentType == "Follow-up");
        result.Appointments.Should().Contain(a => a.AppointmentType == "Review");
    }
}
