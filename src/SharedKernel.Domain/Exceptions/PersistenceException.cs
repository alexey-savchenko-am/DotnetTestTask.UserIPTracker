namespace SharedKernel.Domain.Exceptions;

public sealed class PersistenceException : Exception
{
    public PersistenceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public PersistenceException()
    {
    }

    public PersistenceException(string message) : base(message)
    {
    }
}