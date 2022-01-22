using RefactorThis.Domain.Entities;
using System.Threading.Tasks;

namespace RefactorThis.Application.Common.Interfaces
{
    public interface IPaymentRepository
    {
        public Task<Payment> SavePaymentAsync(Payment payment);
    }
}
