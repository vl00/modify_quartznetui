#if HostInConsole

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Host
{
    partial class Program
    {
        static void Main(string[] args) => BuildWebHost(args).Run();
    }
}

#endif