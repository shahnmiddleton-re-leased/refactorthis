
namespace RefactorThis.Domain.Interfaces
{
    /// <summary>
    /// Manages repositories and transactions.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// The invoice repository.
        /// </summary>
        IInvoiceRepository InvoiceRepository { get; }

        /// <summary>
        /// Commit repository transactions after a unit of work is complete.
        /// </summary>
        void Complete();
    }
}
