using RefactorThis.Application.Common.Interfaces;
using RefactorThis.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace RefactorThis.Persistence
{
    public class PaymentRepository : IPaymentRepository
    {
        public Task<Payment> SavePaymentAsync(Payment payment)
        {
            throw new NotImplementedException();
        }
    }
}
