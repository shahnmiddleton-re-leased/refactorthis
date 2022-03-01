using RefactorThis.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Domain.Tests
{
    public class ModelFactory: IModelFactory
    {
        public Payment GetPaymentObject()
        {
            return new Payment();
        }

        public Invoice GetInvoiceObject(IInventoryRepository inventoryRepository)
        {
            return new Invoice(inventoryRepository);
        }

        public InvoicePaymentProcessor GetInvoicePaymentProcessorObject(IInventoryRepository inventoryRepository)
        {
            return new InvoicePaymentProcessor(inventoryRepository);
        }

        public List<Payment> GetPaymentsObject()
        {
            return new List<Payment>();
        }

        public Payment GetPopulatedPaymentObject(decimal Amount)
        {
            return new Payment { Amount = Amount };
        }

        public List<Payment> GetPopulatedPaymentsObject(params Payment[] payments)
        {
            return new List<Payment> { GetPopulatedPaymentObject(payments[0].Amount) };
            
        }

        public Invoice GetPopulatedInvoiceObject(IInventoryRepository repo, List<Payment> payments, params int[] numbers)
        {
            return new Invoice(repo) { Amount = numbers[0], AmountPaid = numbers[1], Payments = payments };
        }
    }
}
