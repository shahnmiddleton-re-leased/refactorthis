namespace RefactorThis.Domain.PaymentProcessor.Validators
{
    public interface IPaymentValidatorAbstractFactory
    {
        IPaymentValidator GetFirstPaymentValidator();
        IPaymentValidator GetAdditionalPaymentValidator();
    }
}