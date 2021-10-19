using RefactorThis.Domain;
using RefactorThis.Persistence;
using System;
using System.Linq;

namespace RefactorThis.Validation
{
    public class InvoiceValidation
    {
        // ---- Method for exception handling
        InvalidOperationException NewInvalidOperationException(string code)
        {
            string description;
            switch (code)
            {
                case Constants.noInvoice:
                    description = Constants.descNoInvoice;
                    break;
                case Constants.invalidState:
                    description = Constants.descInvalidState;
                    break;
                default:
                    description = Constants.descSomethingWentWrong;
                    break;
            }


            return new InvalidOperationException(description);
        }

        private decimal getTotalInventoryPayables(Invoice inv)
        {
            return inv.Payments.Sum(x => x.Amount);
        }

        public void CheckEmptyPayments(Invoice inv, out string responseMessage)
        {
            if (inv.Payments == null || !inv.Payments.Any())
            {
                responseMessage = Constants.msgNoPaymentNeeded;
            }
            else
            {
                throw NewInvalidOperationException(Constants.invalidState);
            }
        }

        public bool CheckInvoiceValidity(Invoice inv, out string responseMessage)
        {
            responseMessage = null;
            // ---- check if Invoice is null
            _ = inv ?? throw NewInvalidOperationException(Constants.noInvoice);
            // ---- check if Invoice is not 0 value
            bool result = inv.Amount != 0;
            // ---- confirm for empty payments
            if (!result) CheckEmptyPayments(inv, out responseMessage);

            return result;
        }

        public bool isExistingInvoice(Invoice inv)
        {
            return inv.Payments != null && inv.Payments.Any();
        }

        public bool CheckIfFullyPaid(Payment payment, Invoice inv, out string responseMessage)
        {
            if (getTotalInventoryPayables(inv) != 0 && inv.Amount == getTotalInventoryPayables(inv)) { responseMessage = Constants.msgInvoiceIsAlreadyPaid; return true; }

            responseMessage = null;
            return false;
        }

        public bool CheckPaymentIfGreaterThanPartial(Payment payment, Invoice inv, out string responseMessage)
        {
            if (getTotalInventoryPayables(inv) != 0 && payment.Amount > (inv.Amount - inv.AmountPaid)) { responseMessage = Constants.msgPaymentIsGreaterThanPartialAmount; return true; }

            responseMessage = null;
            return false;
        }

        public bool CheckPaymentIfGreaterThanAmount(Payment payment, Invoice inv, out string responseMessage)
        {
            if (payment.Amount > inv.Amount) { responseMessage = Constants.msgPaymentIsGreaterThanInvoice; return true; }

            responseMessage = null;
            return false;
        }

        public bool ValidatePayment(Payment payment, Invoice inv, string paymentMethod, out string responseMessage)
        {
            // initiate
            // this validatepayment Method will collate the message as the system checks the conditions
            switch (paymentMethod)
            {
                case Constants.partialPayment:
                    if (CheckIfFullyPaid(payment, inv, out responseMessage) || CheckPaymentIfGreaterThanPartial(payment, inv, out responseMessage)){ return false; }
                    responseMessage = getInvoiceResponseMessage(payment, inv);
                    return true;
                default:
                    if (CheckPaymentIfGreaterThanAmount( payment, inv, out responseMessage)) { return true; }
                    responseMessage = getInvoiceResponseMessage(payment, inv);
                    return false;
            }

        }

        private string getInvoiceResponseMessage(Payment payment, Invoice inv)
        {
            if ((inv.Amount - inv.AmountPaid) == payment.Amount)
            {
                return Constants.msgInvoicePartiallyPaidFullyPaid;
            }
            else if (inv.Amount == payment.Amount)
            {
                return Constants.msgInvoiceFullyPaid;
            }
            else if (isExistingInvoice(inv))
            {
                return Constants.msgInvoicePartiallyPaidNotFullyPaid;
            }
            else
            {
                return Constants.msgInvoicePartiallyPaid;
            }
        }
    }
}
