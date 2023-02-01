using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class AddEditPropertyPage : ContentPage
{
    AddEditPropertyPageViewModel vm;

    public AddEditPropertyPage(AddEditPropertyPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        this.vm = vm;
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();

        vm.WatchBatteryCommand.Execute(null);
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        vm.WatchBatteryCommand.Execute(null);
    }
}