using RefactorThis.Domain.PaymentRule.Interfaces;
using RefactorThis.Persistence.Model;
using RefactorThis.Persistence.Repository;
using System;
using System.Linq;

namespace RefactorThis.Domain
{
	public class InvoicePaymentProcessor
	{
		private readonly InvoiceRepository _invoiceRepository;
		private readonly IPaymentRuleRepository _paymentRuleRepository;

		public InvoicePaymentProcessor(InvoiceRepository invoiceRepository, IPaymentRuleRepository paymentRuleRepository)
		{
			_invoiceRepository = invoiceRepository;
			_paymentRuleRepository = paymentRuleRepository;
		}

		public string ProcessPayment(Payment payment)
		{
			var inv = _invoiceRepository.Get(payment.Reference);
			var paymentRuleServiceResponce = _paymentRuleRepository.ProcessPayment(inv, payment);
			_invoiceRepository.Save();
			return paymentRuleServiceResponce.Message;
		}
	}
}