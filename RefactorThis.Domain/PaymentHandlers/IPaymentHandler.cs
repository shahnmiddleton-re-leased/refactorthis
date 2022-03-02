namespace RefactorThis.Domain.PaymentHandlers
{
    public interface IPaymentHandler
    {
        void Handle(Payload payload);
    }
}
