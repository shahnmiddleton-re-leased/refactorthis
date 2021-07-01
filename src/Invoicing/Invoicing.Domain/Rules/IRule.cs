namespace Invoicing.Domain.Rules
{
    public interface IRule<T>
    {
        bool IsSatisfied(T command);
            
        bool IsValid(T command, out string message);
    }
}
