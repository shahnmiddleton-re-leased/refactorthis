using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Persistence.Model.Interface
{
    public interface IServiceResponse
    {
        bool IsErrorExist { get; set; }

        string Message { get; set; }

        Exception Exception { get; set; }
    }
}
