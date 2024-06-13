namespace Task4_MVVE.Pages;

public partial class OpeningPage : ContentPage
{
	public OpeningPage(OpeningViewModel ovm)
	{
		BindingContext = ovm;
		InitializeComponent();
	}
}