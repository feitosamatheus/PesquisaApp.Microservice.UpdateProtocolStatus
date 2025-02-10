namespace ApiGetewayAppPesquisa.Application.Exceptions;

public class CustomException : Exception
{
    public CustomException() : base() { }
    public CustomException(string message) : base(message) { }
    public CustomException(string message, Exception exception) : base(message, exception) { }
}
