using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Task4_MVVE.ViewModel;

public class LivestockViewModel : INotifyPropertyChanged
{
    // when any property changed, raise the event otherwise binding system not aware to updata in UI
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string name = "")// attribute automatically obtain the caller name
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    #region CollectionView
    private ObservableCollection<Livestock> _livestocks;
    public ObservableCollection<Livestock> Livestocks
    {
        get { return _livestocks; }
        set
        {
            if (_livestocks != value)
            {
                _livestocks = value;
                OnPropertyChanged();
            }
        }
    }
    public ObservableCollection<Cow> Cows
    {
        get
        {
            IEnumerable<Cow> temp = Livestocks.OfType<Cow>();
            return new ObservableCollection<Cow>(temp);
        }
    }
    public ObservableCollection<Sheep> Sheeps
    {
        get
        {
            IEnumerable<Sheep> temp = Livestocks.OfType<Sheep>();
            return new ObservableCollection<Sheep>(temp);
        }
    }
    #endregion CollectionView
    #region PriceInfo and Report
    private string _priceInfo;
    public string PriceInfo { get => _priceInfo; }
    private string _statisticsReport;
    public string StatisticsReport { get => _statisticsReport; }
    #endregion PriceInfo and Report
    #region Query
    private string _queryVerifyInfo;
    public string QueryVerifyInfo
    {
        get => _queryVerifyInfo;
        set
        {
            if (_queryVerifyInfo != value)
            {
                _queryVerifyInfo = value;
                OnPropertyChanged();
            }
        }
    }
    private void SetQueryVerifyInfo()
    {
        string? type = (QueryByType == null) ? "Type not selected " : "";
        string? colour = (QueryByColour == null) ? "Colour not selected" : "";
        QueryVerifyInfo = type + colour;
    }
    public string[] _types = { "Cow", "Sheep" };
    public string[] Types { get => _types; }
    private string _queryByType;
    public string QueryByType
    {
        get => _queryByType;
        set
        {
            if (_queryByType != value)
            {
                _queryByType = value;
                SetQueryVerifyInfo();
                OnPropertyChanged();
            }
        }
    }
    private string[] _colours = { "Red", "Black", "White", "All" };
    public string[] Colours { get => _colours; }
    private string _queryByColour;
    public string QueryByColour
    {
        get => _queryByColour;
        set
        {
            if (value != _queryByColour)
            {
                _queryByColour = value;
                SetQueryVerifyInfo();
                OnPropertyChanged();
            }
        }
    }
    private string _queryResult;
    public string QueryResult
    {
        get => _queryResult;
        set
        {
            if (_queryResult != value)
            {
                _queryResult = value;
                OnPropertyChanged();
            }
        }
    }
    public ICommand QueryCommand { get; set; }
    public LivestockViewModel()
    {
        Database database = new Database();
        _livestocks = new ObservableCollection<Livestock>();
        database.ToList().ForEach(livestock => { _livestocks?.Add(livestock); });
        Util util = new Util(this);
        _priceInfo = util.GetPricesInfo();
        _statisticsReport = util.GetStatisticsReport();
        #region Query
        string defaulResult = "Empty Result";
        QueryResult = defaulResult;
        QueryCommand = new Command<string>((para) =>
        {
        if (para == "Reset")
        {
            QueryByType = null;
            QueryByColour = null;
            QueryResult = string.Empty;
            QueryVerifyInfo = string.Empty;
            QueryResult = defaulResult;
        }
        else if (para == "Apply")
        {
            bool condition = !string.IsNullOrEmpty(QueryByType) && !string.IsNullOrEmpty(QueryByColour);
                if (condition)
                {
                    QueryResult = util.GetQueryDetail(QueryByType, QueryByColour);
                }
                else
                {
                    QueryResult = defaulResult;
                }
            }
        });
    }
    #endregion Query
}
