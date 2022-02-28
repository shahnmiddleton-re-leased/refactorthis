using System;
using System.Collections.Generic;
using NUnit.Framework;
using RefactorThis.Domain.Entities.Invoice;
using RefactorThis.Domain.Entities.Invoice.Events;
using RefactorThis.Domain.Entities.Invoice.Exceptions;
using EntitiesInvoice = RefactorThis.Domain.Entities.Invoice;

namespace RefactorThis.Domain.Tests.Entities.Invoice
{
    [TestFixture]
    public class InvoiceTests
    {
        private EntitiesInvoice.Invoice _target;

        [Test]
        public void Instantiate_Should_ThrowAnException_When_InvoiceIsZero()
        {
            void Action() => _target = new EntitiesInvoice.Invoice(0, 100, new List<Payment> {
                new Payment(100, "ref") });

            var ex = Assert.Throws<ArgumentException>(Action);
            Assert.That(ex.Message, Is.EqualTo("Amount must be greater than zero."));
        }

        [Test]
        public void Instantiate_Should_ThrowAnException_When_PaymentsNotEqualToAmountPaid()
        {
            void Action() => _target = new EntitiesInvoice.Invoice(200, 100, new List<Payment> {
                new Payment(100, "ref"), new Payment(80, "ref") });

            var ex = Assert.Throws<ArgumentException>(Action);
            Assert.That(ex.Message, Is.EqualTo("Total payments must equal amount paid."));
        }

        [Test]
        public void Instantiate_Should_ThrowAnException_When_PaymentsGreaterThanAmount()
        {
            void Action() => _target = new EntitiesInvoice.Invoice(50, 100, new List<Payment> {
                new Payment(100, "ref") });

            var ex = Assert.Throws<ArgumentException>(Action);
            Assert.That(ex.Message, Is.EqualTo("Total payments must be less than or equal to amount."));
        }

        [Test]
        public void ProcessPayment_Should_ThrowAnException_When_PaymentIsZero()
        {
            _target = new EntitiesInvoice.Invoice(100, 50, new List<Payment> {
                new Payment(50, "ref") });

            var newPayment = new Payment(0, "ref");

            void Action() => _target.ProcessPayment(newPayment);

            var ex = Assert.Throws<ArgumentException>(Action);
            Assert.That(ex.Message, Is.EqualTo("Payment amount must be greater than zero."));
        }

        [Test]
        public void ProcessPayment_Should_ThrowAnException_When_InvoiceAlreadyFullyPaid()
        {
            _target = new EntitiesInvoice.Invoice(100, 100, new List<Payment> {
                new Payment(100, "ref") });

            var newPayment = new Payment(10, "ref");

            void Action() => _target.ProcessPayment(newPayment);

            var ex = Assert.Throws<NoPaymentRequiredException>(Action);
        }

        [Test]
        public void ProcessPayment_Should_ThrowAnException_When_PartialPaymentIsMoreThanAmountDue()
        {
            _target = new EntitiesInvoice.Invoice(100, 50, new List<Payment> {
                new Payment(50, "ref") });

            var newPayment = new Payment(51, "ref");

            void Action() => _target.ProcessPayment(newPayment);

            var ex = Assert.Throws<PartialPaymentTooMuchException>(Action);
        }

        [Test]
        public void ProcessPayment_Should_ThrowAnException_When_FullPaymentIsMoreThanAmountDue()
        {
            _target = new EntitiesInvoice.Invoice(100, 0, new List<Payment>());

            var newPayment = new Payment(101, "ref");

            void Action() => _target.ProcessPayment(newPayment);

            var ex = Assert.Throws<FullPaymentTooMuchException>(Action);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_FirstPaymentProcessedAndNotPaidInFull()
        {
            _target = new EntitiesInvoice.Invoice(100, 0, new List<Payment>());

            var newPayment = new Payment(50, "ref");

            var actual = _target.ProcessPayment(newPayment);

            Assert.IsInstanceOf<PaymentProcessed>(actual);
            Assert.IsTrue(((PaymentProcessed)actual).PaymentCount == 1);
            Assert.IsFalse(((PaymentProcessed)actual).IsPaidInFull);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_FirstPaymentProcessedAndPaidInFull()
        {
            _target = new EntitiesInvoice.Invoice(100, 0, new List<Payment>());

            var newPayment = new Payment(100, "ref");

            var actual = _target.ProcessPayment(newPayment);

            Assert.IsInstanceOf<PaymentProcessed>(actual);
            Assert.IsTrue(((PaymentProcessed)actual).PaymentCount == 1);
            Assert.IsTrue(((PaymentProcessed)actual).IsPaidInFull);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_SubsequentPaymentProcessedAndNotPaidInFull()
        {
            _target = new EntitiesInvoice.Invoice(100, 20, new List<Payment> {
                new Payment(20, "ref") });

            var newPayment = new Payment(50, "ref");

            var actual = _target.ProcessPayment(newPayment);

            Assert.IsInstanceOf<PaymentProcessed>(actual);
            Assert.IsFalse(((PaymentProcessed)actual).PaymentCount == 1);
            Assert.IsFalse(((PaymentProcessed)actual).IsPaidInFull);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_SubsequentPaymentProcessedAndPaidInFull()
        {
            _target = new EntitiesInvoice.Invoice(100, 20, new List<Payment> {
                new Payment(20, "ref") });

            var newPayment = new Payment(80, "ref");

            var actual = _target.ProcessPayment(newPayment);

            Assert.IsInstanceOf<PaymentProcessed>(actual);
            Assert.IsFalse(((PaymentProcessed)actual).PaymentCount == 1);
            Assert.IsTrue(((PaymentProcessed)actual).IsPaidInFull);
        }
    }
}
