using Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;
using Fermion.EntityFramework.AuditLogs.Application.DTOs.EntityPropertyChanges;
using Fermion.EntityFramework.AuditLogs.Domain.Interfaces.Services;
using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fermion.EntityFramework.AuditLogs.Presentation.Controllers;

/// <summary>
/// Controller for managing audit logs and related operations.
/// </summary>
[ApiController]
[Route("api/audit-logs")]
public class AuditLogController(IAuditLogAppService auditLogAppService) : ControllerBase
{
    /// <summary>
    /// Retrieves an audit log entry by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the audit log entry.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The audit log entry if found; otherwise, returns NotFound.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AuditLogResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuditLogResponseDto>> GetByIdAsync([FromRoute(Name = "id")] Guid id, CancellationToken cancellationToken = default)
    {
        var auditLog = await auditLogAppService.GetByIdAsync(id, cancellationToken);
        return Ok(auditLog);
    }

    /// <summary>
    /// Retrieves a paginated list of audit log entries based on specified filtering criteria.
    /// </summary>
    /// <param name="request">The request containing pagination and filtering parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of audit log entries.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PageableResponseDto<AuditLogResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PageableResponseDto<AuditLogResponseDto>>> GetPageableAndFilterAsync([FromQuery] GetListAuditLogRequestDto request, CancellationToken cancellationToken = default)
    {
        var result = await auditLogAppService.GetPageableAndFilterAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all property changes associated with a specific audit log entry.
    /// </summary>
    /// <param name="auditLogId">The unique identifier of the audit log entry.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of property changes for the specified audit log entry.</returns>
    [HttpGet("{auditLogId:guid}/property-changes")]
    [ProducesResponseType(typeof(List<EntityPropertyChangeResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<EntityPropertyChangeResponseDto>>> GetEntityPropertyChangesAsync([FromRoute(Name = "auditLogId")] Guid auditLogId, CancellationToken cancellationToken = default)
    {
        var propertyChanges = await auditLogAppService.GetEntityPropertyChangesAsync(auditLogId, cancellationToken);
        return Ok(propertyChanges);
    }

    /// <summary>
    /// Retrieves a summary of changes for a specific entity.
    /// </summary>
    /// <param name="request">The request containing entity identification and filtering criteria.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A summary of changes for the specified entity.</returns>
    [HttpGet("entity-summary")]
    [ProducesResponseType(typeof(EntityChangeSummaryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EntityChangeSummaryResponseDto>> GetEntityChangeSummaryAsync([FromQuery] GetEntityChangeSummaryRequestDto request, CancellationToken cancellationToken = default)
    {
        var summary = await auditLogAppService.GetEntityChangeSummaryAsync(request, cancellationToken);
        return Ok(summary);
    }

    /// <summary>
    /// Analyzes user activity patterns based on audit logs.
    /// </summary>
    /// <param name="request">The request containing analysis parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An analysis of user activity patterns.</returns>
    [HttpGet("user-activity-analysis")]
    [ProducesResponseType(typeof(UserActivityAnalysisResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserActivityAnalysisResponseDto>> GetUserActivityAnalysisAsync([FromQuery] UserActivityAnalysisRequestDto request, CancellationToken cancellationToken = default)
    {
        var analysis = await auditLogAppService.GetUserActivityAnalysisAsync(request, cancellationToken);
        return Ok(analysis);
    }

    /// <summary>
    /// Identifies the most frequently modified entities in the system.
    /// </summary>
    /// <param name="request">The request containing analysis parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of the most frequently modified entities.</returns>
    [HttpGet("most-modified-entities")]
    [ProducesResponseType(typeof(MostModifiedEntitiesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MostModifiedEntitiesResponseDto>> GetMostModifiedEntitiesAsync([FromQuery] MostModifiedEntitiesRequestDto request, CancellationToken cancellationToken = default)
    {
        var result = await auditLogAppService.GetMostModifiedEntitiesAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Analyzes trends in entity changes over time.
    /// </summary>
    /// <param name="request">The request containing analysis parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An analysis of entity change trends.</returns>
    [HttpGet("entity-changes-trend")]
    [ProducesResponseType(typeof(EntityChangesTrendResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EntityChangesTrendResponseDto>> AnalyzeEntityChangesTrendAsync([FromQuery] EntityChangesTrendRequestDto request, CancellationToken cancellationToken = default)
    {
        var analysis = await auditLogAppService.AnalyzeEntityChangesTrendAsync(request, cancellationToken);
        return Ok(analysis);
    }

    /// <summary>
    /// Analyzes patterns in how users make changes to entities.
    /// </summary>
    /// <param name="request">The request containing analysis parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An analysis of user change behavior patterns.</returns>
    [HttpGet("user-change-behavior")]
    [ProducesResponseType(typeof(UserChangeBehaviorResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserChangeBehaviorResponseDto>> AnalyzeUserChangeBehaviorAsync([FromQuery] UserChangeBehaviorRequestDto request, CancellationToken cancellationToken = default)
    {
        var analysis = await auditLogAppService.AnalyzeUserChangeBehaviorAsync(request, cancellationToken);
        return Ok(analysis);
    }

    /// <summary>
    /// Removes audit log entries older than the specified date.
    /// </summary>
    /// <param name="olderThan">The cutoff date for removing old audit logs.</param>
    /// <param name="isArchive">Indicates whether to remove logs from the archive.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of audit log entries that were removed.</returns>
    [HttpDelete("cleanup")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> CleanupOldAuditLogsAsync([FromQuery] DateTime olderThan, [FromQuery] bool isArchive = true, CancellationToken cancellationToken = default)
    {
        var deletedCount = await auditLogAppService.CleanupOldAuditLogsAsync(olderThan, isArchive, cancellationToken);
        return Ok(deletedCount);
    }
}