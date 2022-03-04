using RefactorThis.Persistence.Interfaces;
using RefactorThis.Persistence.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Persistence.Classes
{
    public class FinancialsService : IFinancialsService
    {
        private readonly IAccountsRepository _accountsRepository;
        public FinancialsService(IAccountsRepository accountsRepository)
        {
            this._accountsRepository = accountsRepository;
        }
        public Invoice GetInvoice(string reference) => _accountsRepository.GetInvoice(reference);        
        public void SaveInvoice(Invoice invoice) => _accountsRepository.SaveInvoice(invoice);
        public void Add(Invoice invoice) => _accountsRepository.Add(invoice);
    }
}
