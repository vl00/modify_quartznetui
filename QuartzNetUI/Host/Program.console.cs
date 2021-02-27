#if HostInConsole

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Host
{
    partial class Program
    {
        static void Main(string[] args) => CreateHostBuilder(args).Build().Run();
    }
}

#endif