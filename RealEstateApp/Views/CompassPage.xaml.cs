using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class CompassPage : ContentPage
{
	CompassPageViewModel vm;
	public CompassPage(CompassPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		this.vm = vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

		vm.ToggleCompassCommand.Execute(null);
    }
}