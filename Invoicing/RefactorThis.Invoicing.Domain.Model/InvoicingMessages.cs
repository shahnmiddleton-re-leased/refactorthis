using RefactorThis.Common.Rules;
using System.Collections.Generic;

namespace RefactorThis.Invoicing.Domain.Model
{
    public static class InvoicingMessages
    {
        private static Dictionary<string, RuleValidation> _validations;
        public static Dictionary<string, RuleValidation> Validations => _validations ??= Initialise();

        public static Dictionary<string, RuleValidation> Initialise()
        {
            var data = new Dictionary<string, string>
            {
                {"INV00001", "There is no invoice matching this payment."},
                {"INV00002", "No payment needed."},                
                {"INV00003", "The invoice is in an invalid state, it has an amount of 0 and it has payments."},
                {"INV00004", "Invoice was already fully paid."},
                {"INV00005", "The payment is greater than the partial amount remaining."},
                {"INV00006", "The payment is greater than the invoice amount."},
            };
            var validations = new Dictionary<string, RuleValidation>();
            foreach (var key in data.Keys)
                validations[key] = new RuleValidation
                {
                    Code = key,
                    Message = data[key]
                };

            return validations;
        }
    }
}
