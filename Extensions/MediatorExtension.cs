using System.Reflection;
using Balta.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Balta.Mediator.Extensions;

public static class MediatorExtension
{
    public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddTransient<IMediator, Mediator>();
        
        var handlerInterfaceType = typeof(IHandler<,>);

        foreach (var assembly in assemblies)
        {
            var handlers = assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces(), (t, i) => new { Type = t, Interface = i })
                .Where(ti => ti.Interface.IsGenericType && ti.Interface.GetGenericTypeDefinition() == handlerInterfaceType);

            foreach (var handler in handlers) 
                services.AddTransient(handler.Interface, handler.Type);
        }

        return services;
    }
}