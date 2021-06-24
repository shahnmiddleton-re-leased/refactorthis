using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Domain.Enum.Payment
{
    public class PaymentValidate
    {
        public List<String> ValidationCases { get; set; }

        public PaymentValidate()
        {
            ValidationCases.Add("NoInoiceFoundForPaymentReference");
            ValidationCases.Add("NoPaymentNeeded");
            ValidationCases.Add("InvoiceAlreadyFullyPaid");
            ValidationCases.Add("PartialPaymentExistsAndAmountPaidExceedsAmountDue");
            ValidationCases.Add("NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount");
            ValidationCases.Add("PartialPaymentExistsAndAmountPaidEqualsAmountDue");
            ValidationCases.Add("NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount");
            ValidationCases.Add("PartialPaymentExistsAndAmountPaidIsLessThanAmountDue");
            ValidationCases.Add("NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount");
        }
    }

}
