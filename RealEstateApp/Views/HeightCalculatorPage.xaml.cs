using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class HeightCalculatorPage : ContentPage
{
	HeightCalculatorPageViewModel vm;
	public HeightCalculatorPage(HeightCalculatorPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		this.vm = vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

		vm.ToggleBarometerCommand.Execute(null);
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        vm.ToggleBarometerCommand.Execute(null);
    }
}