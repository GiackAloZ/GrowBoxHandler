using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckACDConverter
{
    public class PhysicalDevice : Device
    {
        protected ShiftRegister _reg;

        public PhysicalDevice() : base() { _reg = null; }
        public PhysicalDevice(string n) : base(n) { _reg = null; }
        public PhysicalDevice(string n, ShiftRegister rr) : base(n) { _reg = rr; }
    }
}
