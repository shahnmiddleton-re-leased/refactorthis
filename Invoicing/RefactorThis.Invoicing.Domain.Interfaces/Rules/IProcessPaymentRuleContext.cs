using RefactorThis.Common.Interfaces.Rules;
using RefactorThis.Invoicing.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RefactorThis.Invoicing.Domain.Interfaces.Rules
{
    public interface IProcessPaymentRuleContext : IRuleContext
    {
        Invoice Invoice { get; set; }
        Payment Payment { get; set; }
    }
}
