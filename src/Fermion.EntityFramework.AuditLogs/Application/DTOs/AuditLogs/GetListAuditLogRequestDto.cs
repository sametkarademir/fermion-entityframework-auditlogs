using System.Text.Json.Serialization;
using Fermion.EntityFramework.AuditLogs.Core.Enums;
using Fermion.EntityFramework.Shared.DTOs.Sorting;
using FluentValidation;

namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class GetListAuditLogRequestDto
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 25;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SortOrderTypes Order { get; set; } = SortOrderTypes.Desc;
    public string? Field { get; set; } = null;

    public string? EntityId { get; set; }
    public string? EntityName { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public States? States { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? UserId { get; set; }
    public Guid? SnapshotId { get; set; }
    public Guid? SessionId { get; set; }
    public Guid? CorrelationId { get; set; }
}

public class GetListAuditLogRequestValidation : AbstractValidator<GetListAuditLogRequestDto>
{
    public GetListAuditLogRequestValidation()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than or equal to 1.");

        RuleFor(x => x.PerPage)
            .InclusiveBetween(1, 100)
            .WithMessage("Items per page must be between 1 and 100.");

        RuleFor(x => x.Field)
            .MaximumLength(100).WithMessage("Field name cannot exceed 100 characters.")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Field name can only contain alphanumeric characters and underscores.");

        RuleFor(x => x.Order)
            .IsInEnum().WithMessage("Order must be either Asc or Desc.");

        RuleFor(x => x.EntityId)
            .NotEmpty().WithMessage("Entity ID cannot be empty.")
            .MaximumLength(100).WithMessage("Entity ID cannot exceed 100 characters.");

        RuleFor(x => x.EntityName)
            .NotEmpty().WithMessage("Entity Name cannot be empty.")
            .MaximumLength(500).WithMessage("Entity Name cannot exceed 500 characters.");

        RuleFor(x => x.States)
            .IsInEnum().WithMessage("Invalid state value.");

        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate).WithMessage("Start date must be less than end date.")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Start date cannot be in the future.");

        RuleFor(x => x.EndDate)
            .LessThanOrEqualTo(DateTime.Today).WithMessage("End date cannot be in the future.")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be greater than start date.");

        RuleFor(x => x.UserId)
            .Must(x => x == null || x != Guid.Empty).WithMessage("User ID cannot be empty.");

        RuleFor(x => x.SnapshotId)
            .Must(x => x == null || x != Guid.Empty).WithMessage("Snapshot ID cannot be empty.");

        RuleFor(x => x.SessionId)
            .Must(x => x == null || x != Guid.Empty).WithMessage("Session ID cannot be empty.");

        RuleFor(x => x.CorrelationId)
            .Must(x => x == null || x != Guid.Empty).WithMessage("Correlation ID cannot be empty.");
    }
}