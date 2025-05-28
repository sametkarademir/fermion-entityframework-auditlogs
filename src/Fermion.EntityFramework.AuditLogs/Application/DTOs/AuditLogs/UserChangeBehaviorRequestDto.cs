using System.Text.Json.Serialization;
using Fermion.EntityFramework.AuditLogs.Core.Enums;
using FluentValidation;

namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class UserChangeBehaviorRequestDto : DataRangeRequestDto
{
    public Guid UserId { get; set; }
    public List<string>? EntityNames { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public List<States>? States { get; set; }
}

public class UserChangeBehaviorRequestValidator : AbstractValidator<UserChangeBehaviorRequestDto>
{
    public UserChangeBehaviorRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User Id is required.")
            .Must(x => x != Guid.Empty).WithMessage("User Id cannot be empty.");

        RuleFor(x => x.EntityNames)
            .Must(x => x == null || x.Count <= 10).WithMessage("Entity names cannot exceed 10 items.")
            .ForEach(x => x.NotEmpty().WithMessage("Entity name cannot be empty."));

        RuleFor(x => x.States)
            .Must(x => x == null || x.Count <= 10).WithMessage("States cannot exceed 10 items.")
            .ForEach(x => x.IsInEnum().WithMessage("Invalid state value."));
    }
}