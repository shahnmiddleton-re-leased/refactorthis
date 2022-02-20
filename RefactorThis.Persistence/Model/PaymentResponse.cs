using RefactorThis.Persistence.Model.Interface;
using System;

namespace RefactorThis.Persistence.Model
{
    public class PaymentResponse: IServiceResponse
    {
        public bool IsErrorExist { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }
    }
}
