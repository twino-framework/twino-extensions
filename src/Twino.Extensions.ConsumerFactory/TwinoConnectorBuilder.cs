using System;
using System.Collections.Generic;
using Twino.Client.TMQ;
using Twino.Client.TMQ.Connectors;
using Twino.Ioc;
using Twino.Protocols.TMQ;

namespace Twino.Extensions.ConsumerFactory
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
        private bool _useJsonConsumer = true;
        private bool _autoJoin = true;
        private bool _disconnectOnJoinFailure = true;

        private Func<TmqMessage, Type, object> _customConsumer = null;
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

        #region Consumers

        /// <summary>
        /// Uses JSON serialization for consuming messages
        /// </summary>
        public TwinoConnectorBuilder UseJsonConsumer()
        {
            _useJsonConsumer = true;
            _customConsumer = null;
            return this;
        }

        /// <summary>
        /// Uses custom serialization for consuming messages
        /// </summary>
        public TwinoConnectorBuilder UseCustomConsumer(Func<TmqMessage, Type, object> action)
        {
            _useJsonConsumer = false;
            _customConsumer = action;
            return this;
        }

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
        public TmqStickyConnector Build()
        {
            if (_connector != null)
                return _connector;

            _connector = new TmqAbsoluteConnector(_reconnectInterval, new ConnectorInstanceCreator(_id, _name, _type, _token, _enhance).CreateInstance);

            _connector.AutoJoinConsumerChannels = _autoJoin;
            _connector.DisconnectionOnAutoJoinFailure = _disconnectOnJoinFailure;

            if (_useJsonConsumer)
                _connector.InitJsonReader();
            else if (_customConsumer != null)
                _connector.InitReader(_customConsumer);

            foreach (string host in _hosts)
                _connector.AddHost(host);

            if (_connected != null)
                _connector.Connected += new ConnectionEventMapper(_connector, _connected).Action;

            if (_disconnected != null)
                _connector.Disconnected += new ConnectionEventMapper(_connector, _disconnected).Action;

            if (_error != null)
                _connector.ExceptionThrown += new ExceptionEventMapper(_connector, _error).Action;

            return _connector;
        }

        /// <summary>
        /// Releases all resources
        /// </summary>
        internal void Dispose()
        {
            _connector = null;
            _customConsumer = null;
            _connected = null;
            _disconnected = null;
            _error = null;
            _enhance = null;
            _assembyConsumers.Clear();
            _individualConsumers.Clear();
        }

        #endregion
    }
}