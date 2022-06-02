using RefactorThis.Domain.Model;
using RefactorThis.Domain.Services;
using System;
using System.Linq;

namespace RefactorThis.Domain
{
    public abstract class AbstractInvoicePaymentProcessor : IInvoicePaymentProcessor
    {
        private readonly IRepository<Invoice> _invoiceRepository;

        public static readonly string MSG_NO_MATCHING_INVOICE_PAYMENT = "MSG_NO_MATCHING_INVOICE_PAYMENT";
        public static readonly string MSG_NO_PAYMENT_NEED = "MSG_NO_PAYMENT_NEED";
        public static readonly string MSG_INVOICE_INVALID_STAT_AMNT_0_HAS_PAYMENTS = "MSG_INVOICE_INVALID_STAT_AMNT_0_HAS_PAYMENTS";
        public static readonly string MSG_INVOICE_PAID_FULLY = "MSG_INVOICE_PAID_FULLY";
        public static readonly string MSG_PAYMENT_LARGER_THAN_REMAIN = "MSG_PAYMENT_LARGER_THAN_REMAIN";
        public static readonly string MSG_FINAL_PAY_INV_FULLY_PAID = "MSG_FINAL_PAY_INV_FULLY_PAID";
        public static readonly string MSG_PAY_RCVD_NOT_FULLY_PAID = "MSG_PAY_RCVD_NOT_FULLY_PAID";
        public static readonly string MSG_PAY_LARGER_THAN_INV_AMT = "MSG_PAY_LARGER_THAN_INV_AMT";
        public static readonly string MSG_INV_FULLY_PAID = "MSG_INV_FULLY_PAID";
        public static readonly string MSG_INV_PARTLY_PAID = "MSG_INV_PARTLY_PAID";

        protected abstract void InitializeMessages();

        public AbstractInvoicePaymentProcessor(IRepository<Invoice> invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
            InitializeMessages();
        }

        public virtual string ProcessPayment(Payment payment)
        {
            var inv = _invoiceRepository.Get(payment.Reference);

            var responseMessage = string.Empty;

            if (inv == null)
            {
                throw new InvalidOperationException(MSG_NO_MATCHING_INVOICE_PAYMENT);
            }
            else
            {
                if (inv.Amount == 0)
                {
                    if (inv.Payments == null || !inv.Payments.Any())
                    {
                        responseMessage = MSG_NO_PAYMENT_NEED;
                    }
                    else
                    {
                        throw new InvalidOperationException(MSG_INVOICE_INVALID_STAT_AMNT_0_HAS_PAYMENTS);
                    }
                }
                else
                {
                    if (inv.Payments != null && inv.Payments.Any())
                    {
                        if (inv.Payments.Sum(x => x.Amount) != 0 && inv.Amount == inv.Payments.Sum(x => x.Amount))
                        {
                            responseMessage = MSG_INVOICE_PAID_FULLY;
                        }
                        else if (inv.Payments.Sum(x => x.Amount) != 0 && payment.Amount > (inv.Amount - inv.AmountPaid))
                        {
                            responseMessage = MSG_PAYMENT_LARGER_THAN_REMAIN;
                        }
                        else
                        {
                            if ((inv.Amount - inv.AmountPaid) == payment.Amount)
                            {
                                responseMessage = MSG_FINAL_PAY_INV_FULLY_PAID;
                            }
                            else
                            {
                                responseMessage = MSG_PAY_RCVD_NOT_FULLY_PAID;
                            }
                            inv.AmountPaid += payment.Amount;
                            inv.Payments.Add(payment);
                        }
                    }
                    else
                    {
                        if (payment.Amount > inv.Amount)
                        {
                            responseMessage = MSG_PAY_LARGER_THAN_INV_AMT;
                        }
                        else
                        {
                            if (inv.Amount == payment.Amount)
                            {
                                responseMessage = MSG_INV_FULLY_PAID;
                            }
                            else
                            {
                                responseMessage = MSG_INV_PARTLY_PAID;
                            }
                            inv.AmountPaid = payment.Amount;
                            inv.Payments.Add(payment);
                        }
                    }
                }
            }

            _invoiceRepository.Save(inv);

            return responseMessage;
        }
    }
}