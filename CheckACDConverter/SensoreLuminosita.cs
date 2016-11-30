using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckACDConverter
{
    public class SensoreLuminosita : SensoreAnalogico
    {
        private const double A = 3310.333;
        private const double B = 3.276211;
        private const double C = 0.393569;
        private const double D = 36.96403;
        public SensoreLuminosita(MCP3208ADC c, int n) : base(c, n) { }

        private double ReadLux()
        {
            double volt = ReadVolt();
            return D + ((A - D) / (1 + Math.Pow((volt / C), B)));
        }

        public override double ReadValue()
        {
            return ReadLux();
        }
    }
}
