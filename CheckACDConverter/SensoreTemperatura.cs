using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckACDConverter
{
    public class SensoreTemperatura : SensoreAnalogico
    {
        private const double A = -51.94713;
        private const double B = 0.304941;
        private const double C = 2.10E+10;
        private const double D = 152566.7;
        public SensoreTemperatura(MCP3208ADC c, int n) : base(c, n) { }

        private double ReadDegrees()
        {
            double volt = ReadVolt();
            return (D + ((A - D) / (1 + Math.Pow((ReadVolt() / C), B))));
        }

        public override double ReadValue()
        {
            return ReadDegrees();
        }
    }
}
