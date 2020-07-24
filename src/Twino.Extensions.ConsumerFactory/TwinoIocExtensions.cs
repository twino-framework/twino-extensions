using System;
using System.Collections.Generic;
using Twino.Client.TMQ;
using Twino.Ioc;

namespace Twino.Extensions.ConsumerFactory
{
    /// <summary>
    /// Adds consumers to Twino IServiceContainer
    /// </summary>
    public static class TwinoIocExtensions
    {
        /// <summary>
        /// Adds consumer types as transient
        /// </summary>
        public static IServiceContainer AddTransientConsumers(this IServiceContainer services, MessageConsumer consumer, params Type[] assemblyTypes)
        {
            IEnumerable<Type> consumerTypes = consumer.RegisterAssemblyConsumers(() => new TwinoIocConsumerFactory(services, ImplementationType.Transient),
                                                                                 assemblyTypes);
            
            foreach (Type type in consumerTypes)
                services.AddTransient(type, type);
            
            return services;
        }

        /// <summary>
        /// Adds consumer types as scoped
        /// </summary>
        public static IServiceContainer AddScopedConsumers(this IServiceContainer services, MessageConsumer consumer, params Type[] assemblyTypes)
        {
            IEnumerable<Type> consumerTypes = consumer.RegisterAssemblyConsumers(() => new TwinoIocConsumerFactory(services, ImplementationType.Scoped),
                                                                                 assemblyTypes);

            foreach (Type type in consumerTypes)
                services.AddScoped(type, type);
            
            return services;
        }

        /// <summary>
        /// Adds consumer types as singleton
        /// </summary>
        public static IServiceContainer AddSingletonConsumers(this IServiceContainer services, MessageConsumer consumer, params Type[] assemblyTypes)
        {
            IEnumerable<Type> consumerTypes = consumer.RegisterAssemblyConsumers(() => new TwinoIocConsumerFactory(services, ImplementationType.Singleton),
                                                                                 assemblyTypes);

            foreach (Type type in consumerTypes)
                services.AddSingleton(type);

            return services;
        }
    }
}