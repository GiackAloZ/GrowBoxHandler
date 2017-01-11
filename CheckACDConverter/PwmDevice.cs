using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using Windows.Devices.Gpio;

namespace CheckACDConverter
{
    public class PwmDevice : PhysicalDevice
    {
        private GpioPin _pin;

        private double _dutyCycle;
        private double _frequency;
        private ulong _timeOn;
        private ulong _timePeriod;

        private Task _task;

        private bool _on;

        private Stopwatch _sw;

        private const GpioPinValue HIGH = GpioPinValue.High;
        private const GpioPinValue LOW = GpioPinValue.Low;

        private const GpioPinDriveMode OUTPUT = GpioPinDriveMode.Output;

        public PwmDevice() : base () { }

        public PwmDevice(string n, int pinNumber, double freq = 50) : base(n)
        {
            GpioController gpio = GpioController.GetDefault();
            if (gpio == null) throw new Exception("GPIO Initialization Failed");

            _pin = gpio.OpenPin(pinNumber);
            _pin.Write(LOW);
            _pin.SetDriveMode(OUTPUT);

            _dutyCycle = 0;
            _frequency = freq;

            _timeOn = (ulong)((((1.0 / _frequency) / 100.0) * _dutyCycle) * 1000.0);
            _timePeriod = (ulong)((1.0 / _frequency) * 1000.0);

            _on = false;

            _task = new Task(() => PwmRun());
        }

        public void Start()
        {
            if (!_on)
                _task.Start();
            _on = true;
            Task.Delay(20).Wait();
            InvokeWriteLog("started running");
        }

        public void Stop()
        {
            if (_on)
                _task.Wait();
            _on = false;
            Task.Delay(20).Wait();
            InvokeWriteLog("stopped running");
        }

        public void ChangeDutyCycle(int dutyCycle)
        {
            if (dutyCycle < 0 || dutyCycle > 100)
                throw new ArgumentOutOfRangeException();
            _dutyCycle = dutyCycle;
            _timeOn = (ulong)((((1.0 / _frequency) / 100.0) * _dutyCycle) * 1000.0);
            _timePeriod = (ulong)((1.0 / _frequency) * 1000.0);
            InvokeWriteLog("changed dutycycle to -> " + dutyCycle);
        }

        public void ChangeFrequency(int frequency)
        {
            if (frequency < 0 || frequency > 100)
                throw new ArgumentOutOfRangeException();
            _frequency = frequency;
            _timeOn = (ulong)((((1.0 / _frequency) / 100.0) * _dutyCycle) * 1000.0);
            _timePeriod = (ulong)((1.0 / _frequency) * 1000.0);
            InvokeWriteLog("changed frequency to -> " + frequency);
        }

        private void PwmRun()
        {
            _sw = new Stopwatch(); //  Misura con precisione il tempo trascorso
            _sw.Start(); // Avvia o riprende la misurazione del tempo trascorso per un intervallo
            // number of system ticks in a single millisecond
            ulong ticksPerMs = (ulong)(Stopwatch.Frequency) / 1000;
            while (true)
            {
                var ticks = _sw.ElapsedTicks; //Ottiene il tempo totale trascorso misurato dall'istanza corrente in cicli del timer.
                _pin.Write(HIGH);
                while (true)
                {
                    var timePassed = _sw.ElapsedTicks - ticks;
                    if ((ulong)(timePassed) >= (_timeOn * ticksPerMs))
                    {
                        break;
                    }
                }
                _pin.Write(LOW);
                while (true)
                {
                    var timePassed = _sw.ElapsedTicks - ticks;
                    if ((ulong)(timePassed) >= _timePeriod * ticksPerMs)
                    {
                        break;
                    }
                }
            }
        }
    }
}
