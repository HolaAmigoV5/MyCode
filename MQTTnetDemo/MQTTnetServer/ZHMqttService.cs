using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnetServer {
    public class ZHMqttService : ServiceBase {
        private readonly ILogger<ZHMqttService> _log;

        public ZHMqttService(ILogger<ZHMqttService> logger, 
            BrokerSettings brokerSettings, string topicRoot="Home")
            :base(logger,brokerSettings,topicRoot)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Task MqttMsgPublishReceived(MqttApplicationMessageReceivedEventArgs e) {
            throw new NotImplementedException();
        }

        protected override Task StartServiceAsync(CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        protected override Task StopServiceAsync(CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}
