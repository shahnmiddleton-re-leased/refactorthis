using RefactorThis.Domain.Helpers;
using RefactorThis.Persistence;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RefactorThis.Domain.Services
{
    public interface IInvoicePaymentService
    {
        public Task<string> ProcessPayment(Payment payment);
    }
    public class InvoicePaymentService : IInvoicePaymentService
    {
        private InvoiceRepository Repository;

        public InvoicePaymentService(InvoiceRepository repository)
        {
            Repository = repository;
        }

        /// <summary>
        /// Processes a payment for an invoice
        /// </summary>
        /// <param name="payment">The payment to process</param>
        /// <returns>A response of the payment processed</returns>
        public async Task<string> ProcessPayment(Payment payment)
        {
            var inv = await Repository.GetInvoice(payment.Reference);

            string error = string.Empty;
            string response = string.Empty;

            if (inv == null)
                error = Resources.Strings.NoInvoice;
            else
            {
                if (inv.Amount == 0)
                {
                    if (inv.Payments == null || !inv.Payments.Any())
                        response = Resources.Strings.NoPaymentRequired;
                    else
                        error = Resources.Strings.InvalidInvoice;
                }
                else
                {
                    if (inv.Payments?.Any() == true)
                    {
                        decimal sumPayments = inv.Payments.Sum(x => x.Amount);
                        bool paymentsExisting = sumPayments != 0;

                        if (paymentsExisting && inv.Amount == sumPayments)
                            response = Resources.Strings.InvoiceAlreadyPaid;
                        else if (paymentsExisting && payment.Amount > (inv.Amount - inv.AmountPaid))
                            response = Resources.Strings.PaymentGreaterThanPartialRemaining;
                        else
                        {
                            if ((inv.Amount - inv.AmountPaid) == payment.Amount)
                            {
                                inv.AmountPaid += payment.Amount;
                                inv.Payments.Add(payment);
                                response = Resources.Strings.FinalPaymentReceived;
                            }
                            else
                            {
                                inv.AmountPaid += payment.Amount;
                                inv.Payments.Add(payment);
                                response = Resources.Strings.AnotherPaymentReceived;     
                            }
                        }
                    }
                    else
                    {
                        if (payment.Amount > inv.Amount)
                            response = Resources.Strings.PaymentGreaterThanAmount;
                        else
                        {
                            if (inv.Amount == payment.Amount)
                            {
                                inv.AmountPaid = payment.Amount;
                                inv.Payments.Add(payment);
                                response = Resources.Strings.InvoiceFullyPaid;
                            }
                            else
                            {
                                inv.AmountPaid = payment.Amount;
                                inv.Payments.Add(payment);
                                response = Resources.Strings.InvoicePartiallyPaid;
                            }
                        }
                    }
                }
            }

            try
            {
                await inv.Save(inv);
            }
            catch (Exception ex)
            {
                Logger.ELog(ex.ToString());
            }

            if (!string.IsNullOrEmpty(error))
            {
                Logger.ELog(error);
                throw new InvalidOperationException(error);
            }
            else
            {
                Logger.ILog(response);

                return response;
            }
        }
    }
}
