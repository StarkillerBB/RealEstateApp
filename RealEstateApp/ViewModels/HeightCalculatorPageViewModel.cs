using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RealEstateApp.ViewModels
{
    public class HeightCalculatorPageViewModel : BaseViewModel
    {


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
        }
    }
}
