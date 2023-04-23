using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions.ManagedClient;
using System.Data;

namespace MQTTnetServer {
    public abstract class ServiceBase : IHostedService, IDisposable {

        #region Properties & ctor
        private readonly ILogger<ServiceBase> _serviceLog;
        private readonly BrokerSettings _brokerSettings;
        private bool _stopping;

        /// <summary>
        /// MQTT client.
        /// </summary>
        protected IManagedMqttClient MqttClient { get; init; }

        /// <summary>
        /// MQTT client id.
        /// </summary>
        protected Guid MqttClientId { get; init; } = Guid.NewGuid();

        /// <summary>
        /// Holds list of active MQTT subscriptions.
        /// </summary>
        protected IList<string> SubscribedTopics { get; init; } = new List<string>();

        /// <summary>
        /// MQTT Topic root.
        /// </summary>
        protected string TopicRoot { get; init; }

        public ServiceBase(ILogger<ServiceBase> logger, BrokerSettings brokerSettings, string topicRoot) {
            _serviceLog = logger;
            _brokerSettings = brokerSettings;
            TopicRoot = topicRoot;

            var mqttEventLogger = new MqttNetEventLogger("ServiceBaseLogger");
            mqttEventLogger.LogMessagePublished += (sender, e) => {
                var msg = e.LogMessage.Message;
                switch (e.LogMessage.Level) {
                    case MqttNetLogLevel.Info:
                        _serviceLog.LogInformation("Info:{msg}", msg);
                        break;
                    case MqttNetLogLevel.Warning:
                        _serviceLog.LogWarning("Warn:{msg}", msg);
                        break;
                    case MqttNetLogLevel.Error:
                        _serviceLog.LogError("Error:{msg}, Exception:{exception}", msg, e.LogMessage.Exception);
                        break;
                    case MqttNetLogLevel.Verbose:
                    default:
                        _serviceLog.LogTrace("Trace:{msg}", msg);
                        break;
                }
            };

            var mqttFactory = new MqttFactory(mqttEventLogger);
            MqttClient = mqttFactory.CreateManagedMqttClient();

            // Log connect and disconnect events
            MqttClient.ConnectedAsync += async (e) => {
                await MqttClient.EnqueueAsync(new MqttApplicationMessageBuilder()
                    .WithTopic($"{TopicRoot}/connected")
                    .WithPayload(((int)ConnectionStatus.ConnectedMqttAndDevice).ToString())
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .WithRetainFlag()
                    .Build());

                _serviceLog.LogInformation("MQTT Connection established");
            };

            MqttClient.ConnectingFailedAsync += (e) => {
                _serviceLog.LogWarning("MQTT Connection failed, retrying...");

                return Task.CompletedTask;
            };

            MqttClient.DisconnectedAsync += (e) => {
                if (!_stopping) {
                    _serviceLog.LogInformation("MQTT Connection closed unexpectedly, reconnecting...");
                }
                else {
                    _serviceLog.LogInformation("MQTT Connection closed");
                }

                return Task.CompletedTask;
            };


            MqttClient.ApplicationMessageReceivedAsync += MqttMsgPublishReceived;
        } 
        #endregion

        #region MQTT Implementation
        /// <summary>
        /// Handles subscribed commands published to MQTT.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected abstract Task MqttMsgPublishReceived(MqttApplicationMessageReceivedEventArgs e);

        /// <summary>
        /// Subscribes to the MQTT topics in SubscribedTopics.
        /// </summary>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>Awaitable <see cref="Task" />.</returns>
        protected virtual async Task SubscribeAsync(CancellationToken cancellationToken = default) {
            _serviceLog.LogInformation("MQTT subscribing to the following topics: {topics}", string.Join(", ", SubscribedTopics));

            await MqttClient.SubscribeAsync(SubscribedTopics
               .Select(topic => new MqttTopicFilterBuilder()
                   .WithTopic(topic)
                   .WithAtLeastOnceQoS()
                   .Build()).ToList());
        }

        /// <summary>
        /// Unsubscribes from the MQTT topics in SubscribedTopics.
        /// </summary>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>Awaitable <see cref="Task" />.</returns>
        protected virtual async Task UnsubscribeAsync(CancellationToken cancellationToken = default) {
            _serviceLog.LogInformation("MQTT unsubscribing to the following topics: {topics}", string.Join(", ", SubscribedTopics));

            await MqttClient.UnsubscribeAsync(SubscribedTopics);
        }
        #endregion

