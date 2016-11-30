using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckACDConverter
{
    public class SensoreCO2 : SensoreAnalogico
    {
        private const double ZERO_POINT_VOLTAGE = 0.324; //define the output of the sensor in volts when the concentration of CO2 is 400PPM
        private const double REACTION_VOLTAGE = 0.020; //define the voltage drop of the sensor when move the sensor from air into 1000ppm CO2
        private double[] CO2Curve = { 2.602, ZERO_POINT_VOLTAGE, (REACTION_VOLTAGE / (2.602 - 3.0)) };
        private const double DC_GAIN = 8.5;   //define the DC gain of amplifier
        public SensoreCO2(MCP3208ADC c, int n) : base(c, n) { }

        private double ReadPPMCO2()
        {
            double volts = ReadVolt();
            return Math.Pow(10.0, ((volts / DC_GAIN) - CO2Curve[1]) / CO2Curve[2] + CO2Curve[0]);
        }

        public override double ReadValue()
        {
            return ReadPPMCO2();
        }
    }
}
