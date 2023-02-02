using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class ImageListPage : ContentPage
{
	ImageListPageViewModel vm;
	public ImageListPage(ImageListPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		this.vm = vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

		vm.ToggleShakeCommand.Execute(null);
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        vm.ToggleShakeCommand.Execute(null);
    }
}