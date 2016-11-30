using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckACDConverter
{
    public class SensoreAnalogico
    {
        private MCP3208ADC _converter;
        private int CHANNEL_NUMBER;

        public SensoreAnalogico(MCP3208ADC c, int ch)
        {
            _converter = c;
            CHANNEL_NUMBER = ch;
        }

        protected double ReadVolt()
        {
            int sum = 0;
            for(int i = 0; i < 10; i++)
                sum += _converter.ReadADC(CHANNEL_NUMBER);
            return (sum / 10) * (3.3 / 4096);
        }

        public virtual double ReadValue()
        {
            return ReadVolt();
        }
    }
}
