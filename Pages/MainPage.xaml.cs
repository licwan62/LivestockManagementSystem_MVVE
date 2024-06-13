namespace Task4_MVVE.Pages;

public partial class MainPage : ContentPage
{
    MainViewModel vm;
    public MainPage(MainViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        this.vm = vm;
	}
}