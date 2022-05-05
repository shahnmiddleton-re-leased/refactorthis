namespace RefactorThis.Domain.InternationalizationService
{
    public abstract record TranslationKey(string Key);

    public record InvoiceNotFound() : TranslationKey("invoice.not-found");
    public record InvoiceInvalidState() : TranslationKey("invoice.invalid-state");
    public record InvoiceNoPaymentNeeded() : TranslationKey("invoice.no-payment-needed");
    public record InvoiceAlreadyFullyPaid() : TranslationKey("invoice.already-fully-paid");
    public record InvoicePartialPaymentTooBig() : TranslationKey("invoice.partial-payment-too-big");
    public record InvoicePaymentTooBig() : TranslationKey("invoice.payment-too-big");
    public record InvoiceFinalPaymentPaid() : TranslationKey("invoice.final-payment-paid");
    public record InvoiceFullyPaid() : TranslationKey("invoice.fully-paid");
    public record InvoicePartialPaymentReceived() : TranslationKey("invoice.partial-payment-received");
    public record InvoicePartiallyPaid() : TranslationKey("invoice.partially-paid");


}