using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Task4_MVVE.Model;

internal class LivestockViewModel : INotifyPropertyChanged
{
    // when any property changed, raise the event otherwise binding system not aware to updata in UI
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string name = "")// attribute automatically obtain the caller name
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
