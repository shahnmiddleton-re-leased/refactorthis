using System.Collections.Generic;

namespace RefactorThis.Common.Interfaces.Rules
{
    public interface IRuleContext
    {
        List<IRuleValidation> Validations { get; set; }
        List<string> Information { get; set; }
        bool IsValid();
    }
}
