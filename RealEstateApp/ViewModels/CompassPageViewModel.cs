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
    public class CompassPageViewModel : BaseViewModel
    {
		private Property property;

        public Property Property 
        { 
            get => property; 
            set 
            { 
                SetProperty(ref property, value); 
            } 
        }
        private string currentAspect;

        public string CurrentAspect
        {
            get { return currentAspect; }
            set => SetProperty(ref currentAspect, value);
        }

        private double rotationAngle;

        public double RotationAngle
        {
            get { return rotationAngle; }
            set => SetProperty(ref rotationAngle, value);
        }

        private string currentHeading;

        public string CurrentHeading
        {
            get { return currentHeading; }
            set => SetProperty(ref currentHeading, value);
        }

        private Command toggleCompassCommand;
        public ICommand ToggleCompassCommand => toggleCompassCommand ??= new Command(ToggleCompass);
        private void ToggleCompass()
        {
            if (Compass.Default.IsSupported)
            {
                if (!Compass.Default.IsMonitoring)
                {
                    // Turn on compass
                    Compass.Default.ReadingChanged += Compass_ReadingChanged;
                    Compass.Default.Start(SensorSpeed.UI);
                }
                else
                {
                    // Turn off compass
                    Compass.Default.Stop();
                    Compass.Default.ReadingChanged -= Compass_ReadingChanged;
                }
            }
        }

        private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
        {
            double reading = e.Reading.HeadingMagneticNorth;
            CurrentHeading = reading.ToString();
            RotationAngle = 360 - reading;
            Console.WriteLine(reading);
            if (reading >= 45 && reading <= 135)
                CurrentAspect = "Øst";
            else if (reading >= 135 && reading <= 225)
                CurrentAspect = "Syd";
            else if (reading >= 225 && reading <= 315)
                CurrentAspect = "Vest";
            else
                CurrentAspect = "Nord";
        }

        private Command returnToPageCommand;
        public ICommand ReturnToPageCommand => returnToPageCommand ??= new Command(async () => await ReturnToPage());

        async Task ReturnToPage()
        {
            Property.Aspect = CurrentAspect;
            ToggleCompass();
            await Shell.Current.GoToAsync("..");
        }


    }
}
