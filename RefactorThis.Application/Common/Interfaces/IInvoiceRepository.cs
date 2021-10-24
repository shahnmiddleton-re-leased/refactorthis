using System;
using System.Collections.Generic;
using RefactorThis.Domain.Entities;
using System.Threading.Tasks;

namespace RefactorThis.Application.Common.Interfaces
{
    public interface IInvoiceRepository
    {
        public Task<Invoice> GetByIdAsync(string id);
    }
}
