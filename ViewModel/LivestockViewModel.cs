namespace Task4_MVVE.ViewModel;

public partial class LivestockViewModel : ObservableObject
{
    Util _util;
    Database _database;
    readonly string defaultResult = "Empty Result";
    public LivestockViewModel()
    {
        Livestocks = new ObservableCollection<Livestock>();
        _database = new Database();
        _util = new Util(this);
        _database.ToList().ForEach(l => Livestocks?.Add(l));
        PricesInfo = _util.GetPricesInfo();
        StatisticsReport = _util.GetStatisticsReport();
        Init();
    }
    private void Init()
    {
        QueryResult = defaultResult;
        SetInsertProducePlaceholderCommand.Execute(null);
    }

    #region collection view
    [ObservableProperty] private ObservableCollection<Livestock> _livestocks;
    public ObservableCollection<Cow> Cows
    { get => new ObservableCollection<Cow>(Livestocks.OfType<Cow>()); }
    public ObservableCollection<Sheep> Sheeps
    { get => new ObservableCollection<Sheep>(Livestocks.OfType<Sheep>()); }
    #endregion collection view

    [ObservableProperty]// Statistics reporting
    private string _pricesInfo, _statisticsReport;

    #region query
    [ObservableProperty]// picker's items
    private string[] types = { "Cow", "Sheep" },
        colours = { "Red", "Black", "White", "All" };
    [ObservableProperty]
    private string _queryVerifyInfo,
        _queryByType, _queryByColour, _queryResult;
    private bool QueryCondition
    {// 2 Pickers selected
        get => !string.IsNullOrEmpty(QueryByType)
            && !string.IsNullOrEmpty(QueryByColour);
    }
    [RelayCommand]
    private void SetQueryVerifyInfo()
    {
        string? type = (QueryByType == null) ? "Type not selected " : "";
        string? colour = (QueryByColour == null) ? "Colour not selected" : "";
        QueryVerifyInfo = type + colour;
    }
    [RelayCommand]
    private void Query(string para)
    {
        QueryResult = defaultResult;
        if (para == "Reset")
        {
            QueryByType = null;
            QueryByColour = null;
            QueryVerifyInfo = string.Empty;
            QueryResult = defaultResult;
        }
        else if (para == "Apply")
        {
            if (QueryCondition)
            {
                QueryResult = _util.GetQueryDetail(QueryByType, QueryByColour);
            }
            else
            {
                QueryResult = defaultResult;
            }
        }
    }
    #endregion query
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
    [RelayCommand]
    private void SetInsertVerifyInfo()
    {
        string? type = string.IsNullOrEmpty(InsertType) ?
            "*Empty Type" : "";
        string? cost = string.IsNullOrEmpty(InsertCost) ?
            "*Empty Cost" : Util.VerifyFloat(InsertCost) == Util.bad_float ?
            "*Invalid Cost" : "";
        string? weight = string.IsNullOrEmpty(InsertWeight) ?
            "*Empty Weight" : Util.VerifyFloat(InsertWeight) == Util.bad_float ?
            "*Invalid Weight" : "";
        string? colour = string.IsNullOrEmpty(InsertColour) ?
            "*Empty Colour" : "";
        string? produce = string.IsNullOrEmpty(InsertProduce) ?
            $"*Empty {produceName}" : Util.VerifyFloat(InsertProduce) == Util.bad_float ?
            $"*Invalid {produceName}" : "";
        InsertVerifyInfo = string.Format("{0,-15}{1,-15}{2,-15}{3,-15}{4,-15}",
            type, cost, weight, colour, produce);
    }
    [RelayCommand]
    private void SetInsertProducePlaceholder()
    {
        InsertProduce_Placeholder = $"Enter the Weight of its {produceName}";
    }
    [ObservableProperty] private string insertVerifyInfo;
    [ObservableProperty] private string insertType;
    [ObservableProperty] private string insertCost;
    [ObservableProperty] private string insertWeight;
    [ObservableProperty] private string insertColour;
    [ObservableProperty] private string insertProduce_Placeholder;
    [ObservableProperty] private string insertProduce;
    [ObservableProperty] private string insertResult;
    private bool InsertCondition
    {
        get => !string.IsNullOrEmpty(InsertType)
                && Util.VerifyFloat(InsertCost) != Util.bad_float
                && Util.VerifyFloat(InsertWeight) != Util.bad_float
                && !string.IsNullOrEmpty(InsertColour)
                && Util.VerifyFloat(InsertProduce) != Util.bad_float;
    }
    [RelayCommand]
    private void Insert(string para)
    {
        if (para == "Reset")
        {// reset
            InsertType = null;
            InsertCost = null;
            InsertWeight = null;
            InsertColour = null;
            InsertProduce = null;
            InsertVerifyInfo = string.Empty;
            InsertResult = defaultResult;
        }
        else if (para == "Apply")
        {
            bool inserted = false;

            if (InsertCondition)
            {// insert new livestock
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"New {InsertType} Added");
                if (InsertType == "Cow")
                {
                    Cow newCow = new Cow(InsertCost, InsertWeight, InsertColour, InsertProduce);
                    inserted = _database.Insert(newCow) > 0;
                    sb.AppendLine(newCow.ToString());
                }
                else if (InsertType == "Sheep")
                {
                    Sheep newSheep = new Sheep(InsertCost, InsertWeight, InsertColour, InsertProduce);
                    inserted = _database.Insert(newSheep) > 0;
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
                Shell.Current.DisplayAlert("Notice", sb.ToString(), "Cancel");
            }
        }
    }
    #endregion Insert
}
