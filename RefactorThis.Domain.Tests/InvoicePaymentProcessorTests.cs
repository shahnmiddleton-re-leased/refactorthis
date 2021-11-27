using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RefactorThis.Persistence;
using RefactorThis.Persistence.Model;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
    public class InvoicePaymentProcessorTests
    {
        readonly Mock<IRepository<Invoice>> repo;

        public InvoicePaymentProcessorTests()
        {
            repo = new Mock<IRepository<Invoice>>();

        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference()
        {
            repo.Setup(library => library.Get(null))
                .Returns<Invoice>(null);

            var paymentProcessor = new InvoicePaymentProcessor(repo.Object);
            var payment = new Payment();
            Assert.Throws<InvalidOperationException>(() => paymentProcessor.ProcessPayment(payment),
                "There is no invoice matching this payment");
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_ZeroInvoiceAmountAndNoPayment()
        {
            repo.Setup(library => library.Get(It.IsAny<string>()))
                .Returns(new Invoice()
                {
                    Amount = 0,
                    AmountPaid = 0,
                    Payments = new List<Payment>(new[]
                    {
                        new Payment() {Amount = 10},
                    })
                });

            var paymentProcessor = new InvoicePaymentProcessor(repo.Object);
            var payment = new Payment();
            Assert.Throws<InvalidOperationException>(() => paymentProcessor.ProcessPayment(payment),
                "The invoice is in an invalid state, it has an amount of 0 and it has payments.");
        }


        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
        {
            repo.Setup(library => library.Get(It.IsAny<string>()))
                .Returns(new Invoice()
                {
                    Amount = 0,
                    AmountPaid = 0,
                    Payments = null
                });


            var paymentProcessor = new InvoicePaymentProcessor(repo.Object);
            var payment = new Payment();
            var (result, success) = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("no payment needed", result);
            Assert.IsFalse(success);

        }



        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
        {
            repo.Setup(library => library.Get(It.IsAny<string>()))
                .Returns(new Invoice()
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
                });
            var paymentProcessor = new InvoicePaymentProcessor(repo.Object);

            var payment = new Payment();

            var (result, success) = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice was already fully paid", result);
            Assert.IsFalse(success);

        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
        {
            repo.Setup(library => library.Get(It.IsAny<string>()))
                .Returns(new Invoice()
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
                });

            var paymentProcessor = new InvoicePaymentProcessor(repo.Object);

            var payment = new Payment()
            {
                Amount = 6
            };

            var (result, success) = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("the payment is greater than the partial amount remaining", result);
            Assert.IsFalse(success);

        }

        [Test]
        public void
            ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
        {
            repo.Setup(library => library.Get(It.IsAny<string>()))
                .Returns(new Invoice()
                {
                    Amount = 5,
                    AmountPaid = 0,
                    Payments = new List<Payment>()
                });
            var paymentProcessor = new InvoicePaymentProcessor(repo.Object);

            var payment = new Payment()
            {
                Amount = 6
            };

            var (result, success) = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("the payment is greater than the invoice amount", result);
            Assert.IsFalse(success);

        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
        {
            repo.Setup(library => library.Get(It.IsAny<string>()))
                .Returns(new Invoice()
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
                });
            var paymentProcessor = new InvoicePaymentProcessor(repo.Object);

            var payment = new Payment()
            {
                Amount = 5
            };

            var (result, success) = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("final partial payment received, invoice is now fully paid", result);
            Assert.IsTrue(success);
        }

        [Test]
        public void
            ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
        {
            repo.Setup(library => library.Get(It.IsAny<string>()))
                .Returns(new Invoice()
                {
                    Amount = 10,
                    AmountPaid = 0,
                    Payments = new List<Payment>() { new Payment() { Amount = 10 } }
                });

            var paymentProcessor = new InvoicePaymentProcessor(repo.Object);

            var payment = new Payment()
            {
                Amount = 10
            };

            var (result, success) = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice was already fully paid", result);
            Assert.IsFalse(success);
        }

        [Test]
        public void
            ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
        {
            repo.Setup(library => library.Get(It.IsAny<string>()))
                .Returns(new Invoice()
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
                });
            var paymentProcessor = new InvoicePaymentProcessor(repo.Object);

            var payment = new Payment()
            {
                Amount = 1
            };

            var (result, success) = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("another partial payment received, still not fully paid", result);
            Assert.IsTrue(success);

        }

        [Test]
        public void
            ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
        {
            repo.Setup(library => library.Get(It.IsAny<string>()))
                .Returns(new Invoice()
                {
                    Amount = 10,
                    AmountPaid = 0,
                    Payments = new List<Payment>()
                });
            var paymentProcessor = new InvoicePaymentProcessor(repo.Object);

            var payment = new Payment()
            {
                Amount = 1
            };

            var (result, success) = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice is now partially paid", result);
            Assert.IsTrue(success);
        }
    }
}
