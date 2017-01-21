using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Newtonsoft.Json;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CheckACDConverter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private FileLogger _physicalFL;
        private FileLogger _masterFL;
        private const string PHYSICAL_LOG_FILE_NAME = "physicalLog.txt";
        private const string MASTER_LOG_FILE_NAME = "masterLog.txt";

        private const int SERIAL_NUMBER = 0;
        private const int LUM_SHIFT_NUMBER = 0;
        private const int CO2_SHIFT_NUMBER = 3;
        private const int UA_SHIFT_NUMBER = 4;
        private const int UT_SHIFT_NUMBER = 2;
        private const int TEMP_SHIFT_NUMBER = 1;

        private MCP3208ADC _converter;

        private StepperMotorDevice _lightFence;
        private const int A1_STEPPER_PIN = 3;
        private const int A2_STEPPER_PIN = 4;
        private const int B1_STEPPER_PIN = 5;
        private const int B2_STEPPER_PIN = 6;
        private const int STOP_ORARIO_PIN = 21;
        private const int STOP_ANTIORARIO_PIN = 20;
        private OnOffDevice _lightBulb;
        private const int LIGHT_BULB_PIN = 7;
        private LightController _lightController;

        private PwmDevice _heatResistor;
        private const int HEAT_RES_PIN = 12;

        private PwmDevice _airCicleFan;
        private const int AIR_CIRLE_FAN_PIN = 13;

        private SensoreAnalogico[] _sensori;
        private const int LUM_ARRAY_NUMBER = 0;
        private const int CO2_ARRAY_NUMBER = 1;
        private const int UA_ARRAY_NUMBER = 2;
        private const int UT_ARRAY_NUMBER = 3;
        private const int TEMP_ARRAY_NUMBER = 4;

        private const int DATA_SHIFT_REGISTER_PIN = 25;
        private const int CLOCK_SHIFT_REGISTER_PIN = 24;
        private const int LATCH_SHIFT_REGISTER_PIN = 23;

        private const int LCD_BIT0_PIN = 12;
        private const int LCD_BIT1_PIN = 11;
        private const int LCD_BIT2_PIN = 10;
        private const int LCD_BIT3_PIN = 9;
        private const int LCD_ENABLE_PIN = 13;
        private const int LCD_REGISTER_SELECT_PIN = 14;

        private const int SHIFT_REGISTER_BUS_LENGHT = 16;

        private ShiftRegister _shiftR;
        private LCD _lcd;

        private DispatcherTimer _printStatus = new DispatcherTimer();
        private const double CHANGE_STATUS_MILLISECONDS_TIMER = 2000;
        private const int STATUS_NUMBER = 5;
        private int _currentStatus = 0;
        private string[] STATUS_PRE_TEXT_MESSAGES = {
            "Luminosita'",
            "CO2",
            "Umidita' aria",
            "Umidita' terreno",
            "Temperatura"
        };
        private string[] STATUS_POST_TEXT_MESSAGES = {
            " lux",
            " PPM",
            " %",
            " %",
            " C"
        };

        public MainPage()
        {
            this.InitializeComponent();

            _physicalFL = new FileLogger(PHYSICAL_LOG_FILE_NAME);
            _masterFL = new FileLogger(MASTER_LOG_FILE_NAME);

            _converter = new MCP3208ADC(SERIAL_NUMBER);

            _shiftR = new ShiftRegister(SHIFT_REGISTER_BUS_LENGHT, DATA_SHIFT_REGISTER_PIN, CLOCK_SHIFT_REGISTER_PIN, LATCH_SHIFT_REGISTER_PIN);
            _lcd = new LCD(_shiftR, LCD_BIT0_PIN, LCD_BIT1_PIN, LCD_BIT2_PIN, LCD_BIT3_PIN, LCD_ENABLE_PIN, LCD_REGISTER_SELECT_PIN);

            _sensori = new SensoreAnalogico[]
            {
                new SensoreLuminosita(_converter, LUM_SHIFT_NUMBER),
                new SensoreCO2(_converter, CO2_SHIFT_NUMBER),
                new SensoreUmiditaAria(_converter, UA_SHIFT_NUMBER),
                new SensoreUmiditaTerreno(_converter, UT_SHIFT_NUMBER),
                new SensoreTemperatura(_converter, TEMP_SHIFT_NUMBER)
            };

            _printStatus.Interval = TimeSpan.FromMilliseconds(CHANGE_STATUS_MILLISECONDS_TIMER);
            _printStatus.Tick += PrintStatus_Tick;
            _printStatus.Start();

            _lightFence = new StepperMotorDevice("Blinding Fence", _shiftR, A1_STEPPER_PIN, A2_STEPPER_PIN, B1_STEPPER_PIN, B2_STEPPER_PIN, STOP_ORARIO_PIN, STOP_ANTIORARIO_PIN);
            _lightFence.WriteLog += WritePhysicalLogAsync;
            _lightBulb = new OnOffDevice("Agro Light Bulb", _shiftR, LIGHT_BULB_PIN);
            _lightBulb.WriteLog += WritePhysicalLogAsync;
            _lightController = new LightController("Light Controller", _lightFence, _lightBulb);
            _lightController.WriteLog += WriteMasterLogAsync;

            _heatResistor = new PwmDevice("Heat Resistor", HEAT_RES_PIN);
            _heatResistor.WriteLog += WritePhysicalLogAsync;

            _airCicleFan = new PwmDevice("Air Circle Fan", AIR_CIRLE_FAN_PIN);
            _airCicleFan.WriteLog += WritePhysicalLogAsync;

            _airCicleFan.ChangeDutyCycle(50);
            _airCicleFan.Start();
            _heatResistor.ChangeDutyCycle(50);
            _heatResistor.Start();
            Task.Delay(1000).Wait();
            _airCicleFan.Stop();
            _heatResistor.Stop();

            txtStatus.Text = "Running...";
        }

        private async void WritePhysicalLogAsync(string s)
        {
            await _physicalFL.WriteLogAsync(s);
        }

        private async void WriteMasterLogAsync(string s)
        {
            await _masterFL.WriteLogAsync(s);
        }

        private void PrintStatus_Tick(object sender, object e)
        {
            _lcd.Clear();
            if (_currentStatus >= STATUS_NUMBER)
                _currentStatus = 0;
            _lcd.SetCursor(0, 0);
            _lcd.Print(STATUS_PRE_TEXT_MESSAGES[_currentStatus]);
            _lcd.SetCursor(1, 0);
            _lcd.Print(_sensori[_currentStatus].ReadValue().ToString("F2") + STATUS_POST_TEXT_MESSAGES[_currentStatus]);
            _currentStatus++;
        }

        private void btnLum_Click(object sender, RoutedEventArgs e)
        {
            txtLum.Text = _sensori[LUM_ARRAY_NUMBER].ReadValue().ToString() + " lux";
        }

        private void btnCO2_Click(object sender, RoutedEventArgs e)
        {
            txtCO2.Text = _sensori[CO2_ARRAY_NUMBER].ReadValue().ToString() + " PPM";
        }

        private void btnAir_Click(object sender, RoutedEventArgs e)
        {
            txtAir.Text = _sensori[UA_ARRAY_NUMBER].ReadValue().ToString() + "%";
        }

        private void btnTerr_Click(object sender, RoutedEventArgs e)
        {
            txtTerr.Text = _sensori[UT_ARRAY_NUMBER].ReadValue().ToString() + "%";
        }

        private void btnTemp_Click(object sender, RoutedEventArgs e)
        {
            txtTemp.Text = _sensori[TEMP_ARRAY_NUMBER].ReadValue().ToString() + " °C";
        }
    }
}
