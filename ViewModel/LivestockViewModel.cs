using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.Media.SpeechRecognition;

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
    private bool QueryCondition
    {
        get => !string.IsNullOrEmpty(QueryByType) 
            && !string.IsNullOrEmpty(QueryByColour);
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
    #endregion Query
    #region Insert
    private string produceName
    {
        get
        {
            switch (InsertType)
            {
                case "Cow": return "Milk";
                case "Sheep": return "Wool";
                default: return "Produce";
            }
        }
    }
    private void SetInsertVerifyInfo()
    {
        string? type = InsertType == null ? "Type not Selected" : "";
        string? cost = InsertCost == null ? "Empty Cost" : "";
        string? weight = InsertWeight == null ? "Empty Weight" : "";
        string? colour = InsertColour == null ? "Colour not Selected" : "";
        string? produce = InsertProduce == null ? $"Empty {produceName}" : "";
        InsertVerifyInfo = type + cost + weight + colour + produce;
    }
    private void SetInsertProducePlaceholder()
    {
        InsertProduce_Placeholder = $"Enter the Weight of its {produceName}";
    }
    private string _insertVerifyInfo;
    public string InsertVerifyInfo
    {
        get => _insertVerifyInfo;
        set
        {
            if (!_insertVerifyInfo.Equals(value))
            {
                _insertVerifyInfo = value;
                OnPropertyChanged();
            }
        }
    }
    private string _insertType;
    public string InsertType
    {
        get => _insertType;
        set
        {
            if (!_insertType.Equals(value))
            {
                _insertType = value;
                SetInsertVerifyInfo();
                SetInsertProducePlaceholder();
                OnPropertyChanged();
            }
        }
    }
    private string _insertCost;
    public string InsertCost
    {
        get => _insertCost;
        set
        {
            if (!_insertCost.Equals(value))
            {
                _insertCost = value;
                SetInsertVerifyInfo();
                OnPropertyChanged();
            }
        }
    }
    private string _insertWeight;
    public string InsertWeight
    {
        get => _insertWeight;
        set
        {
            if (!_insertWeight.Equals(value))
            {
                _insertWeight = value;
                SetInsertVerifyInfo();
                OnPropertyChanged();
            }
        }
    }
    private string _insertColour;
    public string InsertColour
    {
        get => _insertColour;
        set
        {
            if (!_insertColour.Equals(value))
            {
                _insertColour = value;
                SetInsertVerifyInfo();
                OnPropertyChanged();
            }
        }
    }
    private string _insertProduce_Placeholder;
    public string InsertProduce_Placeholder
    {
        get => _insertProduce_Placeholder;
        set
        {
            if (!_insertProduce_Placeholder.Equals(value))
            {
                _insertProduce_Placeholder = value;
                OnPropertyChanged();
            }
        }
    }
    private string _insertProduce;
    public string InsertProduce
    {
        get => _insertProduce;
        set
        {
            if (!_insertProduce.Equals(value))
            {
                _insertProduce = value;
                SetInsertVerifyInfo();
                OnPropertyChanged();
            }
        }
    }
    private string _insertResult;
    public string InsertResult
    {
        get => _insertResult;
        set
        {
            if (_insertResult.Equals(value))
            {
                _insertResult = value;
                OnPropertyChanged();
            }
        }
    }
    private bool InsertCondition
    {
        get => !string.IsNullOrEmpty(InsertType)
                && Util.VerifyFloat(InsertCost) != Util.bad_float
                && Util.VerifyFloat(InsertWeight) != Util.bad_float
                && !string.IsNullOrEmpty(InsertColour)
                && Util.VerifyFloat(InsertProduce) != Util.bad_float;
    }
    public ICommand InsertCommand { get; set; }
    #endregion Insert

    public LivestockViewModel()
    {
        Database database = new Database();
        _livestocks = new ObservableCollection<Livestock>();
        database.ToList().ForEach(livestock => { _livestocks?.Add(livestock); });
        Util util = new Util(this);
        _priceInfo = util.GetPricesInfo();
        _statisticsReport = util.GetStatisticsReport();
        // Query Command
        string defaultResult = "Empty Result";
        QueryResult = defaultResult;
        QueryCommand = new Command<int>((para) =>
        {
            if (para == 0)
            {
                QueryByType = null;
                QueryByColour = null;
                QueryVerifyInfo = string.Empty;
                QueryResult = defaultResult;
            }
            else if (para == 1)
            {
                if (QueryCondition)
                {
                    QueryResult = util.GetQueryDetail(QueryByType, QueryByColour);
                }
                else
                {
                    QueryResult = defaultResult;
                }
            }
        });
        // Insert Command
        InsertCommand = new Command<int>((para) =>
        {
            if (para == 0)
            {// reset
                InsertType = null;
                InsertCost = null;
                InsertWeight = null;
                InsertColour = null;
                InsertProduce = null;
                InsertVerifyInfo = string.Empty;
                InsertResult = defaultResult;
            }
            else if (para == 1)
            {
                bool inserted = false;

                if (InsertCondition)
                {// insert new livestock
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"New {InsertType} Added");
                    if (InsertType == "Cow")
                    {
                        Cow newCow = new Cow(InsertCost, InsertWeight, InsertColour, InsertProduce);
                        inserted = database.Insert(newCow) > 0;
                        sb.AppendLine(newCow.ToString());
                    }
                    else if (InsertType == "Sheep")
                    {
                        Sheep newSheep = new Sheep(InsertCost, InsertWeight, InsertColour, InsertProduce);
                        inserted = database.Insert(newSheep) > 0;
                        sb.AppendLine(newSheep.ToString());
                    }
                    // pop window remind success
                    Shell.Current.DisplayAlert("Notice", sb.ToString(), "Cancel");
                }
                else
                {// pop window remind failure
                    InsertResult = defaultResult;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("No Insertion have done");
                    sb.AppendLine(InsertVerifyInfo);
                    Shell.Current.DisplayAlert("Falure", sb.ToString(), "Cancel");
                }
            }
        });
    }
}
