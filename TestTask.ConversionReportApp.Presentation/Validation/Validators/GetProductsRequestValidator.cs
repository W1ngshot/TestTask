using FluentValidation;
using Report.Api;

namespace TestTask.ConversionReportApp.Presentation.Validation.Validators;

public class GetProductsRequestValidator : AbstractValidator<GetReportsRequest>
{
    public GetProductsRequestValidator()
    {
        RuleFor(request => request.Page)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ValidationFailedMessages.Required)
            .ChildRules(validator =>
            {
                validator.RuleFor(pageInfo => pageInfo.PageNumber)
                    .GreaterThan(0)
                    .WithMessage(ValidationFailedMessages.MustBePositive);

                validator.RuleFor(pageInfo => pageInfo.ElementsPerPage)
                    .GreaterThan(0)
                    .WithMessage(ValidationFailedMessages.MustBePositive)
                    .LessThanOrEqualTo(50)
                    .WithMessage(ValidationFailedMessages.TooLarge);
            });

        RuleFor(request => request.ItemId)
            .NotNull()
            .WithMessage(ValidationFailedMessages.Required)
            .GreaterThan(0)
            .WithMessage(ValidationFailedMessages.MustBePositive);

        RuleFor(request => request.RegistrationId)
            .NotNull()
            .WithMessage(ValidationFailedMessages.Required)
            .GreaterThan(0)
            .WithMessage(ValidationFailedMessages.MustBePositive);
    }
}