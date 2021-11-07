namespace RefactorThis.Domain.Common.ValidationModel
{
    public class ValidationStatus
    {
        public bool Success { get;}
        public string Message { get;}

        public ValidationStatus(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
