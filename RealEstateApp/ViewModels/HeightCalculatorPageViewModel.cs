using RealEstateApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RealEstateApp.ViewModels
{
    public class HeightCalculatorPageViewModel : BaseViewModel
    {

        public ObservableCollection<BarometerMeasurement> BarometerMeasurements { get; } = new();

        private BarometerMeasurement _barometerObj;
        public BarometerMeasurement BarometerObj
        {
            get { return _barometerObj; }
            set { SetProperty(ref _barometerObj, value); }
        }

        private string _measurementLabel;

        public string MeaurementLabel
        {
            get { return _measurementLabel; }
            set { _measurementLabel = value; }
        }


        double currentPressure;
        public double CurrentPressure
        {
            get { return currentPressure; }
            set { SetProperty(ref currentPressure, value); }
        }

        double currentAltitude;
        public double CurrentAltitude
        {
            get { return currentAltitude; }
            set { SetProperty(ref currentAltitude, value); }
        }

        private Command saveBarometerCommand;
        public ICommand SaveBarometerCommand => saveBarometerCommand ??= new Command(SaveMeasurement);
        public void SaveMeasurement()
        {
            double oldAltitude;
            if (BarometerObj != null)
            {
                oldAltitude = BarometerObj.Altitude;
                BarometerObj = new();
                BarometerObj.HeightChange = oldAltitude - CurrentAltitude;
            }
            else
                BarometerObj = new();

            BarometerObj.Pressure = CurrentPressure;
            BarometerObj.Altitude = CurrentAltitude;
            BarometerObj.Label = MeaurementLabel;

            BarometerMeasurements.Add(BarometerObj);
        }

        private Command toggleBarometerCommand;
        public ICommand ToggleBarometerCommand => toggleBarometerCommand ??= new Command(ToggleBarometer);

        public void ToggleBarometer()
        {
            if (Barometer.Default.IsSupported)
            {
                if (!Barometer.Default.IsMonitoring)
                {
                    // Turn on accelerometer
                    Barometer.Default.ReadingChanged += Barometer_ReadingChanged;
                    Barometer.Default.Start(SensorSpeed.UI);
                }
                else
                {
                    // Turn off accelerometer
                    Barometer.Default.Stop();
                    Barometer.Default.ReadingChanged -= Barometer_ReadingChanged;
                }
            }
        }

        private void Barometer_ReadingChanged(object sender, BarometerChangedEventArgs e)
        {
            // Update UI Label with barometer state
            CurrentPressure = e.Reading.PressureInHectopascals;
            Console.WriteLine(CurrentPressure);

            CurrentAltitude = 44307.694 * (1 - Math.Pow(currentPressure / 1013.9, 0.190284));
            Console.WriteLine(CurrentAltitude);
        }
    }
}
