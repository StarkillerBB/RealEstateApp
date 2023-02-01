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

        vm.CheckConnectivityCommand.Execute(null); //TODO : 2.1.1
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        vm.CancelSaveCommand.Execute(null);
    }
}