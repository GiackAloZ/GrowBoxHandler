using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckACDConverter
{
    public class OnOffDevice : PhysicalDevice
    {
        private bool _status;
        private int _bitN;
        private TimeSpan _timeOn;
        private DateTime _lastOnTime;

        public OnOffDevice() : base() { }
        public OnOffDevice(string n, ShiftRegister rr, int bn) : base(n, rr) { _bitN = bn; _status = false; _reg.WriteBit(_bitN, _status); }
        public void On()
        {
            if (_status)
                return;
            _status = true;
            _reg[_bitN] = _status;
            _reg.OutBits();
            _lastOnTime = DateTime.Now;
            InvokeWriteLog("turned on");
        }
        public void Off()
        {
            if (!_status)
                return;
            _status = false;
            _reg[_bitN] = _status;
            _reg.OutBits();
            _timeOn = new TimeSpan(0);
            InvokeWriteLog("turned off");
        }
        public TimeSpan CheckTimeOn()
        {
            InvokeWriteLog("checked time on");
            return _lastOnTime - DateTime.Now;
        }
    }
}
