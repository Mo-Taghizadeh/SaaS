using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthService.Application.Common.Interfaces;
using MassTransit;

namespace AuthService.Infrastructure.Messaging
{
    public class MassTransitMessageBus : IMessageBus
    {
        private readonly IPublishEndpoint _publish;

        public MassTransitMessageBus(IPublishEndpoint publish)
        {
            _publish = publish;
        }

        public Task Publish<T>(T message, CancellationToken ct = default)
            => _publish.Publish(message, ct);
    }
}
