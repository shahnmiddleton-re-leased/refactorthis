using System.Collections.Generic;

namespace RefactorThis.Common.Interfaces.Rules
{
    public interface IRule
    {
        List<IRuleAction> Actions { get; set; }
    }
}
