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

        vm.GetPropertiesCommand.Execute(null); //TODO : 2.1.1
    }
}