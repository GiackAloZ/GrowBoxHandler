using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckACDConverter
{
    public class SensoreUmiditaTerreno : SensoreAnalogico
    {
        private const double MINVOLT = 2.25;   // Min volt allagato     
        private const double MAXVOLT = 3.3;    // Max volt secco
        public SensoreUmiditaTerreno(MCP3208ADC c, int n) : base(c, n) { }

        private double ReadPercentUmidita()
        {
            //Considera il sensore lineare ma è da verificare
            double umiditaTerreno = 100.0 - ((ReadVolt() - MINVOLT) * (100.0 / (MAXVOLT - MINVOLT)));
            //Tronca eventuali imprecisioni del convertitore A/D
            if (umiditaTerreno > 100.0) umiditaTerreno = 100;
            if (umiditaTerreno < 0.0) umiditaTerreno = 0;

            return umiditaTerreno;
        }

        public override double ReadValue()
        {
            return ReadPercentUmidita();
        }
    }
}
