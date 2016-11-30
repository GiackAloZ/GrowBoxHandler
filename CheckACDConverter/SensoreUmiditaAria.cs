using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckACDConverter
{
    public class SensoreUmiditaAria : SensoreAnalogico
    {
        private const double A = 52.59517;
        private const double B = 15.46814;
        private const double C = 3.550823;
        private const double D = 73.81925;
        public SensoreUmiditaAria(MCP3208ADC c, int n) : base(c, n) { }

        private double ReadPercentUmidita()
        {
            double volt = ReadVolt();
            double perc = D + ((A - D) / (1 + Math.Pow((volt / C), B)));
            if (perc > 100) perc = 100;
            if (perc < 0) perc = 0;
            return perc;
        }

        public override double ReadValue()
        {
            return ReadPercentUmidita();
        }
    }
}
