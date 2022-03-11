using System;
using System.Linq;
using System.Threading.Tasks;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
    public class InvoicePaymentProcessor
    {
        private readonly InvoiceRepository _invoiceRepository;

        public InvoicePaymentProcessor(InvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public string ProcessPayment(Payment payment)
        {
            var inv = _invoiceRepository.GetInvoice(payment.Reference);

            var responseMessage = string.Empty;

            if (inv == null)
            {
                throw new InvalidOperationException("There is no invoice matching this payment");
            }
            else
            {
                if (inv.Amount == 0)
                {
                    if (inv.Payments == null || !inv.Payments.Any())
                    {
                        responseMessage = "no payment needed";
                    }
                    else
                    {
                        throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
                    }
                }
                else
                {
                    if (inv.Payments != null && inv.Payments.Any())
                    {
                        if (inv.Payments.Sum(x => x.Amount) != 0 && inv.Amount == inv.Payments.Sum(x => x.Amount))
                        {
                            responseMessage = "invoice was already fully paid";
                        }
                        else if (inv.Payments.Sum(x => x.Amount) != 0 && payment.Amount > (inv.Amount - inv.AmountPaid))
                        {
                            responseMessage = "the payment is greater than the partial amount remaining";
                        }
                        else
                        {
                            if ((inv.Amount - inv.AmountPaid) == payment.Amount)
                            {
                                inv.AmountPaid += payment.Amount;
                                inv.Payments.Add(payment);
                                responseMessage = "final partial payment received, invoice is now fully paid";
                            }
                            else
                            {
                                inv.AmountPaid += payment.Amount;
                                inv.Payments.Add(payment);
                                responseMessage = "another partial payment received, still not fully paid";
                            }
                        }
                    }
                    else
                    {
                        if (payment.Amount > inv.Amount)
                        {
                            responseMessage = "the payment is greater than the invoice amount";
                        }
                        else if (inv.Amount == payment.Amount)
                        {
                            inv.AmountPaid = payment.Amount;
                            inv.Payments.Add(payment);
                            responseMessage = "invoice is now fully paid";
                        }
                        else
                        {
                            inv.AmountPaid = payment.Amount;
                            inv.Payments.Add(payment);
                            responseMessage = "invoice is now partially paid";
                        }
                    }
                }
            }

            inv.Save();

            return responseMessage;
        }

        public string ProcessThePayment(Payment payment)
        {

            string responseMessage = string.Empty;
            var inv = _invoiceRepository.GetInvoice(payment.Reference);
            try
            {
                if (inv == null) throw new InvalidOperationException(responseMessage = "There is no invoice matching this payment");
                //Check if amount is 0
                if (inv.Amount == 0)
                {
                    //Check if any payements needed to pay.
                    return inv.Payments == null || !inv.Payments.Any() ? responseMessage = "no payment needed" : throw new InvalidOperationException(responseMessage = "The invoice is in an invalid state, it has an amount of 0 and it has payments.");

                }
                else
                {
                    if (inv.Payments != null && inv.Payments.Any())
                    {
                        decimal sumOfPayments = inv.Payments.Sum(x => x.Amount);
                        decimal invAmountMinusInvAmountPaid = inv.Amount - inv.AmountPaid;

                        if (sumOfPayments != 0)
                        {
                            if (inv.Amount == sumOfPayments)
                            {
                                responseMessage = "invoice was already fully paid";
                            }
                            else if (payment.Amount > invAmountMinusInvAmountPaid)
                            {
                                responseMessage = "the payment is greater than the partial amount remaining";
                            }
                            else
                            {
                                if (invAmountMinusInvAmountPaid == payment.Amount)
                                {
                                    inv = AddPayment(inv, payment, inv.AmountPaid += payment.Amount);
                                    responseMessage = "final partial payment received, invoice is now fully paid";
                                }
                                else
                                {
                                    inv = AddPayment(inv, payment, inv.AmountPaid += payment.Amount);
                                    responseMessage = "another partial payment received, still not fully paid";
                                }
                            }
                        }
                    }
                    else
                    {
                        if (payment.Amount > inv.Amount)
                        {
                            responseMessage = "the payment is greater than the invoice amount";
                        }
                        else if (inv.Amount == payment.Amount)
                        {
                            inv = AddPayment(inv, payment, payment.Amount);
                            responseMessage = "invoice is now fully paid";
                        }
                        else
                        {
                            inv = AddPayment(inv, payment, payment.Amount);
                            responseMessage = "invoice is now partially paid";
                        }
                    }
                }

            }
            catch (InvalidOperationException ex) when (inv.Payments != null || inv.Payments.Any())
            {
                return responseMessage = ex.Message;
            }
            catch (InvalidOperationException ex) when (inv == null)
            {
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            inv.Save();

            return responseMessage;
        }
        /// <summary>
        /// Add payement to process
        /// </summary>
        /// <param name="inv"></param>
        /// <param name="payment"></param>
        /// <param name="paymentAmount"></param>
        /// <returns></returns>
        private Invoice AddPayment(Invoice inv,Payment payment, decimal paymentAmount)
        {
            inv.AmountPaid = paymentAmount;
            inv.Payments.Add(payment);
            return inv;
        }
    }
}