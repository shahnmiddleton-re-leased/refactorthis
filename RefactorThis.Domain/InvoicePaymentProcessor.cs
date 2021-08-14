using System;
using RefactorThis.Persistence;
using PaymentMessagesResource = RefactorThis.Domain.Resources.PaymentMessages;

namespace RefactorThis.Domain
{
	/// <summary>
	/// A class representation of InvoicePaymentProcessor
	/// </summary>
	public class InvoicePaymentProcessor
	{
		#region Declarations
		private readonly InvoiceRepository _invoiceRepository;

		private Invoice _invoice;

		private string _paymentResponse = string.Empty;
		#endregion

		#region Properties
		#endregion

		#region Constructor
		public InvoicePaymentProcessor( InvoiceRepository invoiceRepository )
		{
			_invoiceRepository = invoiceRepository;
		}
		#endregion

		#region Private Functions
		/// <summary>
		/// Updates the Invoice object by adding the payment of the customer.
		/// </summary>
		/// <param name="payment"></param>
		private void addPaymentToInvoice(Payment payment)
        {
			_invoice.AmountPaid += payment.Amount;
			_invoice.Payments.Add(payment);
		}

		/// <summary>
		/// Saves the Invoice object to the database.
		/// </summary>
		private void commitToDatabase()
        {
			_invoice.Save();
        }

		/// <summary>
		/// Before accepting the payment, the system should validate the Payment details first. 
		/// To accept a payment, the Invoice should have a balance,
		/// and the payment should not be greater than the balance/amount to be paid.
		/// </summary>
		/// <param name="payment">Amount to be paid from the Payment object</param>
		/// <param name="balance">Remaining balance of the Customer's Invoice</param>
		/// <returns>bool - returns true if the payment is valid and should be processed, false otherwise</returns>
		private bool shouldProcessPayment(decimal payment, decimal balance)
        {
			return (balance > 0 && balance >= payment && _invoice.Amount >= payment);
		}

		/// <summary>
		/// Calculate the remaining balance (if there is any) of the Customer's Invoice.
		/// </summary>
		/// <returns>decimal - current balance from the Invoice object</returns>
		private decimal calculateBalance()
        {
            return (_invoice.Amount - _invoice.AmountPaid);
        }

		/// <summary>
		/// Try to retrieve the invoice from the repo by the Payment Reference.
		/// If the Invoice does not exist, the system will throw an error.
		/// </summary>
		/// <param name="payment">Payment - Payment details</param>
		private void retrieveInvoice(Payment payment)
		{
			if (payment is null) //null checking
            {
				throw new InvalidOperationException(PaymentMessagesResource.PAYMENT_IS_NULL);
			}

			_invoice = _invoiceRepository.GetInvoice(payment.Reference);

			if(_invoice is null) //null checking
            {
				throw new InvalidOperationException(PaymentMessagesResource.NO_INVOICE_FOUND); 
			}
		}

		/// <summary>
		/// Checks if the Payment of the Customer is the initial payment by using the AmountPaid from the Invoice object.
		/// If the AmountPaid is 0, it is an indication that no payment has been made. 
		/// </summary>
		/// <returns>bool - returns true if the payment is initial, false otherwise</returns>
		private bool isInitialPayment()
        {
			return (_invoice.AmountPaid == 0);
        }

		/// <summary>
		/// Process the invalid payment by identifying the specific reason why we should not proceed with the processing.
		/// </summary>
		/// <param name="payment">Payment of the Customer</param>
		/// <param name="balance">Remaining balance to be paid by the Customer</param>
		private void handleInvalidPayment(decimal payment, decimal balance)
        {
            if(_invoice.Amount == 0 && _invoice.AmountPaid == 0) //no amount to be paid.
			{
				_paymentResponse = PaymentMessagesResource.NO_PAYMENT_NEEDED;
			}
			
			else if(_invoice.AmountPaid > 0 && _invoice.Amount == 0) //invoice is in an invalid state.
            {
				_paymentResponse =  PaymentMessagesResource.INVOICE_IS_INVALID;
			}

			else if(_invoice.Amount == _invoice.AmountPaid) //no amount to be paid, the invoice is already fully paid.
            {
				_paymentResponse = PaymentMessagesResource.INVOICE_ALREADY_FULLY_PAID;
            }

			else if(_invoice.AmountPaid > 0 && payment > balance) //the payment is greater than the balance/amount to be paid.
            {
				_paymentResponse = PaymentMessagesResource.PAYMENT_GREATER_THAN_PARTIAL;
			}
			
			else if(payment > _invoice.Amount) //the payment is greater than the amount to be paid.
            {
				_paymentResponse = PaymentMessagesResource.PAYMENT_GREATER_THAN_INVOICE;
			}
        }

		/// <summary>
		/// Proceed with processing the payment. At this time, we are certain that the payment is valid. 
		/// </summary>
		/// <param name="payment"></param>
		/// <param name="balance"></param>
		private void handleValidPayment(Payment payment, decimal balance)
        {
            if (balance == payment.Amount) //payment is completed and fully paid
            {
				_paymentResponse = isInitialPayment() ? PaymentMessagesResource.INVOICE_NOW_FULLY_PAID : PaymentMessagesResource.FINAL_PARTIAL_FULLY_PAID;
			}
			else if(balance > payment.Amount) //payment is processed but not yet fully paid
            {
				_paymentResponse = isInitialPayment() ? PaymentMessagesResource.INVOICE_NOW_PARTIALLY_PAID : PaymentMessagesResource.PARTIAL_PAYMENT_RECEIVED;
            }

			addPaymentToInvoice(payment);
			commitToDatabase(); //we should only be saving the transaction to the database if it's a valid one.
		}
		#endregion

		#region Methods
		/// <summary>
		/// Process the payment of the Customer. The payment may not be valid, 
		/// so we need to check the payment details before processing.
		/// </summary>
		/// <param name="payment">Payment details</param>
		/// <returns>string: Payment response message</returns>
		public string ProcessPayment( Payment payment )
        {
			retrieveInvoice(payment);

			decimal balance = calculateBalance();

			if (shouldProcessPayment(payment.Amount, balance)) 
            {
				handleValidPayment(payment, balance);
            } 
			else
            {
				handleInvalidPayment(payment.Amount, balance);
			}

			return _paymentResponse;
		}
    }

    #endregion
}