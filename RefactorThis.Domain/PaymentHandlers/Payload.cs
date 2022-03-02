using RefactorThis.Persistence;

namespace RefactorThis.Domain.PaymentHandlers
{
    public class Payload
    {
        public Invoice Invoice { get; set; }
        public Payment Payment { get; set; }
        public string Response { get; set; }
        public bool SkipRestHandlers { get; set; }
        public bool IsValid { get; set; }
        public bool InvoiceAlreadyHasPayment { get; set; }
        public CurrentInvoiceStatus CurrentInvoiceStatus { get; set; }
    }

    public enum CurrentInvoiceStatus
    {
        Default,
        FullyPaid,
        PartialPaid,
        ExceedInvoiceAmount
    }
}
