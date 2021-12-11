using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
    public class InvoicePaymentProcessorTests
    {
        private IInvoiceRepository _invoiceRepository;
        private IInvoicePaymentProcessor _invoicePaymentProcessor;
        private IList<Payment> _invoicePaymentsList;

        [SetUp]
        public void InitializeTest()
        {
            _invoiceRepository = new InvoiceRepository();
            _invoicePaymentProcessor = new InvoicePaymentProcessor(_invoiceRepository);
            _invoicePaymentsList = new List<Payment>();
        }

        public void CreateInvoice(decimal amount)
        {
            var invoice = new Invoice(_invoiceRepository)
            {
                Amount = amount,
            };

            foreach (var payment in _invoicePaymentsList)
            {
                invoice.AddPayment(payment);
            }

            _invoiceRepository.Add(invoice);
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference()
        {
            var payment = new Payment();
            var failureMessage = "";

            try
            {
                _invoicePaymentProcessor.ProcessPayment(payment);
            }
            catch (InvalidOperationException e)
            {
                failureMessage = e.Message;
            }

            Assert.AreEqual("There is no invoice matching this payment", failureMessage);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
        {
            CreateInvoice(0);
            var payment = new Payment();

            var result = _invoicePaymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("no payment needed", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
        {
            _invoicePaymentsList.Add(
                new Payment
                {
                    Amount = 10
                });
            CreateInvoice(10);

            var payment = new Payment();

            var result = _invoicePaymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice was already fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
        {
            _invoicePaymentsList.Add(
                new Payment
                {
                    Amount = 5
                });
            CreateInvoice(10);

            var payment = new Payment()
            {
                Amount = 6
            };

            var result = _invoicePaymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("the payment is greater than the amount remaining", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
        {
            CreateInvoice(5);

            var payment = new Payment()
            {
                Amount = 6
            };

            var result = _invoicePaymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("the payment is greater than the invoice amount", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
        {
            _invoicePaymentsList.Add(
                new Payment
                {
                    Amount = 5
                });
            CreateInvoice(10);

            var payment = new Payment()
            {
                Amount = 5
            };

            var result = _invoicePaymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice is now fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
        {
            _invoicePaymentsList.Add(
                new Payment
                {
                    Amount = 10
                });
            CreateInvoice(10);

            var payment = new Payment()
            {
                Amount = 10
            };

            var result = _invoicePaymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice was already fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
        {
            _invoicePaymentsList.Add(
                new Payment
                {
                    Amount = 5
                });
            CreateInvoice(10);

            var payment = new Payment()
            {
                Amount = 1
            };

            var result = _invoicePaymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice is now partially paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
        {
            CreateInvoice(10);

            var payment = new Payment()
            {
                Amount = 1
            };

            var result = _invoicePaymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice is now partially paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_PaymentArgumentIsNull()
        {
            var failureMessage = "";

            try
            {
                _invoicePaymentProcessor.ProcessPayment(null);
            }
            catch (ArgumentNullException e)
            {
                failureMessage = e.Message;
            }

            Assert.AreEqual("parameter cannot be null\r\nParameter name: payment", failureMessage);
        }
    }
}