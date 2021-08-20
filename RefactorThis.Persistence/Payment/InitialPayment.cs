namespace RefactorThis.Persistence.Payment
{
    public class InitialPayment : IPayment
    {
        public decimal Amount { get; set; }
        public string Reference { get; set; }

        private Invoice _invoice;

        public InitialPayment(Invoice invoice)
        {
            this._invoice = invoice;
        }

        public string ProcessPayment(decimal amount)
        {
            this.Amount = amount;
            bool isInvoiceFullyPaid = (this._invoice.Amount == this.Amount);
            string responseMessage = isInvoiceFullyPaid ? "invoice is now fully paid" : "invoice is now partially paid";
            this._invoice.AmountPaid = this.Amount;
            this._invoice.Payments.Add(this);

            return responseMessage;
        }
    }
}
