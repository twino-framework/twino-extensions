using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using Twino.Client.TMQ;
using Twino.Client.TMQ.Connectors;
using Twino.Protocols.TMQ;

namespace Twino.Extensions.ConsumerFactory
{
    /// <summary>
    /// Twino Connector Builder
    /// </summary>
    public class TwinoConnectorBuilder
    {
        private TmqStickyConnector _connector;

        private string _id;
        private string _type;
        private string _name;
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

        /// <summary>
        /// Sets reconnection interval if disconnects. Default is 1000 milliseconds.
        /// </summary>
        public TwinoConnectorBuilder SetReconnectInterval(TimeSpan value)
        {
            _reconnectInterval = value;
            return this;
        }

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
        /// Adds new host to connect
        /// </summary>
        public TwinoConnectorBuilder AddHost(string hostname)
        {
            _hosts.Add(hostname);
            return this;
        }

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

        /// <summary>
        /// Builds new TmqStickyConnector with defined properties
        /// </summary>
        public TmqStickyConnector Build()
        {
            if (_connector != null)
                return _connector;

            _connector = new TmqAbsoluteConnector(_reconnectInterval, () =>
            {
                TmqClient client = new TmqClient();

                if (!string.IsNullOrEmpty(_id))
                    client.ClientId = _id;

                if (!string.IsNullOrEmpty(_name))
                    client.SetClientName(_name);

                if (!string.IsNullOrEmpty(_type))
                    client.SetClientType(_type);

                if (!string.IsNullOrEmpty(_token))
                    client.SetClientToken(_token);

                if (_enhance != null)
                    _enhance(client);

                return client;
            });

            _connector.AutoJoinConsumerChannels = _autoJoin;
            _connector.DisconnectionOnAutoJoinFailure = _disconnectOnJoinFailure;

            if (_useJsonConsumer)
                _connector.InitJsonReader();
            else if (_customConsumer != null)
                _connector.InitReader(_customConsumer);

            foreach (string host in _hosts)
                _connector.AddHost(host);

            if (_connected != null)
                _connector.Connected += c => _connected(_connector);

            if (_disconnected != null)
                _connector.Disconnected += c => _disconnected(_connector);

            if (_error != null)
                _connector.ExceptionThrown += (c, e) => _error(e);

            return _connector;
        }
    }
}