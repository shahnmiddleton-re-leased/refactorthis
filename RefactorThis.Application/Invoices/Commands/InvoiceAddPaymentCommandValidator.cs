using FluentValidation;

namespace RefactorThis.Application.Invoices.Commands
{
    public class InvoiceAddPaymentCommandValidator : AbstractValidator<InvoiceAddPaymentCommand>
    {
        public InvoiceAddPaymentCommandValidator()
        {
            RuleFor(v => v.Reference)
                .NotEmpty();
        }
    }
}
