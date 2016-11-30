using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml.Controls;
using Windows.Devices;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;

namespace CheckACDConverter
{
    public class MCP3208ADC
    {
        private Int32 SPI_CHIP_SELECT_LINE;       /* Line 0 maps to physical pin number 24 on the Rpi2        */
        private SpiDevice SpiADC;

        public MCP3208ADC(Int32 gpio)
        {
            SPI_CHIP_SELECT_LINE = gpio;
            Init();
        }

        /* Initialize GPIO and SPI */
        private async void Init()
        {
            try
            {
                await InitSPI();    /* Initialize the SPI bus for communicating with the ADC      */

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task InitSPI()
        {
            try
            {
                var settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE);
                settings.ClockFrequency = 500000;   /* 0.5MHz clock rate                                        */
                settings.Mode = SpiMode.Mode0;      /* The ADC expects idle-low clock polarity so we use Mode0  */

                var controller = await SpiController.GetDefaultAsync();
                SpiADC = controller.GetDevice(settings);
            }

            /* If initialization fails, display the exception and stop running */
            catch (Exception ex)
            {
                throw new Exception("SPI Initialization Failed", ex);
            }
        }



        public int ReadADC(int ch)
        {
            int adcValue;

            byte[] readBuffer = new byte[3]; /* Buffer to hold read data*/
            byte[] writeBuffer = new byte[3] { 0x00, 0x00, 0x00 };

            writeBuffer[0] = 0x06;

            switch (ch)
            {
                case 0:
                    writeBuffer[0] = 0x06;
                    writeBuffer[1] = 0x00;
                    break;
                case 1:
                    writeBuffer[0] = 0x06;
                    writeBuffer[1] = 0x40;
                    break;
                case 2:
                    writeBuffer[0] = 0x06;
                    writeBuffer[1] = 0x80;
                    break;
                case 3:
                    writeBuffer[0] = 0x06;
                    writeBuffer[1] = 0xc0;
                    break;
                case 4:
                    writeBuffer[0] = 0x07;
                    writeBuffer[1] = 0x00;
                    break;
                case 5:
                    writeBuffer[0] = 0x07;
                    writeBuffer[1] = 0x40;
                    break;
                case 6:
                    writeBuffer[0] = 0x07;
                    writeBuffer[1] = 0x80;
                    break;
                case 7:
                    writeBuffer[0] = 0x07;
                    writeBuffer[1] = 0xc0;
                    break;
            }
            SpiADC.TransferFullDuplex(writeBuffer, readBuffer); /* Read data from the ADC                           */
            adcValue = convertToInt(readBuffer);                /* Convert the returned bytes into an integer value */
            return (adcValue);
        }

        /* Convert the raw ADC bytes to an integer */
        private int convertToInt(byte[] data)
        {
            int result = 0;

            result = data[1] & 0x0F;
            result <<= 8;
            result += data[2];

            return result;
        }
    }
}
