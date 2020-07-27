using System;
using System.Net.Http;
using Twino.Ioc;

namespace Twino.Extensions.Http
{
    /// <summary>
    /// Extension methods for HttpClientFactory
    /// </summary>
    public static class HttpExtensions
    {
        /// <summary>
        /// Adds application default transient HttpClient factory
        /// </summary>
        public static IServiceContainer AddHttpClient(this IServiceContainer services)
        {
            return AddHttpClient(services, "default", null);
        }

        /// <summary>
        /// Adds application default transient HttpClient factory with configurable action
        /// </summary>
        public static IServiceContainer AddHttpClient(this IServiceContainer services, Action<HttpClient> configureClient)
        {
            return AddHttpClient(services, "default", configureClient);
        }

        /// <summary>
        /// Adds application default transient pool HttpClient factory
        /// </summary>
        public static IServiceContainer AddHttpClient(this IServiceContainer services, int poolSize, Action<HttpClient> configureClient)
        {
            return AddHttpClient(services, "default", poolSize, configureClient);
        }

        /// <summary>
        /// Adds name specified transient HttpClient factory
        /// </summary>
        public static IServiceContainer AddHttpClient(this IServiceContainer services, string name, Action<HttpClient> configureClient)
        {
            return AddHttpClient(services, name, 0, configureClient);
        }

        /// <summary>
        /// Adds name specified transient pool HttpClient factory
        /// </summary>
        public static IServiceContainer AddHttpClient(this IServiceContainer services, string name, int poolSize, Action<HttpClient> configureClient)
        {
            IHttpClientFactory factory;
            bool found = services.TryGet(out factory);
            if (!found)
            {
                factory = new HttpClientFactory();
                services.AddSingleton(factory);
            }

            HttpClientFactory f = (HttpClientFactory) factory;
            f.AddConfiguration(name, poolSize, configureClient);

            return services;
        }
    }
}