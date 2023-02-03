using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RealEstateApp.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {

        private double _volume;

        public double Volume
        {
            get { return _volume; }
            set { SetProperty(ref _volume, value); }
        }

        private double _pitch;

        public double Pitch
        {
            get { return _pitch; }
            set { SetProperty(ref _pitch, value); }
        }


        private Command setVolumeCommand;
        public ICommand SetVolumeCommand => setVolumeCommand ??= new Command(SetVolume);
        void SetVolume() => Preferences.Default.Set("volume", _volume);


        private Command setPitchCommand;
        public ICommand SetPitchCommand => setPitchCommand ??= new Command(SetPitch);
        void SetPitch() => Preferences.Default.Set("pitch", _pitch);
    }
}
