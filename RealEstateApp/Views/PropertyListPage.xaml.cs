using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class PropertyListPage : ContentPage
{
    PropertyListPageViewModel vm;
    public PropertyListPage(PropertyListPageViewModel vm)
    {
        InitializeComponent();
        this.vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        vm.GetPropertiesCommand.Execute(null); //TODO : 2.1.1
    }
}