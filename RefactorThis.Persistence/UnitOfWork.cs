using RefactorThis.Domain.Interfaces;

namespace RefactorThis.Persistence
{
    internal class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork()
        {
            this.InvoiceRepository = new InvoiceRepository();
        }

        public IInvoiceRepository InvoiceRepository { get; }

        public void Complete()
        {
            // Save everything and anything.
        }
    }
}
