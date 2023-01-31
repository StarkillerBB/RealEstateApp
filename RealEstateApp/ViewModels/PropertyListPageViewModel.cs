using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;
public class PropertyListPageViewModel : BaseViewModel
{
    public ObservableCollection<PropertyListItem> PropertiesCollection { get; } = new();

    private readonly IPropertyService service;

    public PropertyListPageViewModel(IPropertyService service)
    {
        Title = "Property List";
        this.service = service;
    }

    bool isRefreshing = false; 
    public bool IsRefreshing
    {
        get => isRefreshing;
        set => SetProperty(ref isRefreshing, value);
    }

    private double _lastLatitude;

    public double LastLatitude
    {
        get { return _lastLatitude; }
        set { _lastLatitude = value; }
    }

    private double _lastLongitude;

    public double LastLongitude
    {
        get { return _lastLongitude; }
        set { _lastLongitude = value; }
    }

    private Command getPropertiesCommand; // TODO : 2.1.1
    public ICommand GetPropertiesCommand => getPropertiesCommand ??= new Command(async () => await GetPropertiesAsync()); // TODO : 2.1.1

    async Task GetPropertiesAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            IsRefreshing = true;

            List<Property> properties = service.GetProperties();
            List<PropertyListItem> sortList = new();

            if (PropertiesCollection.Count != 0)
                PropertiesCollection.Clear();

            foreach (Property property in properties)
                sortList.Add(new PropertyListItem(property));

            foreach (PropertyListItem item in sortList)
            {
                Location userLoc = new Location((double)LastLatitude, (double)LastLongitude);
                Location propertyLoc = new Location((double)item.Property.Latitude, (double)item.Property.Longitude);

                item.Distance = Location.CalculateDistance(userLoc, propertyLoc, DistanceUnits.Kilometers);
            }
            foreach (PropertyListItem item in sortList.OrderBy(x => x.Distance))
            {
                PropertiesCollection.Add(item);
            }
            

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get monkeys: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }


    private Command getLastLocation;
    public ICommand GetLastLocation => getLastLocation ??= new Command(async () => await SortLocation());
    public async Task<string> SortLocation()
    {
        try
        {
            Location location = await Geolocation.Default.GetLastKnownLocationAsync();

            if (location != null)
            {
                LastLatitude = location.Latitude;
                LastLongitude = location.Longitude;
            }
            else
            {
                Location newlocation = await Geolocation.Default.GetLocationAsync();
                LastLatitude = location.Latitude;
                LastLongitude = location.Longitude;
            }
        }
        catch (FeatureNotSupportedException fnsEx)
        {
            // Handle not supported on device exception
        }
        catch (FeatureNotEnabledException fneEx)
        {
            // Handle not enabled on device exception
        }
        catch (PermissionException pEx)
        {
            // Handle permission exception
        }
        catch (Exception ex)
        {
            // Unable to get location
        }
        finally
        {
            await GetPropertiesAsync();
        }

        return "None";
    }

    private Command goToDetailsCommand;
    public ICommand GoToDetailsCommand =>
                    goToDetailsCommand ??= new Command<PropertyListItem>(async (property) => await GoToDetails(property));
    async Task GoToDetails(PropertyListItem propertyListItem)
    {
        if (propertyListItem == null)
            return;

        await Shell.Current.GoToAsync(nameof(PropertyDetailPage), true, new Dictionary<string, object>
        {
            {"MyPropertyListItem", propertyListItem }
        });
    }

    private Command goToAddPropertyCommand;
    public ICommand GoToAddPropertyCommand => goToAddPropertyCommand ??= new Command(async () => await GotoAddProperty());
    async Task GotoAddProperty()
    {
        await Shell.Current.GoToAsync($"{nameof(AddEditPropertyPage)}?mode=newproperty", true, new Dictionary<string, object>
        {
            {"MyProperty", new Property() }
        });
    }
}
