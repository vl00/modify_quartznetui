using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host
{
    public class SimResult
    {
        public int Status { get; set; } = 200;
        public string Msg { get; set; }
        public object Data { get; set; }
    }
}
