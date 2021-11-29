using RefactorThis.Domain.Entities.Invoices;

namespace RefactorThis.Domain.Interfaces
{
    /// <summary>
    /// Manages invoice storage - could include a base class for base repository CRUD functions.
    /// </summary>
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        /// <summary>
        /// Finds an invoice by a reference.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns>An invoice if found.</returns>
        Invoice FindInvoiceByReference(string reference);
    }
}
