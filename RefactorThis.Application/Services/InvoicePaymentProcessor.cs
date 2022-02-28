using System;
using RefactorThis.Application.Abstractions.Services;
using RefactorThis.Application.Common;
using RefactorThis.Domain.Abstractions.Messages;
using RefactorThis.Domain.Abstractions.Repositories;
using RefactorThis.Domain.Entities.Invoice;
using RefactorThis.Domain.Entities.Invoice.Events;
using RefactorThis.Domain.Entities.Invoice.Exceptions;

namespace RefactorThis.Application.Services
{
    public class InvoicePaymentProcessor : IInvoicePaymentProcessor
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoicePaymentProcessor(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public string ProcessPayment(Payment payment)
        {
            var inv = _invoiceRepository.GetInvoice(payment.Reference);

            if (inv == null)
            {
                throw new InvalidOperationException("There is no invoice matching this payment");
            }

            try
            {
                var @event = inv.ProcessPayment(payment);

                // todo: publish domain event to any interested parties before persisting

                _invoiceRepository.SaveInvoice(inv);

                return MapDomainEventToResponse(@event);
            }
            catch (NoPaymentRequiredException)
            {
                return ProcessPaymentResponse.InvoiceAlreadyPaid;
            }
            catch (PartialPaymentTooMuchException)
            {
                return ProcessPaymentResponse.PartialPaymentGreaterThanInvoiceAmount;
            }
            catch (FullPaymentTooMuchException)
            {
                return ProcessPaymentResponse.PaymentGreaterThanInvoiceAmount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string MapDomainEventToResponse(IDomainEvent @event)
        {
            if (!(@event is PaymentProcessed paymentProcessedEvent))
            {
                throw new InvalidOperationException("Invalid domain event type");
            }

            if (paymentProcessedEvent.IsPaidInFull)
            {
                return paymentProcessedEvent.PaymentCount == 1 ? ProcessPaymentResponse.InvoiceFullyPaid : ProcessPaymentResponse.PartialPaymentInvoiceFullyPaid;
            }
            else
            {
                return paymentProcessedEvent.PaymentCount == 1 ? ProcessPaymentResponse.InvoicePartiallyPaid : ProcessPaymentResponse.PartialPaymentInvoicePartiallyPaid;
            }
        }
    }
}
