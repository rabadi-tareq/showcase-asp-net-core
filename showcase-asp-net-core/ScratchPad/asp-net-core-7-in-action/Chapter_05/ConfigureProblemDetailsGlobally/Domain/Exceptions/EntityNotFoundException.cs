namespace ConfigureProblemDetailsGlobally.Domain.Exceptions
{
    public class EntityNotFoundException(string message = "The requested resource was not found.")
        : DomainException(message);
}
