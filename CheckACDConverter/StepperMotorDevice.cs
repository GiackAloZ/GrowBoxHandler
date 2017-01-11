using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace CheckACDConverter
{
    public class StepperMotorDevice : PhysicalDevice
    {
        private int _bitA1;
        private int _bitA2;
        private int _bitB1;
        private int _bitB2;
        private GpioPin _endStopOr, _endStopAn;     // Porte collegate ai fine corsa
        private int _fase;                          // Numero di fase

        private const GpioPinValue LOW = GpioPinValue.Low;

        public StepperMotorDevice() : base() { }
        public StepperMotorDevice(string n, ShiftRegister rr, 
            int bitA1,
            int bitA2,
            int bitB1,
            int bitB2,
            int portaEndStopOrario,
            int portaEndStopAntiOrario) : base(n, rr)
        {
            _bitA1 = bitA1;
            _bitA2 = bitA2;
            _bitB1 = bitB1;
            _bitB2 = bitB2;

            GpioController gpio = GpioController.GetDefault();
            if (gpio == null) throw new Exception("GPIO Initialization Failed");

            _endStopOr = gpio.OpenPin(portaEndStopOrario);
            _endStopAn = gpio.OpenPin(portaEndStopAntiOrario);

            _endStopOr.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _endStopAn.SetDriveMode(GpioPinDriveMode.InputPullUp);

            _fase = 0;
            Folle();
        }

        public bool StepOrario(int s)
        {
            if (_endStopOr.Read() == LOW) return (false);//Esce se arrivato a fine corsa
            for (int i = 0; i < s; i++) //Effettua i passi richiesti
            {
                if (_endStopOr.Read() == LOW)
                {
                    if (s != 1)
                        InvokeWriteLog("Gone clockwise for " + s + " steps and reached endline");
                    return (false);//Esce se arrivato a fine corsa
                }
                _fase++; //Passa alla fase successiva
                _fase = _fase % 4;//Rotazione fasi 0-1-2-3
                AttivaFase();
            }
            //Toglie alimentazione alle fasi
            Folle();
            //Thread.Sleep(3);
            if (s != 1) 
                InvokeWriteLog("Gone clockwise for " + s + " steps and NOT reached endline");
            return (true);//Non è arrivato al fine corsa
        }

        public bool StepAntiOrario(int s)
        {
            if (_endStopAn.Read() == LOW) return (false);//Esce se arrivato a fine corsa
            for (int i = 0; i < s; i++) //Effettua i passi richiesti
            {
                if (_endStopAn.Read() == LOW)
                {
                    if (s != 1)
                        InvokeWriteLog("Gone anti-clockwise for " + s + " steps and reached endline");
                    return (false);//Esce se arrivato a fine corsa
                }
                _fase--;//Passa alla fase precedente
                if (_fase < 0) _fase = 3;//Rotazione fasi 3-2-1-0
                AttivaFase();

            }
            //Toglie alimentazione alle fasi
            Folle();
            //Thread.Sleep(3);
            if (s != 1)
                InvokeWriteLog("Gone anti-clockwise for " + s + " steps and NOT reached endline");
            return (true);//Non è arrivato al fine corsa
        }

        public void Freno(bool attivato)
        {
            if (attivato) AttivaFase();
            else Folle(); //Toglie alimentazione la fase attuale
            InvokeWriteLog("Toggled break with : " + attivato);
        }

        public void AttivaFase()
        {
            switch (_fase)//Seleziona la fase 
            {
                case 0://Fase 0
                    _reg[_bitA1] = true;
                    _reg[_bitA2] = false;
                    _reg[_bitB1] = false;
                    _reg[_bitB2] = false;
                    _reg.OutBits();
                    break;
                case 1://Fase 1
                    _reg[_bitA1] = false;
                    _reg[_bitA2] = false;
                    _reg[_bitB1] = true;
                    _reg[_bitB2] = false;
                    _reg.OutBits();
                    break;
                case 2://Fase 2

                    _reg[_bitA1] = false;
                    _reg[_bitA2] = true;
                    _reg[_bitB1] = false;
                    _reg[_bitB2] = false;
                    _reg.OutBits();
                    break;
                case 3://Fase 3
                    _reg[_bitA1] = false;
                    _reg[_bitA2] = false;
                    _reg[_bitB1] = false;
                    _reg[_bitB2] = true;
                    _reg.OutBits();
                    break;
            }
            //L'albero motore deve avere il tempo di raggiungere la nuova posizione
            Task.Delay(20).Wait();// "1" per Stepper Motor Nema 17 "3" M42SP-5
        }

        public void PosizionaFineCorsaOrario()
        {
            while (StepOrario(1)) ; //Si posiziona a fine-corsa anti oraria
            Folle();
            InvokeWriteLog("Gone to clockwise endline");
        }

        public void PosizionaFineCorsaAntiOrario()
        {
            while (StepAntiOrario(1)) ; //Si posiziona a fine-corsa anti oraria 
            Folle();
            InvokeWriteLog("Gone to anti-clockwise endline");
        }

        public void Folle()
        {
            _reg[_bitA1] = false;
            _reg[_bitA2] = false;
            _reg[_bitB1] = false;
            _reg[_bitB2] = false;
            _reg.OutBits();
        }

        public int CalculateStepNumberAndGoToEnd()
        {
            PosizionaFineCorsaAntiOrario();
            Task.Delay(20).Wait();
            int res = 0;
            while (StepOrario(1)) res++;
            InvokeWriteLog("I just calculated steps number and went to end");
            return res;
        }
    }
}
