using RefactorThis.Persistence.Interfaces;
using RefactorThis.Persistence.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Persistence.Classes
{
    public class AccountsRepository : IAccountsRepository
    {
        Invoice _invoice;
        public void Add(Invoice invoice)
        {
            _invoice = invoice;         //Actually it will come from database.. adding here to make it work.
        }

        public Invoice GetInvoice(string reference)
        {
            return _invoice; //Actually it will come from database.. 
        }

        public void SaveInvoice(Invoice invoice)
        {
            //saves the invoice to the database
        }
    }
}
