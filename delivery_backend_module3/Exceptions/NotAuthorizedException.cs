namespace delivery_backend_module3.Exceptions;

public class NotAuthorizedException : Exception
{
    public NotAuthorizedException(string message) : base(message)
    {
    }
}