using RefactorThis.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Domain.Tests
{
    public interface IModelFactory
    {
        Payment GetPaymentObject();

        List<Payment> GetPaymentsObject();

        Invoice GetInvoiceObject(IInventoryRepository inventoryRepository);

        InvoicePaymentProcessor GetInvoicePaymentProcessorObject(IInventoryRepository inventoryRepository);

        Payment GetPopulatedPaymentObject(decimal Amount);
        List<Payment> GetPopulatedPaymentsObject(params Payment[] payments);
        Invoice GetPopulatedInvoiceObject(IInventoryRepository repo, List<Payment> payments, params int[] numbers);
    }
}
