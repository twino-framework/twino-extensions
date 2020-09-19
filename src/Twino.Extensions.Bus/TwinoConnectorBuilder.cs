using System;
using System.Collections.Generic;
using Twino.Client.TMQ;
using Twino.Client.TMQ.Connectors;
using Twino.Ioc;
using Twino.Protocols.TMQ;

namespace Twino.Extensions.Bus
{
    /// <summary>
    /// Twino Connector Builder
    /// </summary>
    public class TwinoConnectorBuilder
    {
        #region Fields

        private TmqStickyConnector _connector;

        private string _id;
        private string _type = "bus";
        private string _name = "unnamed";
        private string _token;
        private TimeSpan _reconnectInterval = TimeSpan.FromSeconds(1);
        private bool _autoJoin = true;
        private bool _disconnectOnJoinFailure = true;
        private IMessageContentSerializer _contentSerializer;

        private readonly List<string> _hosts = new List<string>();

        private Action<TmqStickyConnector> _connected;
        private Action<TmqStickyConnector> _disconnected;
        private Action<Exception> _error;
        private Action<TmqClient> _enhance;

        private readonly List<Tuple<ImplementationType, Type>> _individualConsumers = new List<Tuple<ImplementationType, Type>>();
        private readonly List<Tuple<ImplementationType, Type>> _assembyConsumers = new List<Tuple<ImplementationType, Type>>();

        internal List<Tuple<ImplementationType, Type>> IndividualConsumers => _individualConsumers;

        internal List<Tuple<ImplementationType, Type>> AssembyConsumers => _assembyConsumers;

        #endregion

        #region Client Info

        /// <summary>
        /// Sets client Id. It must be unique.
        /// If another client with same id is already connected to server,
        /// Server will generate new id for this connector
        /// </summary>
        public TwinoConnectorBuilder SetClientId(string id)
        {
            _id = id;
            return this;
        }

        /// <summary>
        /// Client name
        /// </summary>
        public TwinoConnectorBuilder SetClientName(string name)
        {
            _name = name;
            return this;
        }

        /// <summary>
        /// Client type
        /// </summary>
        public TwinoConnectorBuilder SetClientType(string type)
        {
            _type = type;
            return this;
        }

        /// <summary>
        /// Client token for server side authentication and authorization
        /// </summary>
        public TwinoConnectorBuilder SetClientToken(string token)
        {
            _token = token;
            return this;
        }

        #endregion

        #region Connection

        /// <summary>
        /// Adds new host to connect
        /// </summary>
        public TwinoConnectorBuilder AddHost(string hostname)
        {
            _hosts.Add(hostname);
            return this;
        }

        /// <summary>
        /// Sets reconnection interval if disconnects. Default is 1000 milliseconds.
        /// </summary>
        public TwinoConnectorBuilder SetReconnectInterval(TimeSpan value)
        {
            _reconnectInterval = value;
            return this;
        }

        /// <summary>
        /// Executed before each connection initialization.
        /// You can customize and add more options to the client.
        /// </summary>
        public TwinoConnectorBuilder EnhanceConnection(Action<TmqClient> action)
        {
            _enhance = action;
            return this;
        }

        /// <summary>
        /// If true, connector joins all consuming channels.
        /// If false, you should join channels manually.
        /// Default is true.
        /// </summary>
        public TwinoConnectorBuilder AutoJoinConsumerChannels(bool value)
        {
            _autoJoin = value;
            return this;
        }

        /// <summary>
        /// If true, disconnected when any of auto channel join request fails.
        /// Default is true.
        /// </summary>
        public TwinoConnectorBuilder DisconnectionOnAutoJoinFailure(bool value)
        {
            _disconnectOnJoinFailure = value;
            return this;
        }

        #endregion

        #region Serializers

        /// <summary>
        /// Uses Newtonsoft library for JSON serializations
        /// </summary>
        public TwinoConnectorBuilder UseNewtonsoftJsonSerializer(Newtonsoft.Json.JsonSerializerSettings settings = null)
        {
            _contentSerializer = new NewtonsoftContentSerializer(settings);
            return this;
        }

        /// <summary>
        /// Uses System.Text.Json library for JSON serializations
        /// </summary>
        public TwinoConnectorBuilder UseSystemJsonSerializer(System.Text.Json.JsonSerializerOptions options = null)
        {
            _contentSerializer = new SystemJsonContentSerializer(options);
            return this;
        }

        /// <summary>
        /// Uses custom serializer
        /// </summary>
        public TwinoConnectorBuilder UseCustomSerializer(IMessageContentSerializer serializer)
        {
            _contentSerializer = serializer;
            return this;
        }

        #endregion

        #region Consumers

