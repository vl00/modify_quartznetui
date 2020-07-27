using System;
using System.Collections.Generic;

namespace Host.Model
{
    public class ProcessModel
    {
        public string Name { get; set; }
        public string Args { get; set; }
        public string Wd { get; set; }
        public bool Noshell { get; set; } = false;
        public Dictionary<string, string> Envs { get; set; }
        public bool NewWin { get; set; } = false;
        public int? Wait { get; set; }
    }
}
