﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Microsoft.Azure.WebJobs.Extensions.SignalRService
{
    public class Context
    {
        public string Hub { get; set; }

        public string ConnectionId { get; set; }

        public string UserId { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public IDictionary<string, string> Query { get; set; }

        public IDictionary<string, string> Claims { get; set; }
    }
}