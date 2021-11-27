using System;
using System.Collections.Generic;
using System.Linq;
using RefactorThis.Domain.Rule.Interfaces;
using RefactorThis.Domain.Rule.Service;
using RefactorThis.Persistence;
using RefactorThis.Persistence.Model;

namespace RefactorThis.Domain
{
	public class InvoicePaymentProcessor
	{
		private readonly InvoiceRepository _invoiceRepository;
        List<IPaymentRule> _rules = new List<IPaymentRule>();

		public InvoicePaymentProcessor( InvoiceRepository invoiceRepository )
		{
			_invoiceRepository = invoiceRepository;
            _rules.Add(new InvoiceNullValidator());
            _rules.Add(new InvoiceZeroValidator());
            _rules.Add(new InvoiceExistingPaymentsProcess());
            _rules.Add(new InvoicePaymentProcess());
        }

        public string ProcessPayment(Payment payment)
        {
            var inv = _invoiceRepository.Get(payment.Reference);
            var responseMessage = string.Empty;

            foreach (var rule in _rules)
            {
                (inv, responseMessage) = rule.Process(inv, payment);
                if (rule.IsTerminate())
                    return responseMessage;
            }

            

            _invoiceRepository.Add(inv);
            _invoiceRepository.Save();
            return responseMessage;
        }
    }
}