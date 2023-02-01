using RealEstateApp.Models;
using RealEstateApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

[QueryProperty(nameof(Mode), "mode")]
[QueryProperty(nameof(Property), "MyProperty")]
public class AddEditPropertyPageViewModel : BaseViewModel
{
    readonly IPropertyService service;
    private CancellationTokenSource _cancelTokenSource;
    private bool _isCheckingLocation;

    public AddEditPropertyPageViewModel(IPropertyService service)
    {
        this.service = service;
        Agents = new ObservableCollection<Agent>(service.GetAgents());
    }

    public string Mode { get; set; }

    #region PROPERTIES
    public ObservableCollection<Agent> Agents { get; }

    private Property _property;
    public Property Property
    {
        get => _property;
        set
        {
            SetProperty(ref _property, value);
            Title = Mode == "newproperty" ? "Add Property" : "Edit Property";

            if (_property.AgentId != null)
            {
                SelectedAgent = Agents.FirstOrDefault(x => x.Id == _property?.AgentId);
            }
        }
    }

    private Agent _selectedAgent;
    public Agent SelectedAgent
    {
        get => _selectedAgent;
        set
        {
            SetProperty(ref _selectedAgent, value); // TODO : 2.1.4

            if (Property != null)
            {
                _selectedAgent = value;
                Property.AgentId = _selectedAgent?.Id;
            }
        }
    }

    bool flashlightToggle = false;
    public bool FlashlightToggle
    {
        get { return flashlightToggle; }
        set { SetProperty(ref flashlightToggle, value); }
    }


    string statusMessage;
    public string StatusMessage
    {
        get { return statusMessage; }
        set { SetProperty(ref statusMessage, value); }
    }

    Color statusColor;
    public Color StatusColor
    {
        get { return statusColor; }
        set { SetProperty(ref statusColor, value); }
    }
    #endregion



    private Command watchBatteryCommand;
    public ICommand WatchBatteryCommand => watchBatteryCommand ??= new Command(() => WatchBattery());
    private bool _isBatteryWatched;

    private void WatchBattery()
    {

        if (!_isBatteryWatched)
        {
            Battery.Default.BatteryInfoChanged += Battery_BatteryInfoChanged;
        }
        else
        {
            Battery.Default.BatteryInfoChanged -= Battery_BatteryInfoChanged;
        }

        _isBatteryWatched = !_isBatteryWatched;
    }

    private void Battery_BatteryInfoChanged(object sender, BatteryInfoChangedEventArgs e)
    {
        if (e.ChargeLevel < 0.20)
        {
            StatusMessage = "Battery is low";

            if (Battery.State == BatteryState.Charging)
            {
                StatusColor = Colors.Yellow;
            }
            else if (Battery.EnergySaverStatus == EnergySaverStatus.On)
            {
                StatusColor = Colors.Green;
            }
            else
                StatusColor = Colors.Red;
        }
        else
        {
            StatusMessage = string.Empty;

        }
    }

    private Command toggleFlashlightCommand;
    public ICommand ToggleFlashlightCommand => toggleFlashlightCommand ??= new Command(async () => await Flashlight_Toggled());
    private async Task Flashlight_Toggled()
    {
        try
        {
            if (!FlashlightToggle)
                await Flashlight.Default.TurnOnAsync();
            else
                await Flashlight.Default.TurnOffAsync();

            FlashlightToggle = !FlashlightToggle;
        }
        catch (FeatureNotSupportedException ex)
        {
            await Shell.Current.DisplayAlert("Alert", "Flashlight is not supported", "Ok");
            // Handle not supported on device exception
        }
        catch (PermissionException ex)
        {
            await Shell.Current.DisplayAlert("Alert", "Doesn't have permission to use flashlight", "Ok");
            // Handle permission exception
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Alert", "An error has occured, please try again", "Ok");
            // Unable to turn on/off flashlight
        }
    }

    private Command savePropertyCommand;
    public ICommand SavePropertyCommand => savePropertyCommand ??= new Command(async () => await SaveProperty());
    private async Task SaveProperty()
    {
        if (IsValid() == false)
        {
           StatusMessage = "Please fill in all required fields";
            StatusColor = Colors.Red;
        }
        else
        {
            service.SaveProperty(Property);
            await Shell.Current.GoToAsync("///propertylist");
        }
    }

    public bool IsValid()
    {
        if (string.IsNullOrEmpty(Property.Address)
            || Property.Beds == null
            || Property.Price == null
            || Property.AgentId == null)
            return false;
        return true;
    }

    private Command getLocation;
    public ICommand GetLocation => getLocation ??= new Command(async () => await GetCurrentLocation());

    public async Task GetCurrentLocation()
    {
        try
        {
            _isCheckingLocation = true;

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if (location != null)
            {
                _property.Latitude = location.Latitude;
                _property.Longitude = location.Longitude;
                OnPropertyChanged(nameof(Property));

                _property.Address = await GetGeocodeReverseData((double)_property.Latitude, (double)_property.Longitude);
            }

        }
        catch (Exception ex)
        {
            // DisplayAlert
        }
        finally
        {
            _isCheckingLocation = false;
        }
    }

    private async Task<string> GetGeocodeReverseData(double latitude, double longitude)
    {
        IEnumerable<Placemark> placemarks = await Geocoding.Default.GetPlacemarksAsync(latitude, longitude);

        Placemark placemark = placemarks?.FirstOrDefault();

        if (placemark != null)
        {
            return $"{placemark.FeatureName} {placemark.Thoroughfare} {placemark.Locality} {placemark.PostalCode}";
        }

        return "";
    }


    private Command cancelSaveCommand;
    public ICommand CancelSaveCommand => cancelSaveCommand ??= new Command(async () => await Shell.Current.GoToAsync(".."));




}
