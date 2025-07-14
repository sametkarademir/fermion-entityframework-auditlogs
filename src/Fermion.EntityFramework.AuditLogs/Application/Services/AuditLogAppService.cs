using AutoMapper;
using Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;
using Fermion.EntityFramework.AuditLogs.Application.DTOs.EntityPropertyChanges;
using Fermion.EntityFramework.AuditLogs.Domain.Enums;
using Fermion.EntityFramework.AuditLogs.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.AuditLogs.Domain.Interfaces.Services;
using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Fermion.EntityFramework.Shared.Extensions;
using Fermion.Domain.Extensions.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fermion.EntityFramework.AuditLogs.Application.Services;

/// <summary>
/// Application service for managing audit logs and related operations.
/// </summary>
public class AuditLogAppService(
    IAuditLogRepository auditLogRepository,
    IEntityPropertyChangeRepository entityPropertyChangeRepository,
    IMapper mapper,
    ILogger<AuditLogAppService> logger)
    : IAuditLogAppService
{
    /// <summary>
    /// Retrieves an audit log entry by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the audit log entry.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The audit log entry if found; otherwise, entity not found exception.</returns>
    public async Task<AuditLogResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var matchedAuditLog = await auditLogRepository.GetAsync(
            id: id,
            include: item => item.Include(a => a.EntityPropertyChanges),
            enableTracking: false,
            cancellationToken: cancellationToken
        );

        return mapper.Map<AuditLogResponseDto>(matchedAuditLog);
    }

    /// <summary>
    /// Retrieves a paginated list of audit log entries based on specified filtering criteria.
    /// </summary>
    /// <param name="request">The request containing pagination and filtering parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of audit log entries.</returns>
    public async Task<PageableResponseDto<AuditLogResponseDto>> GetPageableAndFilterAsync(GetListAuditLogRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = auditLogRepository.GetQueryable();
        queryable = queryable.Include(item => item.EntityPropertyChanges);
        queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(request.EntityId), item => item.EntityId == request.EntityId);
        queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(request.EntityName), item => item.EntityName == request.EntityName);
        queryable = queryable.WhereIf(request.States.HasValue, item => item.State == request.States);
        queryable = queryable.WhereIf(request.StartDate.HasValue, item => item.CreationTime >= request.StartDate);
        queryable = queryable.WhereIf(request.EndDate.HasValue, item => item.CreationTime <= request.EndDate);
        queryable = queryable.WhereIf(request.UserId.HasValue, item => item.CreatorId == request.UserId);
        queryable = queryable.WhereIf(request.SnapshotId.HasValue, item => item.SnapshotId == request.SnapshotId);
        queryable = queryable.WhereIf(request.SessionId.HasValue, item => item.SessionId == request.SessionId);
        queryable = queryable.WhereIf(request.CorrelationId.HasValue, item => item.CorrelationId == request.CorrelationId);
        queryable = queryable.ApplySort(request.Field, request.Order, cancellationToken);

        queryable = queryable.AsNoTracking();
        var result = await queryable.ToPageableAsync(request.Page, request.PerPage, cancellationToken: cancellationToken);
        var mappedAuditLogs = mapper.Map<List<AuditLogResponseDto>>(result.Data);

        return new PageableResponseDto<AuditLogResponseDto>(mappedAuditLogs, result.Meta);
    }

    /// <summary>
    /// Retrieves a summary of changes for a specific entity.
    /// </summary>
    /// <param name="request">The request containing entity identification and filtering criteria.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A summary of changes for the specified entity.</returns>
    /// <remarks>
    /// This method aggregates changes made to a specific entity within a given date range,
    /// including the total number of changes and the most frequently modified properties.
    /// Take count of the top 10 most frequently modified properties.
    /// </remarks>
    public async Task<EntityChangeSummaryResponseDto> GetEntityChangeSummaryAsync(GetEntityChangeSummaryRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = auditLogRepository.GetQueryable();
        queryable = queryable.Include(item => item.EntityPropertyChanges);
        queryable = queryable.Where(item => item.EntityId == request.EntityId && item.EntityName == request.EntityName);
        queryable = queryable.Where(item => item.CreationTime >= request.StartDate && item.CreationTime <= request.EndDate);
        queryable = queryable.OrderBy(item => item.CreationTime);
        queryable = queryable.AsNoTracking();
        var matchedAuditLogs = await queryable.ToListAsync(cancellationToken: cancellationToken);
        if (matchedAuditLogs.Count == 0)
        {
            return new EntityChangeSummaryResponseDto
            {
                EntityId = request.EntityId,
                EntityName = request.EntityName,
                TotalChanges = 0,
                MostFrequentlyModifiedProperties = []
            };
        }

        var allPropertyChanges = matchedAuditLogs
            .SelectMany(a => a.EntityPropertyChanges)
            .ToList();

        var propertyChangeFrequency = allPropertyChanges
            .GroupBy(p => new { p.PropertyName, p.PropertyTypeFullName })
            .Select(g => new PropertyChangeFrequencyResponseDto
            {
                PropertyName = g.Key.PropertyName,
                PropertyType = g.Key.PropertyTypeFullName,
                ChangeCount = g.Count()
            })
            .OrderByDescending(p => p.ChangeCount)
            .Take(10)
            .ToList();

        var firstLog = matchedAuditLogs[0];
        var lastLog = matchedAuditLogs[^1];

        return new EntityChangeSummaryResponseDto
        {
            EntityId = request.EntityId,
            EntityName = request.EntityName,
            CreationDate = firstLog.CreationTime,
            CreatorId = firstLog.CreatorId,
            LastModificationDate = lastLog.State != States.Deleted ? lastLog.CreationTime : null,
            LastModifierId = lastLog.State != States.Deleted ? lastLog.CreatorId : null,
            TotalChanges = matchedAuditLogs.Count,
            MostFrequentlyModifiedProperties = propertyChangeFrequency
        };
    }

    /// <summary>
    /// Retrieves all property changes associated with a specific audit log entry.
    /// </summary>
    /// <param name="auditLogId">The unique identifier of the audit log entry.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of property changes for the specified audit log entry.</returns>
    public async Task<List<EntityPropertyChangeResponseDto>> GetEntityPropertyChangesAsync(Guid auditLogId, CancellationToken cancellationToken = default)
    {
        var matchedEntityPropertyChanges = await entityPropertyChangeRepository
            .GetAllAsync(
                item => item.AuditLogId == auditLogId,
                orderBy: item => item.OrderBy(epc => epc.PropertyName),
                enableTracking: false,
                cancellationToken: cancellationToken
            );
        var mappedEntityPropertyChanges = mapper.Map<List<EntityPropertyChangeResponseDto>>(matchedEntityPropertyChanges);

        return mappedEntityPropertyChanges;
    }

    /// <summary>
    /// Removes audit log entries older than the specified date.
    /// </summary>
    /// <param name="olderThan">The cutoff date for removing old audit logs.</param>
    /// <param name="isArchive">Indicates whether to archive the logs instead of deleting them.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of audit log entries that were removed.</returns>
    public async Task<int> CleanupOldAuditLogsAsync(DateTime olderThan, bool isArchive = true, CancellationToken cancellationToken = default)
    {
        var queryable = auditLogRepository.GetQueryable();
        queryable = queryable.Where(a => a.CreationTime < olderThan);
        var countToDelete = await queryable.CountAsync(cancellationToken);
        if (countToDelete == 0)
        {
            return 0;
        }

        const int batchSize = 100;
        var totalDeleted = 0;
        while (countToDelete > totalDeleted)
        {
            try
            {
                logger.LogInformation(
                    "[CleanupOldAuditLogsAsync] [Action=DeleteRangeAsync()] [Count={Count}] [Start]",
                    countToDelete - totalDeleted
                );
                var auditLogsToDelete = await queryable
                    .OrderBy(a => a.CreationTime)
                    .Take(batchSize)
                    .ToListAsync(cancellationToken);

                if (auditLogsToDelete.Count == 0)
                {
                    logger.LogInformation(
                        "[CleanupOldAuditLogsAsync] [Action=DeleteRangeAsync()] [Count={Count}] [NoMoreLogsToDelete]",
                        totalDeleted
                    );

                    break;
                }

                await auditLogRepository.DeleteRangeAsync(auditLogsToDelete, permanent: !isArchive, cancellationToken: cancellationToken);
                await auditLogRepository.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "[CleanupOldAuditLogsAsync] [Action=DeleteRangeAsync()] [Count={Count}] [End]",
                    auditLogsToDelete.Count
                );

                totalDeleted += auditLogsToDelete.Count;

                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation(
                        "[CleanupOldAuditLogsAsync] [Action=DeleteRangeAsync()] [Cancelled] [TotalDeleted={TotalDeleted}]",
                        totalDeleted
                    );

                    break;
                }

                if (totalDeleted > 0 && totalDeleted % (batchSize * 5) == 0)
                {
                    await Task.Delay(500, cancellationToken);
                }
            }
            catch (Exception e)
            {
                logger.LogError(
                    e,
                    "[CleanupOldAuditLogsAsync] [Action=DeleteRangeAsync()] [Error] [Exception={Exception}]",
                    e.Message
                );

                break;
            }
        }

        return totalDeleted;
    }

    /// <summary>
    /// Analyzes user activity patterns based on audit logs.
    /// </summary>
    /// <param name="request">The request containing analysis parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An analysis of user activity patterns.</returns>
    public async Task<UserActivityAnalysisResponseDto> GetUserActivityAnalysisAsync(UserActivityAnalysisRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = auditLogRepository.GetQueryable();
        queryable = queryable.Include(item => item.EntityPropertyChanges);
        queryable = queryable.Where(item => item.CreationTime >= request.StartDate && item.CreationTime <= request.EndDate);
        queryable = queryable.WhereIf(request.UserId.HasValue, item => item.CreatorId == request.UserId);
        queryable = queryable.WhereIf(!string.IsNullOrEmpty(request.EntityName), item => item.EntityName == request.EntityName);
        queryable = queryable.AsNoTracking();
        var auditLogs = await queryable.ToListAsync(cancellationToken);
        if (auditLogs.Count == 0)
        {
            return new UserActivityAnalysisResponseDto
            {
                TotalActiveUsers = 0,
                TotalChangeCount = 0,
                MostActiveUsers = [],
                AverageChangesPerUser = 0,
                ActivityDistribution = new Dictionary<string, int>()
            };
        }

        var userActivities = auditLogs
            .Where(a => a.CreatorId.HasValue)
            .GroupBy(a => a.CreatorId!.Value)
            .Select(g => new
            {
                UserId = g.Key,
                ChangeCount = g.Count(),
                LastActivityTime = g.Max(a => a.CreationTime),
                EntityTypeChanges = g.GroupBy(a => a.EntityName)
                    .ToDictionary(g2 => g2.Key, g2 => g2.Count()),
                OperationTypes = g.GroupBy(a => a.State)
                    .ToDictionary(g2 => g2.Key, g2 => g2.Count())
            })
            .ToList();

        if (request.MinActivityCount.HasValue)
        {
            userActivities = userActivities.Where(u => u.ChangeCount >= request.MinActivityCount.Value).ToList();
        }

        var mostActiveUsers = userActivities
            .OrderByDescending(u => u.ChangeCount)
            .Take(10)
            .Select(u => new UserActivityResponseDto
            {
                UserId = u.UserId,
                ChangeCount = u.ChangeCount,
                LastActivityTime = u.LastActivityTime,
                MostModifiedEntityTypes = u.EntityTypeChanges,
                MostCommonOperations = u.OperationTypes
            })
            .ToList();

        var activityDistribution = auditLogs
            .GroupBy(a => a.CreationTime.Date)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key.ToString("yyyy-MM-dd"), g => g.Count());

        var averageChangesPerUser = userActivities.Count > 0
            ? (double)auditLogs.Count / userActivities.Count
            : 0;

        return new UserActivityAnalysisResponseDto
        {
            TotalActiveUsers = userActivities.Count,
            TotalChangeCount = auditLogs.Count,
            MostActiveUsers = mostActiveUsers,
            AverageChangesPerUser = Math.Round(averageChangesPerUser, 2),
            ActivityDistribution = activityDistribution
        };
    }

    /// <summary>
    /// Identifies the most frequently modified entities in the system.
    /// </summary>
    /// <param name="request">The request containing analysis parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of the most frequently modified entities.</returns>
    public async Task<MostModifiedEntitiesResponseDto> GetMostModifiedEntitiesAsync(MostModifiedEntitiesRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = auditLogRepository.GetQueryable();
        queryable = queryable.Include(item => item.EntityPropertyChanges);
        queryable = queryable.Where(item => item.CreationTime >= request.StartDate && item.CreationTime <= request.EndDate);
        queryable = queryable.WhereIf(request.UserId.HasValue, item => item.CreatorId == request.UserId);
        queryable = queryable.WhereIf(request.EntityNames is { Count: > 0 }, item => request.EntityNames!.Contains(item.EntityName));
        queryable = queryable.AsNoTracking();
        var auditLogs = await queryable.ToListAsync(cancellationToken);
        if (auditLogs.Count == 0)
        {
            return new MostModifiedEntitiesResponseDto
            {
                AnalysisPeriod = new DateRangeResponseDto
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                },
                MostModifiedEntities = new List<ModifiedEntityResponseDto>(),
                ChangesByEntityType = new Dictionary<string, int>()
            };
        }

        var changesByEntityType = auditLogs
            .GroupBy(a => a.EntityName)
            .ToDictionary(g => g.Key, g => g.Count());

        var entitiesWithChanges = auditLogs
            .GroupBy(a => new { a.EntityId, a.EntityName })
            .Select(g => new
            {
                EntityId = g.Key.EntityId,
                EntityName = g.Key.EntityName,
                ChangeCount = g.Count(),
                LastModified = g.Max(a => a.CreationTime),
                UniqueUserCount = g.Where(a => a.CreatorId.HasValue)
                    .Select(a => a.CreatorId)
                    .Distinct()
                    .Count(),
                StateDistribution = g.GroupBy(a => a.State)
                    .ToDictionary(g2 => g2.Key, g2 => g2.Count())
            })
            .OrderByDescending(e => e.ChangeCount)
            .Take(10)
            .ToList();

        var mostModifiedEntities = entitiesWithChanges
            .Select(e => new ModifiedEntityResponseDto
            {
                EntityId = e.EntityId,
                EntityName = e.EntityName,
                ChangeCount = e.ChangeCount,
                LastModified = e.LastModified,
                UniqueUserCount = e.UniqueUserCount,
                StateDistribution = e.StateDistribution
            })
            .ToList();

        return new MostModifiedEntitiesResponseDto
        {
            AnalysisPeriod = new DateRangeResponseDto
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate
            },
            MostModifiedEntities = mostModifiedEntities,
            ChangesByEntityType = changesByEntityType
        };
    }

    /// <summary>
    /// Analyzes trends in entity changes over time.
    /// </summary>
    /// <param name="request">The request containing analysis parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An analysis of entity change trends.</returns>
    public async Task<EntityChangesTrendResponseDto> AnalyzeEntityChangesTrendAsync(EntityChangesTrendRequestDto request, CancellationToken cancellationToken = default)
    {
        var startDateNormalized = request.StartDate.Date;
        var endDateNormalized = request.EndDate.Date.AddDays(1).AddTicks(-1);

        var queryable = auditLogRepository.GetQueryable();
        queryable = queryable.Include(a => a.EntityPropertyChanges);
        queryable = queryable.Where(a => a.EntityName == request.EntityName);
        queryable = queryable.Where(a =>
            a.CreationTime.Date >= startDateNormalized &&
            a.CreationTime.Date <= endDateNormalized.Date
        );
        queryable = queryable.AsNoTracking();
        var auditLogs = await queryable.ToListAsync(cancellationToken);
        if (auditLogs.Count == 0)
        {
            return new EntityChangesTrendResponseDto
            {
                EntityName = request.EntityName,
                AnalysisPeriod = new DateRangeResponseDto
                {
                    StartDate = startDateNormalized,
                    EndDate = endDateNormalized
                },
                ChangesByTimeInterval = new List<TimeSeriesDataPointResponseDto>(),
                ChangesByProperty = new Dictionary<string, List<TimeSeriesDataPointResponseDto>>(),
                ChangesByState = new Dictionary<States, List<TimeSeriesDataPointResponseDto>>()
            };
        }

        Func<DateTime, DateTime> getTimeInterval;
        string timeFormat;

        switch (request.TimeGrouping)
        {
            case TimeGrouping.Hourly:
                getTimeInterval = dt => dt.Date;
                timeFormat = "yyyy-MM-dd";
                break;
            case TimeGrouping.Daily:
                getTimeInterval = dt => dt.Date;
                timeFormat = "yyyy-MM-dd";
                break;
            case TimeGrouping.Weekly:
                getTimeInterval = dt =>
                {
                    var firstDayOfWeek = dt.Date.AddDays(-(int)dt.DayOfWeek);
                    return firstDayOfWeek;
                };
                timeFormat = "yyyy-MM-dd";
                break;
            case TimeGrouping.Monthly:
                getTimeInterval = dt => new DateTime(dt.Year, dt.Month, 1);
                timeFormat = "yyyy-MM";
                break;
            default:
                getTimeInterval = dt => dt.Date;
                timeFormat = "yyyy-MM-dd";
                break;
        }


        var allTimeIntervals =
            GenerateTimeIntervals(
                startDateNormalized,
                endDateNormalized,
                request.TimeGrouping
            )
            .Select(interval => new
            {
                TimeInterval = interval.Date,
                FormattedTime = interval.ToString(timeFormat)
            })
            .ToList();

        var changesByTime = auditLogs
            .GroupBy(a => getTimeInterval(a.CreationTime.Date))
            .Select(g => new
            {
                TimeInterval = g.Key,
                Count = g.Count()
            })
            .ToDictionary(x => x.TimeInterval, x => x.Count);

        var changesByTimeInterval = allTimeIntervals
            .Select(interval => new TimeSeriesDataPointResponseDto
            {
                TimeLabel = interval.TimeInterval,
                Value = changesByTime.ContainsKey(interval.TimeInterval)
                    ? changesByTime[interval.TimeInterval]
                    : 0
            })
            .ToList();

        var changesByState = new Dictionary<States, List<TimeSeriesDataPointResponseDto>>();
        foreach (States state in Enum.GetValues(typeof(States)))
        {
            var stateChangesByTime = auditLogs
                .Where(a => a.State == state)
                .GroupBy(a => getTimeInterval(a.CreationTime.Date))
                .ToDictionary(g => g.Key, g => g.Count());

            changesByState[state] = allTimeIntervals
                .Select(interval => new TimeSeriesDataPointResponseDto
                {
                    TimeLabel = interval.TimeInterval,
                    Value = stateChangesByTime.ContainsKey(interval.TimeInterval)
                        ? stateChangesByTime[interval.TimeInterval]
                        : 0
                })
                .ToList();
        }

        var changesByProperty = new Dictionary<string, List<TimeSeriesDataPointResponseDto>>();
        var propertiesToAnalyze = request.PropertyNames;
        if (propertiesToAnalyze == null || propertiesToAnalyze.Count == 0)
        {
            propertiesToAnalyze = auditLogs
                .SelectMany(a => a.EntityPropertyChanges)
                .GroupBy(p => p.PropertyName)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToList();
        }

        foreach (var propertyName in propertiesToAnalyze)
        {
            var propChangesByTime = auditLogs
                .SelectMany(a =>
                    a.EntityPropertyChanges
                        .Where(p => p.PropertyName == propertyName)
                        .Select(p => new { Time = a.CreationTime.Date, Property = p })
                )
                .GroupBy(x => getTimeInterval(x.Time))
                .ToDictionary(g => g.Key, g => g.Count());

            changesByProperty[propertyName] = allTimeIntervals
                .Select(interval => new TimeSeriesDataPointResponseDto
                {
                    TimeLabel = interval.TimeInterval,
                    Value = propChangesByTime.ContainsKey(interval.TimeInterval)
                        ? propChangesByTime[interval.TimeInterval]
                        : 0
                })
                .ToList();
        }

        return new EntityChangesTrendResponseDto
        {
            EntityName = request.EntityName,
            AnalysisPeriod = new DateRangeResponseDto
            {
                StartDate = startDateNormalized,
                EndDate = endDateNormalized
            },
            ChangesByTimeInterval = changesByTimeInterval,
            ChangesByState = changesByState,
            ChangesByProperty = changesByProperty
        };
    }

    /// <summary>
    /// Analyzes patterns in how users make changes to entities.
    /// </summary>
    /// <param name="request">The request containing analysis parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An analysis of user change behavior patterns.</returns>
    public async Task<UserChangeBehaviorResponseDto> AnalyzeUserChangeBehaviorAsync(UserChangeBehaviorRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = auditLogRepository.GetQueryable();
        queryable = queryable.Include(a => a.EntityPropertyChanges);
        queryable = queryable.Where(a => a.CreatorId == request.UserId);
        queryable = queryable.Where(a => a.CreationTime >= request.StartDate && a.CreationTime <= request.EndDate);
        queryable = queryable.WhereIf(request.EntityNames is { Count: > 0 }, a => request.EntityNames!.Contains(a.EntityName));
        queryable = queryable.WhereIf(request.States is { Count: > 0 }, a => request.States!.Contains(a.State));
        queryable = queryable.AsNoTracking();
        var auditLogs = await queryable.ToListAsync(cancellationToken);
        if (auditLogs.Count == 0)
        {
            return new UserChangeBehaviorResponseDto
            {
                UserId = request.UserId,
                AnalysisPeriod = new DateRangeResponseDto
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                },
                TotalChanges = 0,
                ChangesByHourOfDay = new Dictionary<int, int>(),
                ChangesByDayOfWeek = new Dictionary<DayOfWeek, int>(),
                ChangesByEntityType = new Dictionary<string, int>(),
                ChangesByState = new Dictionary<States, int>(),
                MostModifiedEntities = new List<ModifiedEntityResponseDto>(),
                MostChangedProperties = new List<PropertyChangeFrequencyResponseDto>()
            };
        }

        var changesByHourOfDay = auditLogs
            .GroupBy(a => a.CreationTime.Hour)
            .ToDictionary(g => g.Key, g => g.Count());

        for (var hour = 0; hour < 24; hour++)
        {
            if (!changesByHourOfDay.ContainsKey(hour))
            {
                changesByHourOfDay[hour] = 0;
            }
        }

        var changesByDayOfWeek = auditLogs
            .GroupBy(a => a.CreationTime.DayOfWeek)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
        {
            if (!changesByDayOfWeek.ContainsKey(day))
            {
                changesByDayOfWeek[day] = 0;
            }
        }

        var changesByEntityType = auditLogs
            .GroupBy(a => a.EntityName)
            .ToDictionary(g => g.Key, g => g.Count());

        var changesByState = auditLogs
            .GroupBy(a => a.State)
            .ToDictionary(g => g.Key, g => g.Count());

        var mostModifiedEntities = auditLogs
            .GroupBy(a => new { a.EntityId, a.EntityName })
            .Select(g => new ModifiedEntityResponseDto
            {
                EntityId = g.Key.EntityId,
                EntityName = g.Key.EntityName,
                ChangeCount = g.Count(),
                LastModified = g.Max(a => a.CreationTime),
                UniqueUserCount = 1,
                StateDistribution = g.GroupBy(a => a.State)
                    .ToDictionary(g2 => g2.Key, g2 => g2.Count())
            })
            .OrderByDescending(e => e.ChangeCount)
            .Take(10)
            .ToList();

        var mostChangedProperties = auditLogs
            .SelectMany(a => a.EntityPropertyChanges)
            .GroupBy(p => new { p.PropertyName, p.PropertyTypeFullName })
            .Select(g => new PropertyChangeFrequencyResponseDto
            {
                PropertyName = g.Key.PropertyName,
                PropertyType = g.Key.PropertyTypeFullName,
                ChangeCount = g.Count()
            })
            .OrderByDescending(p => p.ChangeCount)
            .Take(10)
            .ToList();

        return new UserChangeBehaviorResponseDto
        {
            UserId = request.UserId,
            AnalysisPeriod = new DateRangeResponseDto
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate
            },
            TotalChanges = auditLogs.Count,
            ChangesByHourOfDay = changesByHourOfDay,
            ChangesByDayOfWeek = changesByDayOfWeek,
            ChangesByEntityType = changesByEntityType,
            ChangesByState = changesByState,
            MostModifiedEntities = mostModifiedEntities,
            MostChangedProperties = mostChangedProperties
        };
    }

    /// <summary>
    /// Generates a list of time intervals based on the specified grouping.
    /// </summary>
    /// <param name="startDate">The start date of the time range.</param>
    /// <param name="endDate">The end date of the time range.</param>
    /// <param name="grouping">The time grouping interval (Hourly, Daily, Weekly, Monthly).</param>
    /// <returns>A list of DateTime values representing the time intervals.</returns>
    private List<DateTime> GenerateTimeIntervals(DateTime startDate, DateTime endDate, TimeGrouping grouping)
    {
        var intervals = new List<DateTime>();
        var current = startDate;

        while (current <= endDate)
        {
            intervals.Add(current);

            switch (grouping)
            {
                case TimeGrouping.Hourly:
                    current = current.AddHours(1);
                    break;
                case TimeGrouping.Daily:
                    current = current.AddDays(1);
                    break;
                case TimeGrouping.Weekly:
                    current = current.AddDays(7);
                    break;
                case TimeGrouping.Monthly:
                    current = current.AddMonths(1);
                    break;
                default:
                    current = current.AddDays(1);
                    break;
            }
        }

        return intervals;
    }
}