        /// <summary>
        /// Registers new transient consumer
        /// </summary>
        public TwinoConnectorBuilder AddTransientConsumer<TConsumer>() where TConsumer : class
        {
            _individualConsumers.Add(new Tuple<ImplementationType, Type>(ImplementationType.Transient, typeof(TConsumer)));
            return this;
        }

        /// <summary>
        /// Registers new scoped consumer
        /// </summary>
        public TwinoConnectorBuilder AddScopedConsumer<TConsumer>() where TConsumer : class
        {
            _individualConsumers.Add(new Tuple<ImplementationType, Type>(ImplementationType.Scoped, typeof(TConsumer)));
            return this;
        }

        /// <summary>
        /// Registers new singleton consumer
        /// </summary>
        public TwinoConnectorBuilder AddSingletonConsumer<TConsumer>() where TConsumer : class
        {
            _individualConsumers.Add(new Tuple<ImplementationType, Type>(ImplementationType.Singleton, typeof(TConsumer)));
            return this;
        }

        /// <summary>
        /// Registers all consumers types with transient lifetime in type assemblies
        /// </summary>
        public TwinoConnectorBuilder AddTransientConsumers(params Type[] assemblyTypes)
        {
            foreach (Type type in assemblyTypes)
                _assembyConsumers.Add(new Tuple<ImplementationType, Type>(ImplementationType.Transient, type));

            return this;
        }

        /// <summary>
        /// Registers all consumers types with scoped lifetime in type assemblies
        /// </summary>
        public TwinoConnectorBuilder AddScopedConsumers(params Type[] assemblyTypes)
        {
            foreach (Type type in assemblyTypes)
                _assembyConsumers.Add(new Tuple<ImplementationType, Type>(ImplementationType.Scoped, type));

            return this;
        }

        /// <summary>
        /// Registers all consumers types with singleton lifetime in type assemblies
        /// </summary>
        public TwinoConnectorBuilder AddSingletonConsumers(params Type[] assemblyTypes)
        {
            foreach (Type type in assemblyTypes)
                _assembyConsumers.Add(new Tuple<ImplementationType, Type>(ImplementationType.Singleton, type));

            return this;
        }

        #endregion

        #region Events

        /// <summary>
        /// Action for connected events
        /// </summary>
        public TwinoConnectorBuilder OnConnected(Action<TmqStickyConnector> action)
        {
            _connected = action;
            return this;
        }

        /// <summary>
        /// Action for disconnected events
        /// </summary>
        public TwinoConnectorBuilder OnDisconnected(Action<TmqStickyConnector> action)
        {
            _disconnected = action;
            return this;
        }

        /// <summary>
        /// Action for errors
        /// </summary>
        public TwinoConnectorBuilder OnError(Action<Exception> action)
        {
            _error = action;
            return this;
        }

        #endregion

        #region Build - Dispose

        /// <summary>
        /// Builds new TmqStickyConnector with defined properties
        /// </summary>
        public TmqStickyConnector<TIdentifier> Build<TIdentifier>()
        {
            if (_connector != null)
                return (TmqStickyConnector<TIdentifier>) _connector;

            _connector = new TmqStickyConnector<TIdentifier>(_reconnectInterval, new ConnectorInstanceCreator(_id, _name, _type, _token, _enhance).CreateInstance);
            ConfigureConnector(_connector);

            return (TmqStickyConnector<TIdentifier>) _connector;
        }

        /// <summary>
        /// Builds new TmqStickyConnector with defined properties
        /// </summary>
        public TmqStickyConnector Build()
        {
            if (_connector != null)
                return _connector;

            _connector = new TmqStickyConnector(_reconnectInterval, new ConnectorInstanceCreator(_id, _name, _type, _token, _enhance).CreateInstance);
            ConfigureConnector(_connector);
            return _connector;
        }


        /// <summary>
        /// Applies configurations on connector
        /// </summary>
        private void ConfigureConnector(TmqStickyConnector connector)
        {
            connector.AutoJoinConsumerChannels = _autoJoin;
            connector.DisconnectionOnAutoJoinFailure = _disconnectOnJoinFailure;
            if (_contentSerializer != null)
                connector.ContentSerializer = _contentSerializer;

            foreach (string host in _hosts)
                connector.AddHost(host);

            if (_connected != null)
                connector.Connected += new ConnectionEventMapper(connector, _connected).Action;

            if (_disconnected != null)
                connector.Disconnected += new ConnectionEventMapper(connector, _disconnected).Action;

            if (_error != null)
                connector.ExceptionThrown += new ExceptionEventMapper(connector, _error).Action;
        }

        /// <summary>
        /// Releases all resources
        /// </summary>
        internal void Dispose()
        {
            _connector = null;
            _connected = null;
            _disconnected = null;
            _error = null;
            _enhance = null;
            _hosts.Clear();
            _assembyConsumers.Clear();
            _individualConsumers.Clear();
        }

        #endregion
    }
}