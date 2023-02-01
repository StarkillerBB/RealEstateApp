using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class PropertyDetailPage : ContentPage
{
	PropertyDetailPageViewModel vm;
	public PropertyDetailPage(PropertyDetailPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		this.vm = vm;
	}

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
		vm.StopSpeechCommand.Execute(null);

    }
}