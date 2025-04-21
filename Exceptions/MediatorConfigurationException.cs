namespace Balta.Mediator.Exceptions;

public sealed class MediatorConfigurationException(string configurationErrorMessage)
    : InvalidOperationException(configurationErrorMessage)
{
}
