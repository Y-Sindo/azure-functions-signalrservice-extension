// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Azure.SignalR;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Azure.WebJobs.Extensions.SignalRService
{
    internal class OptionsSetup : IConfigureOptions<ContextOptions>, IOptionsChangeTokenSource<ContextOptions>
    {
        private readonly IConfiguration configuration;
        private readonly string connectionStringKey;

        public OptionsSetup(IConfiguration configuration, string connectionStringKey)
        {
            if (string.IsNullOrWhiteSpace(connectionStringKey))
            {
                throw new ArgumentException($"'{nameof(connectionStringKey)}' cannot be null or whitespace", nameof(connectionStringKey));
            }

            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.connectionStringKey = connectionStringKey;
        }

        public string Name => Options.DefaultName;

        public void Configure(ContextOptions options)
        {
            options.ServiceEndpoints = configuration.GetSignalRServiceEndpoints(connectionStringKey);
            options.ConnectionCount = 3;
        }

        public IChangeToken GetChangeToken()
        {
            return configuration.GetReloadToken();
        }
    }
}