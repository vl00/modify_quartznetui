#if HostInWindowsService

using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Host
{
    partial class Program
    {
        static void Main(string[] args) 
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(typeof(Program).GetTypeInfo().Assembly.ManifestModule.FullyQualifiedName));

            CreateHostBuilder(args).UseWindowsService().Build().Run();
        }
    }
}

#endif