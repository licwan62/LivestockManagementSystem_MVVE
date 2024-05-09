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

    private void Insert_SenderChanged(object sender, EventArgs e)
    {
        if (sender.GetType() == typeof(Picker))
        {
            Debug.WriteLine(sender.ToString());
        }
    }

    private void Query_SenderChanged(object sender, EventArgs e)
    {
        if (sender.GetType() == typeof(Picker))
        {
            Debug.WriteLine(sender.ToString());
        }

    }
}