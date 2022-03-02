namespace RefactorThis.Persistence.Payment
{
    public class AdditionalPayment : IPayment
    {
        public decimal Amount { get; set; }
        public string Reference { get; set; }

        private Invoice _invoice;

        public AdditionalPayment(Invoice invoice)
        {
            this._invoice = invoice;
        }

        public string ProcessPayment(decimal amount)
        {
            this.Amount = amount;
            bool isFinalPartialPayment = (this._invoice.Amount - this._invoice.AmountPaid) == this.Amount;
            string responseMessage = isFinalPartialPayment ? "final partial payment received, invoice is now fully paid" : "another partial payment received, still not fully paid";
            this._invoice.AmountPaid += this.Amount;
            this._invoice.Payments.Add(this);

            return responseMessage;
        }
    }
}
