﻿using Balta.Mediator.Abstractions;
using Balta.Mediator.Exceptions;

namespace Balta.Mediator;

public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        var handlerObj = serviceProvider.GetService(handlerType);
        if (handlerObj == null)
            throw new MediatorConfigurationException($"Handler not found for {requestType.Name}");

        var method = handlerType.GetMethod("HandleAsync");
        if (method == null)
            throw new MediatorConfigurationException($"HandleAsync method not found in {handlerType.Name}");

        if (method.Invoke(handlerObj, [request, cancellationToken]) is not Task<TResponse> task)
            throw new MediatorConfigurationException(
                $"Handler {handlerObj.GetType().Name} returned null or an incompatible type.");

        return await task;
    }
}