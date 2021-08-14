using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Persistence
{
    /// <summary>
    /// A class representation of an Invoice
    /// </summary>
	public class Invoice
	{
        #region Declarations
        private readonly InvoiceRepository _repository;

        private decimal _amountPaid;
        #endregion

        #region Constructor
        public Invoice( InvoiceRepository repository )
		{
			_repository = repository;
		}
        #endregion

        #region Properties 
        public decimal Amount { get; set; }

		public decimal AmountPaid { //Ideally, this should not be done but I had to put some logic/process in the GET since the two test cases:
                                    //ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount & ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount
                                    //are assigning the AmountPaid to 0 and this should not be the value since the Payments list object have values. To my understanding, AmountPaid should contain the total amount paid by the Customer? Which is also the .Sum of the List of Payments?
                                    //With the refactored code, the AmountPaid is now being used instead of the sum of the Payments list to get the total amount paid by the Customer for consistency purposes.
            get
            {
                if(Payments != null)
                {
                    return Payments.Sum(x => x.Amount);
                }

                return _amountPaid;
            }
            set
            {
                _amountPaid = value;
            }
        }
		public List<Payment> Payments { get; set; }
        #endregion

        #region Methods
        public void Save()
        {
            _repository.SaveInvoice(this);
        }
        #endregion
    }
}