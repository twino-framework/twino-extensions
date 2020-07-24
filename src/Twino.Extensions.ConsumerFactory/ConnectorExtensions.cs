using System;
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
        public static IServiceContainer UseTwinoConnector(this IServiceContainer services, Action<TwinoConnectorBuilder> config)
        {
            TwinoConnectorBuilder builder = new TwinoConnectorBuilder();
            config(builder);
            TmqStickyConnector connector = builder.Build();
            services.AddSingleton(connector);
            connector.Run();
            return services;
        }

        /// <summary>
        /// Adds Twino connector with configuration
        /// </summary>
        public static IServiceCollection UseTwinoConnector(this IServiceCollection services, Action<TwinoConnectorBuilder> config)
        {
            TwinoConnectorBuilder builder = new TwinoConnectorBuilder();
            config(builder);
            TmqStickyConnector connector = builder.Build();
            services.AddSingleton(connector);
            connector.Run();
            return services;
        }
    }
}