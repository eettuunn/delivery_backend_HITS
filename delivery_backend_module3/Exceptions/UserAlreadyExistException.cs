namespace delivery_backend_module3.Exceptions;

public class UserAlreadyExistException : Exception
{
    public UserAlreadyExistException(string message) : base(message)
    {
    }
}