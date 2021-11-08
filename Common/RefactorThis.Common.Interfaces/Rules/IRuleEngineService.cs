
namespace RefactorThis.Common.Interfaces.Rules
{
    public interface IRuleEngineService
    {
        bool ExecuteRules(IRuleContext context, string rulesetType);
    }
}
