namespace ConfigureProblemDetailsGlobally.Domain.Exceptions
{
    public class InsufficientFundsException(string message = "The account balance is too low.")
        : DomainException(message);
}
