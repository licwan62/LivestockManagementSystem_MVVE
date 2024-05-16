using System.Diagnostics;

namespace Task4_MVVE.Pages;

public partial class MainPage : ContentPage
{
    LivestockViewModel vm;
    public MainPage(LivestockViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        this.vm = vm;
	}
}