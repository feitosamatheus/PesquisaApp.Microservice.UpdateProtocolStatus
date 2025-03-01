using ApiGetewayAppPesquisa.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.UpdateProtocolStatus.Application.Exceptions;

public class ValueIsNullException : CustomException
{
    public ValueIsNullException() : base() { }
    public ValueIsNullException(string message) : base(message) { }
    public ValueIsNullException(string message, Exception exception) : base(message, exception) { }
}
