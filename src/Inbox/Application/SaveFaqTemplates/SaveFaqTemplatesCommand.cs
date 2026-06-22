namespace QuotationAccelerator.Inbox.Application.SaveFaqTemplates;

using FluentValidation;
using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Results;

public sealed record SaveFaqTemplatesCommand(IReadOnlyList<FaqTemplate> Templates) : ICommand<Result>;

public sealed class SaveFaqTemplatesValidator : AbstractValidator<SaveFaqTemplatesCommand>
{
    public SaveFaqTemplatesValidator()
    {
        RuleForEach(command => command.Templates).ChildRules(template =>
        {
            template.RuleFor(item => item.Keywords).NotEmpty();
            template.RuleFor(item => item.ReplyBody).NotEmpty();
        });
    }
}

public sealed class SaveFaqTemplatesHandler(
    IFaqTemplateRepository repository,
    IValidator<SaveFaqTemplatesCommand> validator)
    : ICommandHandler<SaveFaqTemplatesCommand, Result>
{
    public async Task<Result> HandleAsync(SaveFaqTemplatesCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure(validation.Errors.Select(error => error.ErrorMessage));
        }

        await repository.SaveAllAsync(command.Templates, cancellationToken);
        return Result.Success();
    }
}
