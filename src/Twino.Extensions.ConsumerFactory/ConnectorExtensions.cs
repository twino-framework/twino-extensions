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
        #region Twino

        /// <summary>
        /// Adds Twino connector with configuration
        /// </summary>
        public static ITwinoServiceCollection UseTwinoBus(this ITwinoServiceCollection services, Action<TwinoConnectorBuilder> config)
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
        public static ITwinoServiceCollection UseTwinoBus<TIdentifier>(this ITwinoServiceCollection services, Action<TwinoConnectorBuilder> config)
        {
            TwinoConnectorBuilder builder = new TwinoConnectorBuilder();
            config(builder);

            TmqStickyConnector<TIdentifier> connector = builder.Build<TIdentifier>();

            AddConsumersTwino(services, connector, builder);
            services.AddSingleton(connector);
            services.AddSingleton<ITwinoBus<TIdentifier>>(connector);

            connector.Run();
            return services;
        }

        private static void AddConsumersTwino(ITwinoServiceCollection services, TmqStickyConnector connector, TwinoConnectorBuilder builder)
        {
            foreach (Tuple<ImplementationType, Type> pair in builder.AssembyConsumers)
            {
                IEnumerable<Type> types = connector.Observer
                                                   .RegisterAssemblyConsumers(() => new TwinoIocConsumerFactory((IServiceContainer)services, pair.Item1),
                                                                              pair.Item2);

                foreach (Type type in types)
                    AddConsumerIntoContainer(services, pair.Item1, type);
            }

            foreach (Tuple<ImplementationType, Type> pair in builder.IndividualConsumers)
            {
                connector.Observer.RegisterConsumer(pair.Item2,
                                                    () => new TwinoIocConsumerFactory((IServiceContainer)services, pair.Item1));
                AddConsumerIntoContainer(services, pair.Item1, pair.Item2);
            }
        }

        private static void AddConsumerIntoContainer(ITwinoServiceCollection container, ImplementationType implementationType, Type consumerType)
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

        #endregion

        #region MS DI

        /// <summary>
        /// Adds Twino connector with configuration
        /// </summary>
        public static IServiceCollection AddTwinoBus(this IServiceCollection services, Action<TwinoConnectorBuilder> config)
        {
            TwinoConnectorBuilder builder = new TwinoConnectorBuilder();
            config(builder);

            TmqStickyConnector connector = builder.Build();

            AddConsumersMicrosoftDI(services, connector, builder);
            services.AddSingleton(connector);
            services.AddSingleton<ITwinoBus>(connector);

            return services;
        }

        /// <summary>
        /// Adds Twino connector with configuration
        /// </summary>
        public static IServiceCollection AddTwinoBus<TIdentifier>(this IServiceCollection services, Action<TwinoConnectorBuilder> config)
        {
            TwinoConnectorBuilder builder = new TwinoConnectorBuilder();
            config(builder);

            TmqStickyConnector<TIdentifier> connector = builder.Build<TIdentifier>();

            AddConsumersMicrosoftDI(services, connector, builder);
            services.AddSingleton(connector);
            services.AddSingleton<ITwinoBus<TIdentifier>>(connector);

            return services;
        }

        private static void AddConsumersMicrosoftDI(IServiceCollection services, TmqStickyConnector connector, TwinoConnectorBuilder builder)
        {
            foreach (Tuple<ImplementationType, Type> pair in builder.AssembyConsumers)
            {
                IEnumerable<Type> types = connector.Observer
                                                   .RegisterAssemblyConsumers(() => new MicrosoftDependencyConsumerFactory(pair.Item1.ToLifeTime()),
                                                                              pair.Item2);

                foreach (Type type in types)
                    AddConsumerIntoCollection(services, pair.Item1, type);
            }

            foreach (Tuple<ImplementationType, Type> pair in builder.IndividualConsumers)
            {
                connector.Observer.RegisterConsumer(pair.Item2, () => new MicrosoftDependencyConsumerFactory(pair.Item1.ToLifeTime()));
                AddConsumerIntoCollection(services, pair.Item1, pair.Item2);
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

        /// <summary>
        /// Uses twino bus and connects to the server
        /// </summary>
        public static IServiceProvider UseTwinoBus(this IServiceProvider provider)
        {
            MicrosoftDependencyConsumerFactory.Provider = provider;
            TmqStickyConnector connector = provider.GetService<TmqStickyConnector>();
            connector.Run();
            return provider;
        }

        /// <summary>
        /// Uses twino bus and connects to the server
        /// </summary>
        public static IServiceProvider UseTwinoBus<TIdentifier>(this IServiceProvider provider)
        {
            MicrosoftDependencyConsumerFactory.Provider = provider;
            TmqStickyConnector<TIdentifier> connector = provider.GetService<TmqStickyConnector<TIdentifier>>();
            connector.Run();
            return provider;
        }

        #endregion

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