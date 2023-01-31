﻿using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Google.Crypto.Tink.Mac;

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

            if (PropertiesCollection.Count != 0)
                PropertiesCollection.Clear();

            foreach (Property property in properties)
                PropertiesCollection.Add(new PropertyListItem(property));

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
