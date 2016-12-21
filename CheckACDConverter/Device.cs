using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckACDConverter
{
    public class Device
    {
        public delegate void WriteLoggerEventHandler(string s);
        public event WriteLoggerEventHandler WriteLog;

        public string Name { get; set; }

        public Device() { }

        public Device(string n) { Name = n; }

        protected void InvokeWriteLog(string line)
        {
            string s = "The device named " + Name + " has done this operation -> ";
            s += line;
            WriteLog?.Invoke(s);
        }
    }
}
