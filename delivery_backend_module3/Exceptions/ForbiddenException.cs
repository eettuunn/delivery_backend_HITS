namespace delivery_backend_module3.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message)
    {
    }
}