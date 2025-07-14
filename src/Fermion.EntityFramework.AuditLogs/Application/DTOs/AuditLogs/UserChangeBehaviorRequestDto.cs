using System.Text.Json.Serialization;
using Fermion.EntityFramework.AuditLogs.Domain.Enums;
using FluentValidation;

namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class UserChangeBehaviorRequestDto : DateRangeRequestDto
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
            .NotEmpty()
            .Must(x => x != Guid.Empty);

        RuleFor(x => x.EntityNames)
            .Must(x => x == null || x.Count <= 10)
            .ForEach(x => x.NotEmpty());

        RuleFor(x => x.States)
            .Must(x => x == null || x.Count <= 10)
            .ForEach(x => x.IsInEnum());
    }
}