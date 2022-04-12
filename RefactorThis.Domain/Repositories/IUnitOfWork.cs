using System;

namespace RefactorThis.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IInvoiceRepository Invoices { get; }

        void Save();
    }
}
