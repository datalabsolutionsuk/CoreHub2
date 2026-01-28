using System;
using System.Threading;
using System.Threading.Tasks;
using CoreHub.Application.Features.Patients.Commands;
using CoreHub.Application.Interfaces;
using CoreHub.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CoreHub.Tests.Features.Patients.Commands;

public class CreatePatientCommandHandlerTests
{
    private readonly Mock<IPatientRepository> _mockRepository;
    private readonly CreatePatientCommandHandler _handler;

    public CreatePatientCommandHandlerTests()
    {
        _mockRepository = new Mock<IPatientRepository>();
        _handler = new CreatePatientCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesPatientAndReturnsId()
    {
        // Arrange
        var command = new CreatePatientCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "555-1234",
            Status = "Active"
        };

        _mockRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Patient?)null);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Patient>()))
            .ReturnsAsync((Patient p) => p);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        _mockRepository.Verify(r => r.AddAsync(It.Is<Patient>(p =>
            p.FirstName == "John" &&
            p.LastName == "Doe" &&
            p.Email == "john.doe@example.com"
        )), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new CreatePatientCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "existing@example.com"
        };

        var existingPatient = new Patient
        {
            Id = Guid.NewGuid(),
            Email = "existing@example.com"
        };

        _mockRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingPatient);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public void Handle_NullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CreatePatientCommandHandler(null!));
    }
}
