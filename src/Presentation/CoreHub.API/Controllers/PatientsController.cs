using AutoMapper;
using CoreHub.Application.DTOs;
using CoreHub.Application.Features.Patients.Commands;
using CoreHub.Application.Features.Patients.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreHub.API.Controllers;

/// <summary>
/// Patients API controller for managing patient records
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Policy = "RequireOrganizationAccess")]
public class PatientsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(
        IMediator mediator,
        IMapper mapper,
        ILogger<PatientsController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a paginated list of patients with optional search and filtering
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for name, email, or phone</param>
    /// <param name="status">Filter by status (Active, Inactive, Archived)</param>
    /// <returns>Paginated list of patients</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<PatientDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedResult<PatientDto>>> GetPatients(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? status = null)
    {
        try
        {
            var query = new GetAllPatientsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                Status = status
            };

            var (items, totalCount, actualPageNumber, actualPageSize) = await _mediator.Send(query);
            var patientDtos = _mapper.Map<IEnumerable<PatientDto>>(items);

            var result = new PaginatedResult<PatientDto>(patientDtos, actualPageNumber, actualPageSize, totalCount);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patients");
            return BadRequest(new { error = "Failed to retrieve patients", details = ex.Message });
        }
    }

    /// <summary>
    /// Gets a specific patient by ID
    /// </summary>
    /// <param name="id">Patient ID</param>
    /// <returns>Patient details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientDto>> GetPatient(Guid id)
    {
        try
        {
            var query = new GetPatientQuery(id);
            var patient = await _mediator.Send(query);

            if (patient == null)
            {
                return NotFound(new { error = $"Patient with ID {id} not found" });
            }

            var patientDto = _mapper.Map<PatientDto>(patient);
            return Ok(patientDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient {PatientId}", id);
            return BadRequest(new { error = "Failed to retrieve patient", details = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new patient
    /// </summary>
    /// <param name="createPatientDto">Patient data</param>
    /// <returns>Created patient ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreatePatient([FromBody] CreatePatientDto createPatientDto)
    {
        try
        {
            var command = _mapper.Map<CreatePatientCommand>(createPatientDto);
            var patientId = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetPatient),
                new { id = patientId },
                new { id = patientId });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating patient");
            return BadRequest(new { error = ex.Message });
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed while creating patient");
            var errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage });
            return BadRequest(new { error = "Validation failed", validationErrors = errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating patient");
            return BadRequest(new { error = "Failed to create patient", details = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing patient
    /// </summary>
    /// <param name="id">Patient ID</param>
    /// <param name="updatePatientDto">Updated patient data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePatient(Guid id, [FromBody] UpdatePatientDto updatePatientDto)
    {
        try
        {
            if (id != updatePatientDto.Id)
            {
                return BadRequest(new { error = "ID in URL does not match ID in request body" });
            }

            var command = _mapper.Map<UpdatePatientCommand>(updatePatientDto);
            await _mediator.Send(command);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while updating patient {PatientId}", id);
            
            if (ex.Message.Contains("not found"))
                return NotFound(new { error = ex.Message });
            
            return BadRequest(new { error = ex.Message });
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed while updating patient {PatientId}", id);
            var errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage });
            return BadRequest(new { error = "Validation failed", validationErrors = errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating patient {PatientId}", id);
            return BadRequest(new { error = "Failed to update patient", details = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a patient
    /// </summary>
    /// <param name="id">Patient ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeletePatient(Guid id)
    {
        try
        {
            var command = new DeletePatientCommand(id);
            await _mediator.Send(command);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while deleting patient {PatientId}", id);
            
            if (ex.Message.Contains("not found"))
                return NotFound(new { error = ex.Message });
            
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting patient {PatientId}", id);
            return BadRequest(new { error = "Failed to delete patient", details = ex.Message });
        }
    }
}