        #region Service implementation

        /// <summary>
        /// Service Start action. Do not call this directly.
        /// </summary>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>Awaitable <see cref="Task" />.</returns>
        public async Task StartAsync(CancellationToken cancellationToken = default) {
            _serviceLog.LogInformation("Service start initiated");
            _stopping = false;

            // MQTT client options
            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(_brokerSettings.BrokerIp, _brokerSettings.BrokerPort)
                .WithClientId(MqttClientId.ToString())
                .WithCleanSession()
                .WithWillTopic($"{TopicRoot}/connected")
                .WithWillPayload(((int)ConnectionStatus.Disconnected).ToString())
                .WithWillQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .WithWillRetain();

            // MQTT TLS support
            if (_brokerSettings.BrokerUseTls) {
                if (_brokerSettings.BrokerTlsSettings == null) {
                    throw new ArgumentException($"BrokerSettings {nameof(_brokerSettings.BrokerTlsSettings)} cannot be null when {nameof(_brokerSettings.BrokerUseTls)} is true");
                }


                var certificateValidationHandler = MqttClientDefaultCertificateValidationHandler.Handle;

                var tlsOptions = new MqttClientOptionsBuilderTlsParameters {
                    UseTls = _brokerSettings.BrokerUseTls,
                    AllowUntrustedCertificates = _brokerSettings.BrokerTlsSettings.AllowUntrustedCertificates,
                    IgnoreCertificateChainErrors = _brokerSettings.BrokerTlsSettings.IgnoreCertificateChainErrors,
                    IgnoreCertificateRevocationErrors = _brokerSettings.BrokerTlsSettings.IgnoreCertificateRevocationErrors,
                    SslProtocol = _brokerSettings.BrokerTlsSettings.SslProtocol,
                    Certificates = _brokerSettings.BrokerTlsSettings.Certificates,
                    CertificateValidationHandler = certificateValidationHandler
                };

                optionsBuilder.WithTls(tlsOptions);
            }

            // MQTT credentials
            if (!string.IsNullOrEmpty(_brokerSettings.BrokerUsername) && !string.IsNullOrEmpty(_brokerSettings.BrokerPassword)) {
                optionsBuilder.WithCredentials(_brokerSettings.BrokerUsername, _brokerSettings.BrokerPassword);
            }

            var managedOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(_brokerSettings.BrokerReconnectDelay))
                .WithClientOptions(optionsBuilder.Build())
                .Build();

            // Subscribe to MQTT messages
            await SubscribeAsync(cancellationToken);

            // Connect to MQTT
            await MqttClient.StartAsync(managedOptions);

            // Call startup on inheriting service class
            await StartServiceAsync(cancellationToken);

            _serviceLog.LogInformation("Service started successfully");
        }

        /// <summary>
        /// Service Stop action. Do not call this directly.
        /// </summary>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>Awaitable <see cref="Task" />.</returns>
        public async Task StopAsync(CancellationToken cancellationToken = default) {
            _serviceLog.LogInformation("Service stop initiated");

            _stopping = true;

            try {
                // Stop inheriting service class
                await StopServiceAsync(cancellationToken);

                // Graceful MQTT disconnect
                if (MqttClient.IsConnected) {
                    // Publish disconnected announcement
                    await MqttClient.EnqueueAsync(
                        new MqttApplicationMessageBuilder()
                            .WithTopic($"{TopicRoot}/connected")
                            .WithPayload(((int)ConnectionStatus.Disconnected).ToString())
                            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                            .WithRetainFlag()
                            .Build());

                    await UnsubscribeAsync(cancellationToken);

                    await MqttClient.StopAsync();
                }
            }
            catch (Exception ex) {
                // Log error, but do not rethrow, Dispose should not throw exceptions.
                _serviceLog.LogError(ex, "Exception:{ex}", ex.Message);
            }

            _serviceLog.LogInformation("Service stopped successfully");
        }

        /// <summary>
        /// Service start.
        /// </summary>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>Awaitable <see cref="Task" />.</returns>
        protected abstract Task StartServiceAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Service stop.
        /// </summary>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>Awaitable <see cref="Task" />.</returns>
        protected abstract Task StopServiceAsync(CancellationToken cancellationToken);
        #endregion

        #region IDisposable
        private bool _disposed;

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (_disposed) return;
            if (disposing) { MqttClient.Dispose(); }
            _disposed = true;
        }
        #endregion
    }
}
