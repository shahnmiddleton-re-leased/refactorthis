﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Refactor.Process.InvoicePaymentProcessing;
using RefactorThis.Domain.Model;
using RefactorThis.Persistence;

namespace RefactorThis.Tests
{
    [TestFixture]
    public class InvoicePaymentProcessorTests
    {
        private Repository<Invoice> _invoiceRepository;
        private InvoicePaymentProcessor _paymentProcessor;

        [SetUp]
        public void SetUp()
        {
            _invoiceRepository = new Repository<Invoice>();
            _paymentProcessor = new InvoicePaymentProcessor(_invoiceRepository);
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference()
        {
            var payment = new Payment(Guid.NewGuid().ToString());
            var failureMessage = "";

            try
            {
                var result = _paymentProcessor.ProcessPayment(payment);
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
            var invId = Guid.NewGuid().ToString();
            var invoice = new Invoice(invId)
            {
                Amount = 0,
                AmountPaid = 0,
                Payments = null
            };

            _invoiceRepository.Add(invoice);

            var payment = new Payment(invId);

            var result = _paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("no payment needed", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
        {
            var invId = Guid.NewGuid().ToString();
            var invoice = new Invoice(invId)
            {
                Amount = 10,
                AmountPaid = 10,
                Payments = new List<Payment>
                {
                    new Payment(invId)
                    {
                        Amount = 10
                    }
                }
            };
            _invoiceRepository.Add(invoice);

            var payment = new Payment(invId);
            var result = _paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice was already fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
        {
            var invId = Guid.NewGuid().ToString();
            var invoice = new Invoice(invId)
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>
                {
                    new Payment(invId)
                    {
                        Amount = 5
                    }
                }
            };
            _invoiceRepository.Add(invoice);

            var payment = new Payment(invId)
            {
                Amount = 6
            };

            var result = _paymentProcessor.ProcessPayment(payment);
            Assert.AreEqual("the payment is greater than the partial amount remaining", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
        {
            var invId = Guid.NewGuid().ToString();
            var invoice = new Invoice(invId)
            {
                Amount = 5,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            _invoiceRepository.Add(invoice);

            var payment = new Payment(invId)
            {
                Amount = 6
            };

            var result = _paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("the payment is greater than the invoice amount", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
        {
            var invId = Guid.NewGuid().ToString();
            var invoice = new Invoice(invId)
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>
                {
                    new Payment(invId)
                    {
                        Amount = 5
                    }
                }
            };
            _invoiceRepository.Add(invoice);

            var payment = new Payment(invId)
            {
                Amount = 5
            };

            var result = _paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("final partial payment received, invoice is now fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
        {
            var invId = Guid.NewGuid().ToString();
            var invoice = new Invoice(invId)
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>() { new Payment(invId) { Amount = 10 } }
            };
            _invoiceRepository.Add(invoice);

            var payment = new Payment(invId)
            {
                Amount = 10
            };

            var result = _paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice was already fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
        {
            var invId = Guid.NewGuid().ToString();
            var invoice = new Invoice(invId)
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>
                {
                    new Payment(invId)
                    {
                        Amount = 5
                    }
                }
            };
            _invoiceRepository.Add(invoice);

            var payment = new Payment(invId)
            {
                Amount = 1
            };

            var result = _paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("another partial payment received, still not fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
        {
            var invId = Guid.NewGuid().ToString();
            var invoice = new Invoice(invId)
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            _invoiceRepository.Add(invoice);

            var payment = new Payment(invId)
            {
                Amount = 1
            };

            var result = _paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice is now partially paid", result);
        }
    }
}