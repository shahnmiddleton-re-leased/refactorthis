using System.Collections.Generic;

namespace Invoicing.Domain.Rules
{
    public abstract class AbstractRuleEngine<T>
    {
        public RuleResult Run(T command)
        {
            foreach (var rule in GetRules())
            {
                if (!rule.IsSatisfied(command))
                {
                    continue;
                }

                return new RuleResult(
                    rule.IsValid(command, out string message),
                    message);
            }

            return new RuleResult(true, null);
        }

        // TODO: we can take this further and use IoC & generics to retrieve the rules
        public abstract IEnumerable<IRule<T>> GetRules();
    }
}