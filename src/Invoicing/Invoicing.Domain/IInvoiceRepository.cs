namespace Invoicing.Domain
{
    public interface IInvoiceRepository
    {
        Invoice Get(string reference);

        void Update(Invoice invoice);

        void Add(Invoice invoice);
    }
}
