using RefactorThis.Common.Interfaces.Rules;
using RefactorThis.Invoicing.Domain.Interfaces.Rules;
using RefactorThis.Invoicing.Domain.Model;
using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Invoicing.Domain.Services.Rules.Actions
{
    public class ProcessPaymentValidator : IRuleAction
    {
        public void Process(IRuleContext context)
        {
            var processPaymentRuleContext = (IProcessPaymentRuleContext)context;
            var invoice = processPaymentRuleContext.Invoice;
            var payment = processPaymentRuleContext.Payment;
			context.Validations.AddRange(ValidateProcessPayment(invoice, payment));
		}

        internal List<IRuleValidation> ValidateProcessPayment(Invoice inv, Payment payment)
        {
            var validationList = new List<IRuleValidation>();

            if (inv == null)
            {
                validationList.Add(InvoicingMessages.Validations["INV00001"]);
                return validationList;
            }
            
            if(inv.Amount == 0)
            {
                if (inv.Payments == null || !inv.Payments.Any())
                {
                    validationList.Add(InvoicingMessages.Validations["INV00002"]);
                }
                else
                {
                    validationList.Add(InvoicingMessages.Validations["INV00003"]);                    
                }
            }
			else
			{
				if (inv.Payments != null && inv.Payments.Any())
				{
					if (inv.Payments.Sum(x => x.Amount) != 0 && inv.Amount == inv.Payments.Sum(x => x.Amount))
					{
						validationList.Add(InvoicingMessages.Validations["INV00004"]);
					}
					else if (inv.Payments.Sum(x => x.Amount) != 0 && payment.Amount > (inv.Amount - inv.AmountPaid))
					{
						validationList.Add(InvoicingMessages.Validations["INV00005"]);						
					}					
				}
				else
				{
					if (payment.Amount > inv.Amount)
					{
						validationList.Add(InvoicingMessages.Validations["INV00006"]);
					}					
				}
			}
			return validationList;
        }
    }
}
