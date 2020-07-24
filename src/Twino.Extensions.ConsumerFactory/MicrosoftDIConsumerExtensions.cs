using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Twino.Client.TMQ;
using Twino.Client.TMQ.Connectors;

namespace Twino.Extensions.ConsumerFactory
{
    /// <summary>
    /// Adds consumers to Microsoft Dependency IServiceCollection
    /// </summary>
    public static class MicrosoftDIConsumerExtensions
    {
        /// <summary>
        /// Adds consumer types as transient
        /// </summary>
        public static IServiceCollection AddTransientConsumers(this IServiceCollection services,
                                                               params Type[] assemblyTypes)
        {
            IServiceProvider provider = services.BuildServiceProvider();
            TmqStickyConnector connector = provider.GetService<TmqStickyConnector>();
            return AddTransientConsumers(services, provider, connector.Consumer, assemblyTypes);
        }

        /// <summary>
        /// Adds consumer types as transient
        /// </summary>
        public static IServiceCollection AddTransientConsumers(this IServiceCollection services,
                                                               MessageConsumer consumer,
                                                               params Type[] assemblyTypes)
        {
            return AddTransientConsumers(services, services.BuildServiceProvider(), consumer, assemblyTypes);
        }

        /// <summary>
        /// Adds consumer types as transient
        /// </summary>
        public static IServiceCollection AddTransientConsumers(this IServiceCollection services,
                                                               IServiceProvider provider,
                                                               params Type[] assemblyTypes)
        {
            TmqStickyConnector connector = provider.GetService<TmqStickyConnector>();
            return AddTransientConsumers(services, provider, connector.Consumer, assemblyTypes);
        }

        /// <summary>
        /// Adds consumer types as transient
        /// </summary>
        public static IServiceCollection AddTransientConsumers(this IServiceCollection services,
                                                               IServiceProvider provider,
                                                               MessageConsumer consumer,
                                                               params Type[] assemblyTypes)
        {
            IEnumerable<Type> consumerTypes = consumer.RegisterAssemblyConsumers(() => new MicrosoftDependencyConsumerFactory(provider, ServiceLifetime.Transient),
                                                                                 assemblyTypes);

            foreach (Type type in consumerTypes)
                services.AddTransient(type, type);

            return services;
        }

        /// <summary>
        /// Adds consumer types as scoped
        /// </summary>
        public static IServiceCollection AddScopedConsumers(this IServiceCollection services,
                                                            params Type[] assemblyTypes)
        {
            IServiceProvider provider = services.BuildServiceProvider();
            TmqStickyConnector connector = provider.GetService<TmqStickyConnector>();
            return AddScopedConsumers(services, provider, connector.Consumer, assemblyTypes);
        }

        /// <summary>
        /// Adds consumer types as scoped
        /// </summary>
        public static IServiceCollection AddScopedConsumers(this IServiceCollection services,
                                                            MessageConsumer consumer,
                                                            params Type[] assemblyTypes)
        {
            return AddScopedConsumers(services, services.BuildServiceProvider(), consumer, assemblyTypes);
        }

        /// <summary>
        /// Adds consumer types as scoped
        /// </summary>
        public static IServiceCollection AddScopedConsumers(this IServiceCollection services,
                                                            IServiceProvider provider,
                                                            params Type[] assemblyTypes)
        {
            TmqStickyConnector connector = provider.GetService<TmqStickyConnector>();
            return AddScopedConsumers(services, provider, connector.Consumer, assemblyTypes);
        }

        /// <summary>
        /// Adds consumer types as scoped
        /// </summary>
        public static IServiceCollection AddScopedConsumers(this IServiceCollection services,
                                                            IServiceProvider provider,
                                                            MessageConsumer consumer,
                                                            params Type[] assemblyTypes)
        {
            IEnumerable<Type> consumerTypes = consumer.RegisterAssemblyConsumers(() => new MicrosoftDependencyConsumerFactory(provider, ServiceLifetime.Scoped),
                                                                                 assemblyTypes);

            foreach (Type type in consumerTypes)
                services.AddScoped(type, type);

            return services;
        }

        /// <summary>
        /// Adds consumer types as singleton
        /// </summary>
        public static IServiceCollection AddSingletonConsumers(this IServiceCollection services,
                                                               params Type[] assemblyTypes)
        {
            IServiceProvider provider = services.BuildServiceProvider();
            TmqStickyConnector connector = provider.GetService<TmqStickyConnector>();
            return AddSingletonConsumers(services, provider, connector.Consumer, assemblyTypes);
        }

        /// <summary>
        /// Adds consumer types as singleton
        /// </summary>
        public static IServiceCollection AddSingletonConsumers(this IServiceCollection services,
                                                               MessageConsumer consumer,
                                                               params Type[] assemblyTypes)
        {
            return AddSingletonConsumers(services, services.BuildServiceProvider(), consumer, assemblyTypes);
        }

        /// <summary>
        /// Adds consumer types as singleton
        /// </summary>
        public static IServiceCollection AddSingletonConsumers(this IServiceCollection services,
                                                               IServiceProvider provider,
                                                               params Type[] assemblyTypes)
        {
            TmqStickyConnector connector = provider.GetService<TmqStickyConnector>();
            return AddSingletonConsumers(services, provider, connector.Consumer, assemblyTypes);
        }

        /// <summary>
        /// Adds consumer types as singleton
        /// </summary>
        public static IServiceCollection AddSingletonConsumers(this IServiceCollection services,
                                                               IServiceProvider provider,
                                                               MessageConsumer consumer,
                                                               params Type[] assemblyTypes)
        {
            IEnumerable<Type> consumerTypes = consumer.RegisterAssemblyConsumers(() => new MicrosoftDependencyConsumerFactory(provider, ServiceLifetime.Singleton),
                                                                                 assemblyTypes);

            foreach (Type type in consumerTypes)
                services.AddSingleton(type, type);

            return services;
        }
    }
}