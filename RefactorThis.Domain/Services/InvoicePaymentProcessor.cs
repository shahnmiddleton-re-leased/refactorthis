using FluentResults;
using RefactorThis.Domain.Model.Invoices;
using RefactorThis.Domain.Repositories;

namespace RefactorThis.Domain.Services
{
	public class InvoicePaymentProcessor
	{
        private readonly IUnitOfWork _unitOfWork;

		public InvoicePaymentProcessor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

		public Result ProcessPayment( Payment payment )
		{
			var invoice = _unitOfWork.Invoices.GetInvoice( payment.Reference );

			if ( invoice == null )
			{
				return Result.Fail("There is no invoice matching this payment" );
			}

            var result = invoice.AddPayment(payment);

            if (result.IsSuccess)
            {
                _unitOfWork.Save();
            }

			return result;
		}

		public IUnitOfWork UnitOfWork => _unitOfWork;
	}
}