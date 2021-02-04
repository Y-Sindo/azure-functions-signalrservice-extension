using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Extensions.Hosting;

namespace SignalRServiceExtension.Tests
{
    public class Test
    {
        public Test()
        {
            var hostBuilder = new HostBuilder().AddScriptHost(o => o.ScriptPath = @"C:\Users\zityang\source\extensions-for-scripthost\samples\simple-chat\js\functionapp").Build();
        }
    }
}
