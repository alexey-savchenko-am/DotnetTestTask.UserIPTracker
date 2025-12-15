namespace SharedKernel.Domain.Exceptions;

public sealed class ApplicationValidationException : Exception
{
    public ApplicationValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public ApplicationValidationException()
    {
    }

    public ApplicationValidationException(string message) : base(message)
    {
    }
}