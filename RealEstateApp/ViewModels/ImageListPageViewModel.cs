using RealEstateApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RealEstateApp.ViewModels
{
    [QueryProperty(nameof(Property), "Property")]
    public class ImageListPageViewModel : BaseViewModel
    {
        

        private Property _property;

        public Property Property
        {
            get { return _property; }
            set { SetProperty(ref _property, value); }
        }

        private int _position;

        public int Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }


        private Command toggleShakeCommand;
        public ICommand ToggleShakeCommand => toggleShakeCommand ??= new Command(ToggleShake);
        private void ToggleShake()
        {
            if (Accelerometer.Default.IsSupported)
            {
                if (!Accelerometer.Default.IsMonitoring)
                {
                    // Turn on compass
                    Accelerometer.Default.ShakeDetected += Accelerometer_ShakeDetected;
                    Accelerometer.Default.Start(SensorSpeed.Game);
                }
                else
                {
                    // Turn off compass
                    Accelerometer.Default.Stop();
                    Accelerometer.Default.ShakeDetected -= Accelerometer_ShakeDetected;
                }
            }
        }

        private void Accelerometer_ShakeDetected(object sender, EventArgs e)
        {
            if (Position == Property.ImageUrls.Count() - 1)
                Position = 0;
            else
                Position++;
        }

    }
}
