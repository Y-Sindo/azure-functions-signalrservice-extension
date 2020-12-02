﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Extensions.SignalRService
{
    internal class ServiceManagerStore : IServiceManagerStore
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly ServiceTransportType transportType;
        private readonly IConfiguration configuration;
        private readonly ConcurrentDictionary<string, IServiceHubContextStore> store = new ConcurrentDictionary<string, IServiceHubContextStore>();

        public ServiceManagerStore(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
            this.configuration = configuration;
            var serviceTransportTypeStr = configuration[Constants.ServiceTransportTypeName];
            var logger = loggerFactory.CreateLogger<ServiceManagerStore>();
            if (Enum.TryParse<ServiceTransportType>(serviceTransportTypeStr, out var transport))
            {
                this.transportType = transport;
            }
            else
            {
                this.transportType = ServiceTransportType.Transient;
                logger.LogWarning($"Unsupported service transport type: {serviceTransportTypeStr}. Use default {transportType} instead.");
            }
        }

        public IServiceHubContextStore GetOrAddByConfigurationKey(string configurationKey)
        {
            string connectionString = configuration[configurationKey];
            return GetOrAddByConnectionString(connectionString);
        }

        public IServiceHubContextStore GetOrAddByConnectionString(string connectionString)
        {
            return store.GetOrAdd(connectionString, CreateHubContextStore);
        }

        // test only
        public IServiceHubContextStore GetByConfigurationKey(string configurationKey)
        {
            string connectionString = configuration[configurationKey];
            return store.ContainsKey(connectionString) ? store[connectionString] : null;
        }

        private IServiceHubContextStore CreateHubContextStore(string connectionString)
        {
            var serviceManager = CreateServiceManager(connectionString);
            return new ServiceHubContextStore(serviceManager, loggerFactory);
        }

        private IServiceManager CreateServiceManager(string connectionString)
        {
            return new ServiceManagerBuilder().WithOptions(o =>
            {
                o.ConnectionString = connectionString;
                o.ServiceTransportType = transportType;
            }).WithCallingAssembly().Build();
        }
    }
}