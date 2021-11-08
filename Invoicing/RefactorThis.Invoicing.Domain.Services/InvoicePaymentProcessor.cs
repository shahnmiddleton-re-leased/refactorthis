using RefactorThis.Invoicing.Domain.Services.Rules;
using RefactorThis.Invoicing.DataAccess.Interfaces;
using RefactorThis.Invoicing.DataAccess.Repository;
using RefactorThis.Invoicing.Domain.Interfaces;
using RefactorThis.Invoicing.Domain.Interfaces.Rules;
using RefactorThis.Invoicing.Domain.Model;
using RefactorThis.Common.Interfaces.Rules;
using System.Linq;

namespace RefactorThis.Invoicing.Domain.Services
{
    public class InvoicePaymentProcessor : IInvoicePaymentProcessor
    {
        private readonly IInvoiceReadRepository _invoiceReadRepository;
        private readonly IInvoiceWriteRepository _invoiceWriteRepository;
        private readonly IInvoicingRulesEngine _ruleEngineService;

        public InvoicePaymentProcessor ()
        {
            // normally I would use an IOC container and contructor injection
            _invoiceReadRepository = new InvoiceReadRepository();
            _invoiceWriteRepository = new InvoiceWriteRepository();
            _ruleEngineService = new InvoicingRulesEngine();
        }

        public IRuleContext ProcessPayment(Payment payment)
        {
            var context = new ProcessPaymentRuleContext();
            var returnContext = new ProcessPaymentRuleReturnContext();

            var invoice = _invoiceReadRepository.GetInvoice(payment.Reference);

            context.Invoice = invoice;
            context.Payment = payment;

            // perform a validation befoe saving
            _ruleEngineService.ExecuteRules(context, InvoicingRulesSet.ProcessPayment);
            if (!context.IsValid())
            {
                // return the validation messages if invalid, no save performed
                returnContext.Validations.AddRange(context.Validations);
                return returnContext;
            }

            // now if no errors, add information to return context
            if (invoice.Payments != null && invoice.Payments.Any())
            {
                if ((invoice.Amount - invoice.AmountPaid) == payment.Amount)
                {
                    returnContext.Information.Add("final partial payment received, invoice is now fully paid");
                }
                else
                {
                    returnContext.Information.Add("another partial payment received, still not fully paid");
                }
            }
            else
            {
                if (invoice.Amount == payment.Amount)
                {

                    returnContext.Information.Add("invoice is now fully paid");
                }
                else
                {
                    returnContext.Information.Add("invoice is now partially paid");
                }
            }

            //perform required actions on the invoice
            invoice.AmountPaid += payment.Amount;
            invoice.Payments.Add(payment);

            // and save to the repository
            _invoiceWriteRepository.SaveInvoice(invoice);

            return returnContext;
        }

        public void AddInvoice(Invoice invoice)
        {
            _invoiceWriteRepository.Add(invoice);
        }
    }
}
