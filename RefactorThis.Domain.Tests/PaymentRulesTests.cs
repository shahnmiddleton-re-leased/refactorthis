using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RefactorThis.Domain.PaymentChecks.Rules;
using RefactorThis.Persistence.Entities;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
    public class PaymentRulesTests
    {
        [Test]
        public void PaymentRule_Should_ThrowException_When_InvoiceIsInvalid()
        {
            var paymentRule = new InvalidInvoiceRule();

            var invoice = new Invoice()
            {
                Amount = 0,
                AmountPaid = 0,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 10
                    }
                }
            };

            var payment = new Payment();
            var failureMessage = "";

            try
            {
                paymentRule.RunRule(invoice, payment);
            }
            catch (InvalidOperationException e)
            {
                failureMessage = e.Message;
            }

            Assert.AreEqual("The invoice is in an invalid state, it has an amount of 0 and it has payments.", failureMessage);
        }
        
        [Test]
        public void PaymentRule_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
        {
            var paymentRule = new InvoiceAlreadyFullyPaidRule();

            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 10,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 10
                    }
                }
            };

            var payment = new Payment();

            var result = paymentRule.RunRule(invoice, payment);

            Assert.AreEqual(true, result.HasConditionMet);
            Assert.AreEqual(false, result.AddPayment);
            Assert.AreEqual("invoice was already fully paid", result.ResponseMessage);
        }

        [Test]
        public void PaymentRule_Should_ReturnFailureMessage_When_InvoiceNowFullyPaid()
        {
            var paymentRule = new InvoiceNowFullyPaidRule();

            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };

            var payment = new Payment()
            {
                Amount = 5
            };

            var result = paymentRule.RunRule(invoice, payment);

            Assert.AreEqual(true, result.HasConditionMet);
            Assert.AreEqual(true, result.AddPayment);
            Assert.AreEqual("final partial payment received, invoice is now fully paid", result.ResponseMessage);
        }
        
        [Test]
        public void PaymentRule_Should_ReturnFailureMessage_When_NoPaymentNeeded()
        {
            var paymentRule = new NoPaymentNeededRule();

            var invoice = new Invoice()
            {
                Amount = 0,
                AmountPaid = 0,
                Payments = null
            };

            var payment = new Payment();

            var result = paymentRule.RunRule(invoice, payment);

            Assert.AreEqual(true, result.HasConditionMet);
            Assert.AreEqual(false, result.AddPayment);
            Assert.AreEqual("no payment needed", result.ResponseMessage);
        }

        [Test]
        public void PaymentRule_Should_ReturnFailureMessage_When_NoPaymentsInvoiceIsFullyPaid()
        {
            var paymentRule = new NoPaymentsInvoiceIsFullyPaidRule();

            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };

            var payment = new Payment()
            {
                Amount = 10
            };

            var result = paymentRule.RunRule(invoice, payment);

            Assert.AreEqual(true, result.HasConditionMet);
            Assert.AreEqual(true, result.AddPayment);
            Assert.AreEqual("invoice is now fully paid", result.ResponseMessage);
        }

        [Test]
        public void PaymentRule_Should_ReturnFailureMessage_When_NoPaymentsInvoiceIsPartiallyPaid()
        {
            var paymentRule = new NoPaymentsInvoiceIsPartiallyPaidRule();

            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };

            var payment = new Payment()
            {
                Amount = 1
            };

            var result = paymentRule.RunRule(invoice, payment);

            Assert.AreEqual(true, result.HasConditionMet);
            Assert.AreEqual(true, result.AddPayment);
            Assert.AreEqual("invoice is now partially paid", result.ResponseMessage);
        }

        [Test]
        public void PaymentRule_Should_ReturnFailureMessage_When_PaymentAmountGreaterThanInvoiceAmount()
        {
            var paymentRule = new PaymentAmountGreaterThanInvoiceAmountRule();

            var invoice = new Invoice()
            {
                Amount = 5,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };

            var payment = new Payment()
            {
                Amount = 6
            };

            var result = paymentRule.RunRule(invoice, payment);

            Assert.AreEqual(true, result.HasConditionMet);
            Assert.AreEqual(true, result.AddPayment);
            Assert.AreEqual("the payment is greater than the invoice amount", result.ResponseMessage);
        }

        [Test]
        public void PaymentRule_Should_ReturnFailureMessage_When_PaymentIsGreaterThanRemainingAmount()
        {
            var paymentRule = new PaymentIsGreaterThanRemainingAmountRule();

            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };

            var payment = new Payment()
            {
                Amount = 6
            };

            var result = paymentRule.RunRule(invoice, payment);

            Assert.AreEqual(true, result.HasConditionMet);
            Assert.AreEqual(false, result.AddPayment);
            Assert.AreEqual("the payment is greater than the partial amount remaining", result.ResponseMessage);
        }

        [Test]
        public void PaymentRule_Should_ReturnFailureMessage_When_PaymentReceivedNotFullPaid()
        {
            var paymentRule = new PaymentReceivedNotFullPaidRule();

            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };

            var payment = new Payment()
            {
                Amount = 1
            };

            var result = paymentRule.RunRule(invoice, payment);

            Assert.AreEqual(true, result.HasConditionMet);
            Assert.AreEqual(true, result.AddPayment);
            Assert.AreEqual("another partial payment received, still not fully paid", result.ResponseMessage);
        }
    }
}
