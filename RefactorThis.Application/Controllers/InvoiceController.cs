using RefactorThis.Domain.Entities.Invoices;
using RefactorThis.Domain.Interfaces;

namespace RefactorThis.Application.Controllers
{
    public class InvoiceController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInvoicePaymentProcessor _invoicePaymentProcessor;

        public InvoiceController(IUnitOfWork unitOfWork, IInvoiceFactory invoiceFactory)
        {
            _unitOfWork = unitOfWork;
            _invoicePaymentProcessor = invoiceFactory.CreateInvoicePaymentProcessor();
        }

        public string AddPayment(Payment payment)
        {
            var invoice = _unitOfWork.InvoiceRepository.FindInvoiceByReference(payment.Reference);
            string responseMessage = _invoicePaymentProcessor.ProcessPayment(invoice, payment);
            _unitOfWork.Complete();

            return responseMessage;
        }
    }
}
