using Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;
using Fermion.EntityFramework.AuditLogs.Application.DTOs.EntityPropertyChanges;
using Fermion.EntityFramework.Shared.DTOs.Pagination;

namespace Fermion.EntityFramework.AuditLogs.Domain.Interfaces.Services;

/// <summary>
/// Application service interface for managing audit logs.
/// </summary>
public interface IAuditLogAppService
{
    /// <summary>
    /// Retrieves an audit log entry by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the audit log entry.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The audit log entry if found; otherwise, entity not found exception.</returns>
    Task<AuditLogResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of audit log entries based on specified filtering criteria.
    /// </summary>
    /// <param name="request">The request containing pagination and filtering parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of audit log entries.</returns>
    Task<PageableResponseDto<AuditLogResponseDto>> GetPageableAndFilterAsync(GetListAuditLogRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a summary of changes for a specific entity.
    /// </summary>
    /// <param name="request">The request containing entity identification and filtering criteria.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A summary of changes for the specified entity.</returns>
    Task<EntityChangeSummaryResponseDto> GetEntityChangeSummaryAsync(GetEntityChangeSummaryRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all property changes associated with a specific audit log entry.
    /// </summary>
    /// <param name="auditLogId">The unique identifier of the audit log entry.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of property changes for the specified audit log entry.</returns>
    Task<List<EntityPropertyChangeResponseDto>> GetEntityPropertyChangesAsync(Guid auditLogId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes audit log entries older than the specified date.
    /// </summary>
    /// <param name="olderThan">The cutoff date for removing old audit logs.</param>
    /// <param name="isArchive">Indicates whether to archive the logs before deletion.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of audit log entries that were removed.</returns>
    Task<int> CleanupOldAuditLogsAsync(DateTime olderThan, bool isArchive = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes user activity patterns based on audit logs.
    /// </summary>
    /// <param name="request">The request containing analysis parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An analysis of user activity patterns.</returns>
    Task<UserActivityAnalysisResponseDto> GetUserActivityAnalysisAsync(UserActivityAnalysisRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Identifies the most frequently modified entities in the system.
    /// </summary>
    /// <param name="request">The request containing analysis parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of the most frequently modified entities.</returns>
    Task<MostModifiedEntitiesResponseDto> GetMostModifiedEntitiesAsync(MostModifiedEntitiesRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes trends in entity changes over time.
    /// </summary>
    /// <param name="request">The request containing analysis parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An analysis of entity change trends.</returns>
    Task<EntityChangesTrendResponseDto> AnalyzeEntityChangesTrendAsync(EntityChangesTrendRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes patterns in how users make changes to entities.
    /// </summary>
    /// <param name="request">The request containing analysis parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An analysis of user change behavior patterns.</returns>
    Task<UserChangeBehaviorResponseDto> AnalyzeUserChangeBehaviorAsync(UserChangeBehaviorRequestDto request, CancellationToken cancellationToken = default);
}