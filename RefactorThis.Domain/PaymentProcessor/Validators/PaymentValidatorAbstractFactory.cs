namespace RefactorThis.Domain.PaymentProcessor.Validators
{
    public class PaymentValidatorAbstractFactory : IPaymentValidatorAbstractFactory
    {
        public IPaymentValidator GetFirstPaymentValidator()
        {
            return new FirstPaymentValidator();
        }

        public IPaymentValidator GetAdditionalPaymentValidator()
        {
            return new AdditionalPaymentValidator();
        }
    }
}