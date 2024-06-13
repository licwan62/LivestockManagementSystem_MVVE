namespace Task4_MVVE.ViewModel;

public partial class OpeningViewModel : ObservableObject
{
    [ObservableProperty]
    string _data;
    [ObservableProperty]
    Livestock livestock;
    [ObservableProperty]
    string _type, _colour;
    [ObservableProperty]
    int _iD;
    [ObservableProperty]
    float _cost, _weight, _produce;
    public OpeningViewModel()
    {
        /*Type = livestock.Type;
        ID = livestock.Id;
        Colour = livestock.Colour;
        Cost = livestock.Cost;
        Weight = livestock.Weight;*/
    }
}
