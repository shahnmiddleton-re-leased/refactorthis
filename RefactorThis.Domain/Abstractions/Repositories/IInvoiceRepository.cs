﻿using RefactorThis.Domain.Entities.Invoice;

namespace RefactorThis.Domain.Abstractions.Repositories
{
    public interface IInvoiceRepository
    {
        Invoice GetInvoice(string reference);

        void SaveInvoice(Invoice invoice);

        void Add(Invoice invoice);
    }
}
