using System;
using System.Linq;
using System.Threading.Tasks;
using CoreHub.Domain.Entities;
using CoreHub.Infrastructure.Data;
using CoreHub.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CoreHub.Tests.Infrastructure.Repositories;

public class PatientRepositoryTests : TestBase
{
    private readonly PatientRepository _repository;

    public PatientRepositoryTests()
    {
        _repository = new PatientRepository(Context, MockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task AddAsync_ValidPatient_AddsToDatabase()
    {
        // Arrange
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            Status = "Active"
        };

        // Act
        await _repository.AddAsync(patient);
        await _repository.SaveChangesAsync();

        // Assert
        var savedPatient = await Context.Patients.FindAsync(patient.Id);
        savedPatient.Should().NotBeNull();
        savedPatient!.FirstName.Should().Be("Jane");
        savedPatient.LastName.Should().Be("Smith");
        savedPatient.Email.Should().Be("jane.smith@example.com");
        savedPatient.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetByEmailAsync_ExistingEmail_ReturnsPatient()
    {
        // Arrange
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.Patients.Add(patient);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("test@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByEmailAsync_NonExistingEmail_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nonexisting@example.com");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SearchByNameAsync_ValidSearchTerm_ReturnsMatchingPatients()
    {
        // Arrange
        var patients = new[]
        {
            new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice@example.com",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = "Bob",
                LastName = "Johnson",
                Email = "bob@example.com",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = "Charlie",
                LastName = "Smith",
                Email = "charlie@example.com",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        Context.Patients.AddRange(patients);
        await Context.SaveChangesAsync();

        // Act
        var results = await _repository.SearchByNameAsync("Johnson");

        // Assert
        results.Should().HaveCount(2);
        results.Should().Contain(p => p.FirstName == "Alice");
        results.Should().Contain(p => p.FirstName == "Bob");
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsCorrectPagedResults()
    {
        // Arrange
        var patients = Enumerable.Range(1, 25).Select(i => new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = $"Patient{i}",
            LastName = $"Test{i}",
            Email = $"patient{i}@example.com",
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToArray();

        Context.Patients.AddRange(patients);
        await Context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _repository.GetPagedAsync(2, 10);

        // Assert
        totalCount.Should().Be(25);
        items.Should().HaveCount(10);
    }

    [Fact]
    public async Task UpdateAsync_ExistingPatient_UpdatesDatabase()
    {
        // Arrange
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "Original",
            LastName = "Name",
            Email = "original@example.com",
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.Patients.Add(patient);
        await Context.SaveChangesAsync();

        // Act
        patient.FirstName = "Updated";
        patient.Email = "updated@example.com";
        await _repository.UpdateAsync(patient);
        await _repository.SaveChangesAsync();

        // Assert
        var updatedPatient = await Context.Patients.FindAsync(patient.Id);
        updatedPatient!.FirstName.Should().Be("Updated");
        updatedPatient.Email.Should().Be("updated@example.com");
        updatedPatient.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task DeleteAsync_ExistingPatient_RemovesFromDatabase()
    {
        // Arrange
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "ToDelete",
            LastName = "Patient",
            Email = "delete@example.com",
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.Patients.Add(patient);
        await Context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(patient.Id);
        await _repository.SaveChangesAsync();

        // Assert
        var deletedPatient = await Context.Patients.FindAsync(patient.Id);
        deletedPatient.Should().BeNull();
    }
}
