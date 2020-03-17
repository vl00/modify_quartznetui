using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Host
{
    public partial class Program
    {
        static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)                
                .UseKestrel()
                .UseStartup<Startup>()
                .UseUrls("http://*:52726", "https://*:52727")
                .Build(); 
    }
}
