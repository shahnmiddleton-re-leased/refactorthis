using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
	public class InvoicePaymentProcessor
	{
		private readonly InvoiceRepository _invoiceRepository;
		private Invoice _paymentInvoice;

		public InvoicePaymentProcessor(InvoiceRepository invoiceRepository)
		{
			_invoiceRepository = invoiceRepository;
		}

		/// <summary>
		/// Get/Set for the invoice relating to a payment.
		/// </summary>
		/// <exception cref="InvalidOperationException">Invoice not found by reference.</exception>
		private Invoice PaymentInvoice
		{
			get => _paymentInvoice;
			
			// TODO: Discuss with team, do we want global exception messages? These should not go to the user, so no i18n required.
			set => _paymentInvoice = value ?? throw new InvalidOperationException("There is no invoice matching this payment");
		}

		// :TODO: Probably create internal class for PaymentInvoice.
		private decimal InvoicePastPaymentTotal { get; set; }
		private decimal InvoiceRemainingBalance { get; set; }

		private void SetInvoicePastPaymentTotal(List<Payment> payments)
		{
			if (payments != null && payments.Any())
			{
				// There are payments. Set the sum.
				InvoicePastPaymentTotal = payments.Sum(x => x.Amount);
			}
			else
			{
				// No past payments. Set to zero.
				InvoicePastPaymentTotal = 0;
			}
		}

		/// <summary>
		/// Submits payment to invoice and saves.
		/// </summary>
		/// <param name="invoice">The invoice object to update</param>
		/// <param name="payment">The payment object to apply on the Invoice</param>
		private static void AddInvoicePayment(Invoice invoice, Payment payment)
		{
			// Calculate total payment.
			invoice.AmountPaid += payment.Amount;

			// Add payment object to invoice.
			invoice.Payments?.Add(payment);
					
			invoice.Save();
		}

		/// <summary>
		/// Given a payment object, reconcile the invoice with the payment amount. Returns status message.
		/// </summary>
		/// <remarks>
		/// List of scenarios to handle
		/// - No valid invoice or invoice 0 / payment 0
		/// - Invoice already paid
		/// - Invoice paid in full
		/// - Invoice underpayment (w/w/o past payments)
		/// - Invoice overpayment (w/w/o past payments)
		/// </remarks>
		/// <param name="payment"></param>
		/// <returns>String response message unless exception thrown.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public string ProcessPayment( Payment payment )
		{
			try
			{
				// Get the invoice by payment reference.
				PaymentInvoice = _invoiceRepository.GetInvoice(payment.Reference);

				// Collect the total of past payments to this invoice.
				SetInvoicePastPaymentTotal(PaymentInvoice.Payments);
				// :TODO: Is it necessary to calculate past payments if the Payment object includes AmountPaid?
				
				// The invoice is 0.
				if (PaymentInvoice.Amount == 0)
				{
					if(!InvoicePastPaymentTotal.Equals(0))
					{
						// The invoice has payments because it is not zero. Rhut ruh.
						throw new InvalidOperationException(
							"The invoice is in an invalid state, it has an amount of 0 and it has payments.");
						// TODO: Bubble this logic to see if it is even a condition that exists.
					}
					
					// Invoice zero, payments zero.
					throw new InvoicePaymentException("no payment needed", payment.Reference);
				}
				
				// The remaining balance of the invoice before this payment.
				InvoiceRemainingBalance = PaymentInvoice.Amount - InvoicePastPaymentTotal;

				if (InvoicePastPaymentTotal == PaymentInvoice.Amount)
				{
					// Invoice has past payments and is already settled.
					throw new InvoicePaymentException("invoice was already fully paid", payment.Reference);
				}

				if ( payment.Amount > InvoiceRemainingBalance )
				{
					// Overpayment of invoice. Do not store payment on invoice.
					throw new InvoicePaymentException(
						!InvoicePastPaymentTotal.Equals(0)
							? "the payment is greater than the partial amount remaining"
							: "the payment is greater than the invoice amount",
						payment.Reference);
				}

				// Add this payment to the invoice and save it.
				AddInvoicePayment(PaymentInvoice, payment);

				if (InvoiceRemainingBalance == payment.Amount)
				{
					// Invoice fully paid. Vary response based on past payment existence.
					throw new InvoicePaymentException(
						!InvoicePastPaymentTotal.Equals(0)
							? "final partial payment received, invoice is now fully paid"
							: "invoice is now fully paid",
						payment.Reference);
				}

				// Invoice partially paid. Vary response based on past payment existence.
				throw new InvoicePaymentException(
					!InvoicePastPaymentTotal.Equals(0)
						? "another partial payment received, still not fully paid"
						: "invoice is now partially paid",
					payment.Reference);
			}
			catch (InvoicePaymentException e)
			{
				// Return the message from the custom exception.
				return (e.Message);
			}
		}
	}
	
	[Serializable]
	internal class InvoicePaymentException : Exception
	{
		public string PaymentReference { get; }

		public InvoicePaymentException() { }
		public InvoicePaymentException(string message) : base(message) { }
		public InvoicePaymentException(string message, Exception inner) : base(message, inner) { }
		
		public InvoicePaymentException(string message, string paymentReference)
			: this(message)
		{
			// Added payment reference for future use in logging / debugging.
			PaymentReference = paymentReference;
		}
	}
}