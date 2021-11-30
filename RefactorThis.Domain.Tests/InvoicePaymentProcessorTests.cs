using NUnit.Framework;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
    public class InvoicePaymentProcessorTests
    {
        [OneTimeSetUp]
        public void InitialiseRepository()
        {
            var invoice = new Invoice("ABC", 100M);

            InvoiceRepository.Instance.SaveInvoice(invoice);
        }

        [Test]
        public void ProcessPayment_ForNonExistingInvoiceNumber_ReturnsNotPaidNoMatchingInvoice()
        {
            var result = InvoicePaymentProcessor.Process("InvalidInvoiceNumber", new Payment(10M, "payment"));

            Assert.AreEqual(InvoicePaymentResult.NotPaid_NoMatchingInvoice, result);
        }

        [Test]
        public void ProcessPayment_ForZeroAmountInvoice_ReturnsNotPaidNoPaymentNeeded()
        {
            var invoice = new Invoice("NoPaymentNeeded", 0M);
            InvoiceRepository.Instance.SaveInvoice(invoice);

            var result = InvoicePaymentProcessor.Process(invoice.InvoiceNumber, new Payment(0M, "payment"));

            Assert.AreEqual(InvoicePaymentResult.NotPaid_NoPaymentNeeded, result);
        }

        [Test]
        public void ProcessPayment_ForFullyPaidInvoice_ReturnsNotPaidAlreadyFullyPaid()
        {
            var invoice = new Invoice("MyFullyPaidInvoice", 250M);
            InvoiceRepository.Instance.SaveInvoice(invoice);
            InvoicePaymentProcessor.Process(invoice.InvoiceNumber, new Payment(250M, "FullyPaid"));

            var result = InvoicePaymentProcessor.Process(invoice.InvoiceNumber, new Payment(10M, "Overpaid"));

            Assert.AreEqual(InvoicePaymentResult.NotPaid_AlreadyFullyPaid, result);
        }

        [Test]
        public void ProcessPayment_ForPartiallyPaidInvoiceWithOverpay_ReturnsNotPaidOverpayment()
        {
            var invoice = new Invoice("MyPartiallyPaidInvoice", 120M);
            InvoiceRepository.Instance.SaveInvoice(invoice);
            InvoicePaymentProcessor.Process(invoice.InvoiceNumber, new Payment(60M, "PartiallyPaid"));

            var result = InvoicePaymentProcessor.Process(invoice.InvoiceNumber, new Payment(70M, "Overpaid"));

            Assert.AreEqual(InvoicePaymentResult.NotPaid_Overpayment, result);
        }

        [Test]
        public void
            ProcessPayment_ForUnpaidInvoiceWithOverpay_ReturnsNotPaidOverpayment()
        {
            var invoice = new Invoice("MyUnpaidInvoice", 180M);
            InvoiceRepository.Instance.SaveInvoice(invoice);

            var result = InvoicePaymentProcessor.Process(invoice.InvoiceNumber, new Payment(200M, "Overpaid"));

            Assert.AreEqual(InvoicePaymentResult.NotPaid_Overpayment, result);
        }

        [Test]
        public void ProcessPayment_ForPartialPaidInvoiceFullyPaid_ReturnsPaidInvoiceFullyPaid()
        {
            var invoice = new Invoice("PartialPaymentFullyPaidInvoice", 500M);
            InvoiceRepository.Instance.SaveInvoice(invoice);
            InvoicePaymentProcessor.Process(invoice.InvoiceNumber, new Payment(200M, "PartialPayment"));

            var result = InvoicePaymentProcessor.Process(invoice.InvoiceNumber, new Payment(300M, "Final Payment"));

            Assert.AreEqual(InvoicePaymentResult.Paid_InvoiceFullyPaid, result);
        }

        [Test]
        public void ProcessPayment_ForUnpaidInvoiceFullyPaid_ReturnsPaidInvoiceFullyPaid()
        {
            var invoice = new Invoice("NoPaymentFullyPaidInvoice", 600M);
            InvoiceRepository.Instance.SaveInvoice(invoice);

            var result = InvoicePaymentProcessor.Process(invoice.InvoiceNumber, new Payment(600M, "FullPayment"));

            Assert.AreEqual(InvoicePaymentResult.Paid_InvoiceFullyPaid, result);
        }

        [Test]
        public void ProcessPayment_ForPartialPaidInvoicePartiallyPaid_ReturnsPaidPartialAmountRemaining()
        {
            var invoice = new Invoice("PartialPaymentPartiallyPaidInvoice", 1000M);
            InvoiceRepository.Instance.SaveInvoice(invoice);
            InvoicePaymentProcessor.Process(invoice.InvoiceNumber, new Payment(500M, "FirstPayment"));

            var result = InvoicePaymentProcessor.Process(invoice.InvoiceNumber, new Payment(250M, "SecondPayment"));

            Assert.AreEqual(InvoicePaymentResult.Paid_PartialAmountRemaining, result);
        }

        [Test]
        public void ProcessPayment_ForUnpaidInvoicePartiallyPaid_ReturnsPaidPartialAmountRemaining()
        {
            var invoice = new Invoice("NoPaymentPartiallyPaidInvoice", 30M);
            InvoiceRepository.Instance.SaveInvoice(invoice);

            var result = InvoicePaymentProcessor.Process(invoice.InvoiceNumber, new Payment(10M, "Deposit"));

            Assert.AreEqual(InvoicePaymentResult.Paid_PartialAmountRemaining, result);
        }
    }
}