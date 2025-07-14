namespace Fermion.EntityFramework.AuditLogs.Domain.Enums;

/// <summary>
/// Represents the time intervals used for grouping audit log data.
/// </summary>
public enum TimeGrouping
{
    /// <summary>
    /// Group data by hour.
    /// </summary>
    Hourly,

    /// <summary>
    /// Group data by day.
    /// </summary>
    Daily,

    /// <summary>
    /// Group data by week.
    /// </summary>
    Weekly,

    /// <summary>
    /// Group data by month.
    /// </summary>
    Monthly
}