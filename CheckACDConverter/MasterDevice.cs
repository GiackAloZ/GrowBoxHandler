using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckACDConverter
{
    public class MasterDevice : Device
    {
        protected PhysicalDevice[] _physicalDevices;

        public MasterDevice() : base() { }
        public MasterDevice(string n, PhysicalDevice[] ph) : base(n) { _physicalDevices = ph; }

        public virtual string CheckStatus() { return "Not assigned"; }
    }
}
