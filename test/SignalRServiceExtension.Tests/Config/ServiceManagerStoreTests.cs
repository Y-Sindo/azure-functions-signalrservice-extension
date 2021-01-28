﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.SignalR.Common;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Azure.SignalR.Tests.Common;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace SignalRServiceExtension.Tests
{
    public class ServiceManagerStoreTests
    {
        [Fact]
        public void GetServiceManager_WithSingleEndpoint()
        {
            var connectionString = FakeEndpointUtils.GetFakeConnectionString(1).Single();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var connectionStringKey = "key";
            configuration[connectionStringKey] = connectionString;

            var managerStore = new ServiceManagerStore(configuration, NullLoggerFactory.Instance, null);
            var hubContextStore = managerStore.GetOrAddByConnectionStringKey(connectionStringKey);
            var manager = hubContextStore.ServiceManager;
        }

        [Fact]
        public void ProductInfoExists()
        {
            var connectionString = FakeEndpointUtils.GetFakeConnectionString(1).Single();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var connectionStringKey = "key";
            configuration[connectionStringKey] = connectionString;

            var productInfo = new ServiceCollection()
                .AddSignalRServiceManager(new OptionsSetup(configuration, NullLoggerFactory.Instance, connectionStringKey))
                .BuildServiceProvider()
                .GetRequiredService<IOptions<ServiceManagerOptions>>()
                .Value.ProductInfo;
            Assert.NotNull(productInfo);
        }

        [Fact]
        public async Task DefaultRouterExists()
        {
            var builder = new HostBuilder();
            var host = builder
                .ConfigureAppConfiguration(b => b.AddInMemoryCollection(new Dictionary<string, string> { {"key", FakeEndpointUtils.GetFakeConnectionString(1).Single()
        } }))
                .ConfigureWebJobs(b => b.AddSignalR()).Build();
            var hubContext = await host.Services.GetRequiredService<IServiceManagerStore>().GetOrAddByConnectionStringKey("key").GetAsync("hubName") as IInternalServiceHubContext;
            var exp = await Assert.ThrowsAsync<AzureSignalRException>(() => hubContext.NegotiateAsync());
            Assert.IsType<AzureSignalRNotConnectedException>(exp.InnerException);
        }
    }
}