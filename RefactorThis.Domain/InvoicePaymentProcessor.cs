using RefactorThis.Domain.PaymentHandlers;
using RefactorThis.Persistence;
using System;
using System.Collections.Generic;

namespace RefactorThis.Domain
{
    public class InvoicePaymentProcessor
    {
        private readonly InvoiceRepository _invoiceRepository;
        private readonly IList<IPaymentHandler> _handlers;

        public InvoicePaymentProcessor(InvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
            _handlers = new List<IPaymentHandler>
            {
                new ValidationHandler(),
                new PreProcessingHandler(),
                new FirstPaymentHandler(),
                new PartialPaymentHandler()
            };
        }

        public string ProcessPayment(Payment payment)
        {
            var inv = _invoiceRepository.GetInvoice(payment.Reference);

            var responseMessage = string.Empty;

            if (inv == null)
            {
                throw new InvalidOperationException("There is no invoice matching this payment");
            }
            else
            {
                var payload = new Payload
                {
                    Invoice = inv,
                    Payment = payment,
                    Response = responseMessage
                };

                foreach (var handler in _handlers)
                {
                    handler.Handle(payload);
                }

                payload.Invoice.Save();
                return payload.Response;
            }
        }
    }
}