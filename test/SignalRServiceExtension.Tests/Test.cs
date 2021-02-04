// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace SignalRServiceExtension.Tests
{
    public class Test
    {
        [Fact]
        public async Task TestScriptHostAsync()
        {
            var host = new HostBuilder().AddScriptHost(o =>
            {
                o.ScriptPath = @"..\..\samples\simple-chat\js\functionapp";
                o.IsSelfHost = true;
            }).ConfigureWebJobs(webjobsBuilder =>
            {
                webjobsBuilder.AddSignalR();
            })
            .Build();
            await host.StartAsync();
        }
    }
}