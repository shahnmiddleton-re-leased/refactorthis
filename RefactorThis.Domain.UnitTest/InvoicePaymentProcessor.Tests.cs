using NUnit.Framework;
using RefactorThis.Domain.Constants;
using RefactorThis.Domain.PaymentRule;
using RefactorThis.Persistence.Model;
using System;
using System.Collections.Generic;

namespace RefactorThis.Domain.UnitTest
{
    public class InvoicePaymentProcessorTests
    {
        [Test]
        public void ProcessPayment_Throws_Error_InvalidStateInvoice()
        {
            var invoice = new Invoice()
            {
                Amount = 0,
                AmountPaid = 0,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 100
                    }
                }
            };

            var payment = new Payment();
            var paymentRuleRepository = new PaymentRuleRepository();

            Assert.Throws<InvalidOperationException>(() => paymentRuleRepository.ProcessPayment(invoice, payment),
                PaymentMessage.InvalidStateInvoice);
        }

        [Test]
        public void ProcessPayment_Throws_Error_InvalidInvoice()
        {
            var payment = new Payment();
            var paymentRuleRepository = new PaymentRuleRepository();

            Assert.Throws<InvalidOperationException>(() => paymentRuleRepository.ProcessPayment(null, payment),
                PaymentMessage.InvalidInvoice);
        }


        [Test]
        public void ProcessPayment_Error_NoPaymentRequired()
        {
            var invoice = new Invoice()
            {
                Amount = 0,
                AmountPaid = 0
            };
            var payment = new Payment();
            var paymentRuleRepository = new PaymentRuleRepository();
            var serviceResponse = paymentRuleRepository.ProcessPayment(invoice, payment);
            Assert.AreEqual(PaymentMessage.NoPaymentRequired, serviceResponse.Message);
            Assert.IsTrue(serviceResponse.IsErrorExist);
        }

        [Test]
        public void ProcessPayment_Error_InvoiceAlreadyPaid()
        {
            var invoice = new Invoice()
            {
                Amount = 100,
                AmountPaid = 100,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 100
                    }
                }
            };
            var payment = new Payment();
            var paymentRuleRepository = new PaymentRuleRepository();
            var serviceResponse = paymentRuleRepository.ProcessPayment(invoice, payment);
            Assert.AreEqual(PaymentMessage.InvoiceAlreadyPaid, serviceResponse.Message);
            Assert.IsTrue(serviceResponse.IsErrorExist);
        }

        [Test]
        public void ProcessPayment_Error_PaymentHighThanPartialAmount()
        {
            var invoice = new Invoice()
            {
                Amount = 100,
                AmountPaid = 50,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 50
                    }
                }
            };
            var payment = new Payment()
            {
                Amount = 80
            };
            var paymentRuleRepository = new PaymentRuleRepository();
            var serviceResponse = paymentRuleRepository.ProcessPayment(invoice, payment);
            Assert.AreEqual(PaymentMessage.PaymentHighThanPartialAmount, serviceResponse.Message);
            Assert.IsTrue(serviceResponse.IsErrorExist);
        }

        [Test]
        public void ProcessPayment_Error_PaymentGreaterThanInvoice()
        {
            var invoice = new Invoice()
            {
                Amount = 100,
                AmountPaid = 0
            };
            var payment = new Payment()
            {
                Amount = 150
            };
            var paymentRuleRepository = new PaymentRuleRepository();
            var serviceResponse = paymentRuleRepository.ProcessPayment(invoice, payment);
            Assert.AreEqual(PaymentMessage.PaymentGreaterThanInvoice, serviceResponse.Message);
            Assert.IsTrue(serviceResponse.IsErrorExist);
        }

        [Test]
        public void ProcessPayment_CompletePayment()
        {
            var invoice = new Invoice()
            {
                Amount = 100,
                AmountPaid = 50,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 50
                    }
                }
            };
            var payment = new Payment()
            {
                Amount = 50
            };
            var paymentRuleRepository = new PaymentRuleRepository();
            var serviceResponse = paymentRuleRepository.ProcessPayment(invoice, payment);
            Assert.AreEqual(PaymentMessage.CompletePayment, serviceResponse.Message);
        }


        [Test]
        public void ProcessPayment_Error_InvoiceAlreadyPaid_V2()
        {
            var invoice = new Invoice()
            {
                Amount = 100,
                AmountPaid = 0,
                Payments = new List<Payment>() { new Payment() { Amount = 100 } }
            };
            var payment = new Payment()
            {
                Amount = 100
            };
            var paymentRuleRepository = new PaymentRuleRepository();
            var serviceResponse = paymentRuleRepository.ProcessPayment(invoice, payment);
            Assert.AreEqual(PaymentMessage.InvoiceAlreadyPaid, serviceResponse.Message);
            Assert.IsTrue(serviceResponse.IsErrorExist);
        }

        [Test]
        public void ProcessPayment_PartialPaymentComplete()
        {
            var invoice = new Invoice()
            {
                Amount = 100,
                AmountPaid = 10,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 10
                    }
                }
            };
            var payment = new Payment()
            {
                Amount = 5
            };
            var paymentRuleRepository = new PaymentRuleRepository();
            var serviceResponse = paymentRuleRepository.ProcessPayment(invoice, payment);
            Assert.AreEqual(PaymentMessage.PartialPaymentComplete, serviceResponse.Message);
        }

        [Test]
        public void ProcessPayment_InvoicePartiallyPaid()
        {
            var invoice = new Invoice()
            {
                Amount = 100,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            var payment = new Payment()
            {
                Amount = 10
            };
            var paymentRuleRepository = new PaymentRuleRepository();
            var serviceResponse = paymentRuleRepository.ProcessPayment(invoice, payment);
            Assert.AreEqual(PaymentMessage.InvoicePartiallyPaid, serviceResponse.Message);
        }

        [Test]
        public void ProcessPayment_InvoicePaymentComplete()
        {
            var invoice = new Invoice()
            {
                Amount = 5,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            var payment = new Payment()
            {
                Amount = 5
            };
            var paymentRuleRepository = new PaymentRuleRepository();
            var serviceResponse = paymentRuleRepository.ProcessPayment(invoice, payment);
            Assert.AreEqual(PaymentMessage.InvoicePaymentComplete, serviceResponse.Message);
        }
    }
}