using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Twino.Client.TMQ.Connectors;
using Twino.Ioc;

namespace Twino.Extensions.ConsumerFactory
{
    /// <summary>
    /// Twino Connector implementations
    /// </summary>
    public static class TwinoIocConnectorExtensions
    {
        /// <summary>
        /// Adds Twino connector with configuration
        /// </summary>
        public static IServiceContainer UseTwinoBus(this IServiceContainer services, Action<TwinoConnectorBuilder> config)
        {
            TwinoConnectorBuilder builder = new TwinoConnectorBuilder();
            config(builder);

            TmqStickyConnector connector = builder.Build();

            AddConsumersTwino(services, connector, builder);
            services.AddSingleton(connector);
            services.AddSingleton<ITwinoBus>(connector);

            connector.Run();
            return services;
        }

        /// <summary>
        /// Adds Twino connector with configuration
        /// </summary>
        public static IServiceCollection UseTwinoBus(this IServiceCollection services, Action<TwinoConnectorBuilder> config)
        {
            return UseTwinoBus(services, services.BuildServiceProvider(), config);
        }

        /// <summary>
        /// Adds Twino connector with configuration
        /// </summary>
        public static IServiceCollection UseTwinoBus(this IServiceCollection services, IServiceProvider provider, Action<TwinoConnectorBuilder> config)
        {
            TwinoConnectorBuilder builder = new TwinoConnectorBuilder();
            config(builder);

            TmqStickyConnector connector = builder.Build();

            AddConsumersMicrosoftDI(services, provider, connector, builder);
            services.AddSingleton(connector);
            services.AddSingleton<ITwinoBus>(connector);

            connector.Run();
            return services;
        }

        private static void AddConsumersTwino(IServiceContainer services, TmqStickyConnector connector, TwinoConnectorBuilder builder)
        {
            foreach (Tuple<ImplementationType, Type> pair in builder.AssembyConsumers)
            {
                IEnumerable<Type> types = connector.Consumer
                                                   .RegisterAssemblyConsumers(() => new TwinoIocConsumerFactory(services, pair.Item1),
                                                                              pair.Item2);

                foreach (Type type in types)
                    AddConsumerIntoContainer(services, pair.Item1, type);
            }

            foreach (Tuple<ImplementationType, Type> pair in builder.IndividualConsumers)
            {
                connector.Consumer.RegisterConsumer(pair.Item2, () => new TwinoIocConsumerFactory(services, pair.Item1));
                AddConsumerIntoContainer(services, pair.Item1, pair.Item2);
            }
        }

        private static void AddConsumersMicrosoftDI(IServiceCollection services, IServiceProvider provider, TmqStickyConnector connector, TwinoConnectorBuilder builder)
        {
            foreach (Tuple<ImplementationType, Type> pair in builder.AssembyConsumers)
            {
                IEnumerable<Type> types = connector.Consumer
                                                   .RegisterAssemblyConsumers(() => new MicrosoftDependencyConsumerFactory(provider, pair.Item1.ToLifeTime()),
                                                                              pair.Item2);

                foreach (Type type in types)
                    AddConsumerIntoCollection(services, pair.Item1, type);
            }

            foreach (Tuple<ImplementationType, Type> pair in builder.IndividualConsumers)
            {
                connector.Consumer.RegisterConsumer(pair.Item2, () => new MicrosoftDependencyConsumerFactory(provider, pair.Item1.ToLifeTime()));
                AddConsumerIntoCollection(services, pair.Item1, pair.Item2);
            }
        }

        private static void AddConsumerIntoContainer(IServiceContainer container, ImplementationType implementationType, Type consumerType)
        {
            switch (implementationType)
            {
                case ImplementationType.Transient:
                    container.AddTransient(consumerType, consumerType);
                    break;

                case ImplementationType.Scoped:
                    container.AddScoped(consumerType, consumerType);
                    break;

                case ImplementationType.Singleton:
                    container.AddSingleton(consumerType, consumerType);
                    break;
            }
        }

        private static void AddConsumerIntoCollection(IServiceCollection container, ImplementationType implementationType, Type consumerType)
        {
            switch (implementationType)
            {
                case ImplementationType.Transient:
                    container.AddTransient(consumerType, consumerType);
                    break;

                case ImplementationType.Scoped:
                    container.AddScoped(consumerType, consumerType);
                    break;

                case ImplementationType.Singleton:
                    container.AddSingleton(consumerType, consumerType);
                    break;
            }
        }

        private static ServiceLifetime ToLifeTime(this ImplementationType type)
        {
            if (type == ImplementationType.Singleton)
                return ServiceLifetime.Singleton;

            if (type == ImplementationType.Scoped)
                return ServiceLifetime.Scoped;

            return ServiceLifetime.Transient;
        }
    }
}