﻿using CSGenio.core.messaging;
using CSGenio.framework;
using CSGenio.messaging;

namespace GenioMVC;

public class MessagingServiceHost : IHostedService
{
    private MessagingService _messagingService;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if(Configuration.Messaging.Enabled)
        {
            _messagingService = MessagingService.Instance;
            _messagingService.Start(
                metadata: MessageMetadataFactory.GeneratedMetadata(),
                providerType: Configuration.Messaging.Host.Provider,
                enableSubscribe: false
            );
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _messagingService?.Close();
        return Task.CompletedTask;
    }
}
