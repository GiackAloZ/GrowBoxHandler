using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Gpio;

namespace CheckACDConverter
{
    public class ShiftRegister
    {
        private bool[] bits;

        private GpioPin _dataPin;
        private GpioPin _clockPin;
        private GpioPin _latchPin;

        private const GpioPinValue HIGH = GpioPinValue.High;
        private const GpioPinValue LOW = GpioPinValue.Low;

        private const GpioPinDriveMode OUTPUT = GpioPinDriveMode.Output;

        public ShiftRegister(int bitsN, int dp, int cp, int lp)
        {
            bits = new bool[bitsN];
            Init(dp, cp, lp);
        }

        private void Init(int dp, int cp, int lp)
        {
            GpioController controller = GpioController.GetDefault();

            _dataPin = controller.OpenPin(dp);
            _clockPin = controller.OpenPin(cp);
            _latchPin = controller.OpenPin(lp);

            _dataPin.Write(LOW);
            _clockPin.Write(LOW);
            _latchPin.Write(LOW);

            _dataPin.SetDriveMode(OUTPUT);
            _clockPin.SetDriveMode(OUTPUT);
            _latchPin.SetDriveMode(OUTPUT);

            OutResetBits();
        }

        public bool this[int i]
        {
            get
            {
                if ((i >= 0) && (i < bits.Length)) return (bits[i]);
                throw new IndexOutOfRangeException("ShiftRegister: Indice fuori range");
            }
            set
            {
                if ((i >= 0) && (i < bits.Length)) bits[i] = value;
                else throw new IndexOutOfRangeException("ShiftRegister: Indice fuori range");
            }
        }

        public void OutResetBits()
        {
            for (int i = 0; i < bits.Length; i++)
                bits[i] = false;
            OutBits();
        }

        public void OutSetBits()
        {
            for (int i = 0; i < bits.Length; i++)
                bits[i] = true;
            OutBits();
        }

        public void OutBits()
        {
            for (int i = (bits.Length - 1); i >= 0; i--)
            {
                _dataPin.Write(bits[i] ? HIGH : LOW);
                _clockPin.Write(HIGH);
                _clockPin.Write(LOW);
            }
            _latchPin.Write(HIGH);
            _latchPin.Write(LOW);
        }

        public void WriteBit(int i, bool b)
        {
            bool tmp = bits[i];
            bits[i] = b;
            OutBits();
            bits[i] = tmp;
        }
    }
}
