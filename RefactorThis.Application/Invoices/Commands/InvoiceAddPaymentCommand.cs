using MediatR;
using RefactorThis.Application.Common.Interfaces;
using RefactorThis.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RefactorThis.Application.Invoices.Commands
{
    public class InvoiceAddPaymentCommand : IRequest<string>
    {
        public decimal Amount { get; set; }
        public string Reference { get; set; } = string.Empty;
    }

    public class InvoiceAddPaymentCommandHandler : IRequestHandler<InvoiceAddPaymentCommand, string>
    {
        readonly IInvoiceRepository _invoiceRepository;
        readonly IPaymentRepository _paymentRepository;
        public InvoiceAddPaymentCommandHandler(IInvoiceRepository invoiceRepository, IPaymentRepository paymentRepository)
        {
            _invoiceRepository = invoiceRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<string> ProcessPaymentAsync(Invoice invoice, Payment payment)
        {
            string? message;
            if (invoice.Payments.Any())
            {
                if (invoice.AmountDue < payment.Amount)
                    return "the payment is greater than the partial amount remaining";

                message = invoice.AmountDue == payment.Amount ? "final partial payment received, invoice is now fully paid" : "another partial payment received, still not fully paid";
            }
            else
            {
                if (payment.Amount > invoice.Amount)
                    return "the payment is greater than the invoice amount";

                message = invoice.AmountDue == payment.Amount ? "invoice is now fully paid" : "invoice is now partially paid";
            }
            await _paymentRepository.SavePaymentAsync(payment);
            return message;
        }

        public async Task<string> Handle(InvoiceAddPaymentCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.Reference);

            if (invoice.Amount == 0)
            {
                if (invoice.Payments == null || !invoice.Payments.Any())
                    return "no payment needed";
                else
                    throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
            }
            if (invoice.IsFullyPaid)
                return "invoice was already fully paid";

            var payment = new Payment() {
                Reference = request.Reference, 
                Amount = request.Amount 
            };
            return await ProcessPaymentAsync(invoice, payment);
        }
    }

}
