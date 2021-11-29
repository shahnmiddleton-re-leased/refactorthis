namespace RefactorThis.Domain.PaymentChecks
{
    public class PaymentResult
    {
        public string ResponseMessage { get; set; }

        public  bool AddPayment { get; set; }

        public bool HasConditionMet => !string.IsNullOrEmpty( this.ResponseMessage );
    }
}
