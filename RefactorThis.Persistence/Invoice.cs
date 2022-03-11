using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RefactorThis.Persistence
{
    public class Invoice
    {
        /// <summary>
        /// AddPaymentStatus is used as the return type of AddPayment method
        /// </summary>
        public enum AddPaymentStatus
        {
            ///<summary>Payment is successful and invoice is fully paid</summary>
            SuccessFullyPaid,
            ///<summary>Payment is successful and invoice is partially paid</summary>
            SuccessPartiallyPaid,
            ///<summary>Payment is failed because the payment amount is invalid</summary>
            FailInvalidAmount,
            ///<summary>Payment is failed because the payment object is null</summary>
            FailNullPayment,
            ///<summary>Payment is failed because the payment amount is greater than the unpaid amount of invoice</summary>
            FailOverPayment,
            ///<summary>Payment is failed because the invoice is already fully paid</summary>
            FailFullyPaidInvoice
        }

        private readonly InvoiceRepository _repository;
        private readonly List<Payment> _payments;
        private decimal _amount;

        public Invoice()
        {
            _repository = InvoiceRepository.GetInvoiceRepository();
            _payments = new List<Payment>();
        }
        
        /// <summary>
        /// AddPayment validates the payment before adding it to the invoice. The validation is done on the payment amount and the remaining payment of the invoice.
        /// </summary>
        /// <param name="payment">Payment object containing the payment amount</param>
        /// <returns>
        ///   enum  AddPaymentStatus: depending outcome of he payment an appropriate enum value is returned.
        ///   Enum values SuccessFullyPaid and SuccessPartiallyPaid indicates the successful payment, while all other enum values indicate failure of the payment.
        /// </returns>
        public AddPaymentStatus AddPayment(Payment payment)
        {
            if (payment == null)
            {
                return AddPaymentStatus.FailNullPayment;
            }

            if (payment.Amount <= 0)
            {
                return AddPaymentStatus.FailInvalidAmount;
            }

            decimal duePayment = DuePayment();
            if (duePayment == 0)
            {
                return AddPaymentStatus.FailFullyPaidInvoice;
            }

            if (duePayment < payment.Amount)
            {
                return AddPaymentStatus.FailOverPayment;
            }

            _payments.Add(payment);

            return duePayment == payment.Amount ? AddPaymentStatus.SuccessFullyPaid : AddPaymentStatus.SuccessPartiallyPaid;
        }

        public void Save()
        {
            _repository.SaveInvoice(this);
        }

        /// <summary>
        /// DuePayment returns the remaining payment of the invoice subtracting the payment amount from invoice amount.
        /// </summary>
        /// <returns>
        ///   decimal: remaining payment of the invoice
        /// </returns>
        public decimal DuePayment()
        {
            return Amount - AmountPaid;
        }


        /// <summary>
        /// AmountPaid returns the sum of all payments.
        /// </summary>
        /// <returns>
        ///   decimal: sum of all payments
        /// </returns>
        public decimal AmountPaid => _payments.Sum(payment => payment.Amount);


        public decimal Amount
        {
            get => _amount;
            
            // Setting the Amount is only allowed if the amount being set is greater than or equal to paid amount.
            // Also Amount must be a positive value 
            set
            {
                if (value < AmountPaid)
                {
                    throw new InvalidOperationException(
                        "Invoice amount should be positive and greater than the paid amount");
                }

                _amount = value;
            }
        }
        
        /// <summary>
        /// GetPayments returns the list of payments as a read only collection.
        /// </summary>
        /// <returns>
        ///   IEnumerable<Payment>: list of payments
        /// </returns>
        public IEnumerable<Payment> GetPayments()
        {
            return _payments.AsReadOnly();
        }
    }
}