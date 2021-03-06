﻿using NServiceBus.Config;
using NServiceBus.Integration.Azure;

namespace NServiceBus.Hosting.Azure.LoggingHandlers
{
    /// <summary>
    /// Handles logging configuration for the production profile
    /// </summary>
    public class ProductionLoggingHandler : IConfigureLoggingForProfile<Production>
    {
        void IConfigureLogging.Configure(IConfigureThisEndpoint specifier)
        {
            //if (Configure.Instance == null || Configure.Instance.Configurer == null)
            //    return;

            //Configure.Instance
            //    .AzureConfigurationSource()
            //    .Log4Net<AzureAppender>(
            //    a =>
            //    {
            //        a.ScheduledTransferPeriod = 10;
            //    });
        }
    }
}