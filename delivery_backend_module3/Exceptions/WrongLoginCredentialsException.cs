namespace delivery_backend_module3.Exceptions;

public class WrongLoginCredentialsException : Exception
{
    public WrongLoginCredentialsException(string message) : base(message)
    {
    }
}