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
    private void Query_SenderChanged(object sender, EventArgs e)
    {
        vm.SetQueryVerifyInfoCommand.Execute(null);
    }

    private void Insert_SenderChanged(object sender, EventArgs e)
    {
        vm.SetInsertVerifyInfoCommand.Execute(null);
        vm.SetInsertProducePlaceholderCommand.Execute(null);
    }

    private void Delete_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        vm.SetDeleteVerifyInfoCommand.Execute(null);
    }
}