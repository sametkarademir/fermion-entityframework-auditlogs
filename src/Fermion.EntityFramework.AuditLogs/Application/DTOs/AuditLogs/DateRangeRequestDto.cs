using FluentValidation;

namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class DateRangeRequestDto
{
    public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-7).ToUniversalTime();
    public DateTime EndDate { get; set; } = DateTime.Today.ToUniversalTime();
}

public class DateRangeRequestValidator : AbstractValidator<DateRangeRequestDto>
{
    public DateRangeRequestValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .LessThan(x => x.EndDate)
            .LessThanOrEqualTo(DateTime.Today);

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.Today)
            .GreaterThan(x => x.StartDate);
    }
}