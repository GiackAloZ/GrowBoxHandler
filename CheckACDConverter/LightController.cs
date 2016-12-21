using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckACDConverter
{
    public class LightController : MasterDevice
    {
        private StepperMotorDevice _blindingFence;
        private OnOffDevice _lightBulb;

        private int _stepTotali; //Numero di step presenti fra un fine corsa e l'altro   
        private int _percentuale; //Percentuale di illuminazione attuale
        private int _posizioneAttualeInStep; //Posizione attuale del servo in numero di step

        public int Percentuale
        {
            get
            {
                return _percentuale;
            }
        }

        public LightController() : base() { }

        public LightController(string n, StepperMotorDevice fence, OnOffDevice light) : base(n, new PhysicalDevice[] { fence, light })
        {
            _blindingFence = fence;
            _lightBulb = light;

            _lightBulb.Off();
            Task.Delay(20);
            _blindingFence.PosizionaFineCorsaAntiOrario();
            Task.Delay(20);
            _stepTotali = _blindingFence.CalculateStepNumberAndGoToEnd();
            _posizioneAttualeInStep = 0;
            _percentuale = 0;
            _blindingFence.Folle();
            InvokeWriteLog("Device setted up");
        }

        public void PercentualeApertura(int percentuale)
        {
            if (percentuale == 0)
            {
                _lightBulb.Off();
                Task.Delay(20);
                _blindingFence.PosizionaFineCorsaAntiOrario(); //Si posiziona a fine-corsa anti oraria chiude le persiane paralume
                Task.Delay(20);
                _posizioneAttualeInStep = 0;
            }
            else if (percentuale == 100)
            {
                _lightBulb.On();
                Task.Delay(20);
                _blindingFence.PosizionaFineCorsaAntiOrario(); //Si posiziona a fine-corsa oraria apre le persiane paralume
                Task.Delay(20);
                _posizioneAttualeInStep = _stepTotali;
            }
            else if ((percentuale < 100) && (percentuale > 0))
            {
                _lightBulb.On();
                Task.Delay(20);
                _blindingFence.PosizionaFineCorsaOrario(); //Si posiziona a fine-corsa anti oraria chiude le persiane paralume
                Task.Delay(20);
                _posizioneAttualeInStep = (Int32)(((double)_stepTotali / 100.0) * (double)percentuale);
                _blindingFence.StepAntiOrario(_posizioneAttualeInStep);
                Task.Delay(20);
            }
            _percentuale = percentuale;
            InvokeWriteLog("Setted to % -> " + _percentuale);
            _blindingFence.Folle();
        }

        public void ApriCompletamente()
        {
            PercentualeApertura(100);
        }

        public void ChiudiCompletamente()
        {
            PercentualeApertura(0);
        }

        public void OnLuce()
        {
            _lightBulb.On();
            if (_percentuale == 0) _percentuale = 1;
            InvokeWriteLog("Light on");
        }

        public void Incrementa()
        {
            if (_percentuale >= 100) return;
            if (_percentuale == 0)
                _lightBulb.On();
            _blindingFence.StepAntiOrario(1);
            _posizioneAttualeInStep++;
            _percentuale = (Int32)((100.0 / (double)_stepTotali) * (double)_posizioneAttualeInStep);
            InvokeWriteLog("Augmented to % -> " + _percentuale);
        }

        public void Decrementa()
        {
            if (_percentuale <= 0) return;
            _blindingFence.StepOrario(1);
            _posizioneAttualeInStep--;
            _percentuale = (Int32)((100.0 / (double)_stepTotali) * (double)_posizioneAttualeInStep);
            if (_percentuale == 0)
            {
                _lightBulb.Off();
            }
            InvokeWriteLog("Reduced to % -> " + _percentuale);
        }
    }
}
