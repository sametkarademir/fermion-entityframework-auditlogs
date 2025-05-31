using FluentValidation;

namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class DateRangeRequestDto
{
    public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-7).ToUniversalTime();
    public DateTime EndDate { get; set; } = DateTime.Today.ToUniversalTime();
}

public class DataRangeRequestValidator : AbstractValidator<DateRangeRequestDto>
{
    public DataRangeRequestValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .LessThan(x => x.EndDate).WithMessage("Start date must be less than end date.")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Start date cannot be in the future.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("End date cannot be in the future.")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be greater than start date.");
    }
}