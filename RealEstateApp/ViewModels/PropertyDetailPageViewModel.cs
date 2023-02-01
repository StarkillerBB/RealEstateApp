using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

[QueryProperty(nameof(PropertyListItem), "MyPropertyListItem")]
public class PropertyDetailPageViewModel : BaseViewModel
{
    private readonly IPropertyService service;
    public PropertyDetailPageViewModel(IPropertyService service)
    {
        this.service = service;
    }

    Property property;
    public Property Property { get => property; set { SetProperty(ref property, value); } }


    Agent agent;
    public Agent Agent { get => agent; set { SetProperty(ref agent, value); } }


    PropertyListItem propertyListItem;
    public PropertyListItem PropertyListItem
    {
        set
        {
            SetProperty(ref propertyListItem, value);
           
            Property = propertyListItem.Property;
            Agent = service.GetAgents().FirstOrDefault(x => x.Id == Property.AgentId);
        }
    }

    private bool playIsVisible = true;

    public bool PlayIsVisible
    {
        get { return playIsVisible; }
        set { playIsVisible = value; }
    }

    private bool stopIsVisible;

    public bool StopIsVisible
    {
        get { return stopIsVisible; }
        set { stopIsVisible = value; }
    }



    private Command editPropertyCommand;
    public ICommand EditPropertyCommand => editPropertyCommand ??= new Command(async () => await GotoEditProperty());
    async Task GotoEditProperty()
    {
        await Shell.Current.GoToAsync($"{nameof(AddEditPropertyPage)}", true, new Dictionary<string, object>
        {
            {"MyProperty", property }
        });
    }

    private Command txtToSpeechCommand;
    public ICommand TxtToSpeechCommand => txtToSpeechCommand ??= new Command(async () => await TxtToSpeech());

    CancellationTokenSource cts;
    async Task TxtToSpeech()
    {
        cts = new CancellationTokenSource();
        PlayIsVisible = false;
        StopIsVisible = true;
        OnPropertyChanged(nameof(PlayIsVisible));
        OnPropertyChanged(nameof(StopIsVisible));
        await TextToSpeech.Default.SpeakAsync(property.Description, cancelToken: cts.Token);
        PlayIsVisible = true;
        StopIsVisible = false;
        OnPropertyChanged(nameof(PlayIsVisible));
        OnPropertyChanged(nameof(StopIsVisible));

    }


    private Command stopSpeechCommand;
    public ICommand StopSpeechCommand => stopSpeechCommand ??= new Command(() => StopSpeech());
    public void StopSpeech()
    {
        if (cts?.IsCancellationRequested ?? true)
            return;

        PlayIsVisible = true;
        StopIsVisible = false;
        OnPropertyChanged(nameof(PlayIsVisible));
        OnPropertyChanged(nameof(StopIsVisible));
        cts.Cancel();
    }

}
