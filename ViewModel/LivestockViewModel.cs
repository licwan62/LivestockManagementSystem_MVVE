namespace Task4_MVVE.ViewModel;

public partial class LivestockViewModel : ObservableObject
{
    Util util;
    Database database;
    public LivestockViewModel()
    {
        database = new Database();
        Livestocks = new ObservableCollection<Livestock>();
        database.ToList().ForEach(l => Livestocks?.Add(l));
        util = new Util(this);
        PricesInfo = util.GetPricesInfo();
        StatisticsReport = util.GetStatisticsReport();

        SetQVInfoCommand = new Command(SetQVInfo);
        //QueryCommand = new Command<string>(Query);

    }
    // Collection view
    [ObservableProperty] private ObservableCollection<Livestock> livestocks;
    public ObservableCollection<Cow> Cows { get => new ObservableCollection<Cow>(Livestocks.OfType<Cow>()); }
    public ObservableCollection<Sheep> Sheeps { get => new ObservableCollection<Sheep>(Livestocks.OfType<Sheep>()); }
    // Statistics reporting
    [ObservableProperty] private string pricesInfo, statisticsReport;
    // query form
    [ObservableProperty]
    private string[] types = { "Cow", "Sheep" },
        colours = { "Red", "Black", "White", "All" };
    [ObservableProperty] private string queryVerifyInfo, queryByType, queryByColour, queryResult;
    private bool QueryCondition
    {// 2 Pickers selected
        get => !string.IsNullOrEmpty(QueryByType)
            && !string.IsNullOrEmpty(QueryByColour);
    }
    public ICommand SetQVInfoCommand { get; }
    private void SetQVInfo()
    {
        string? type = (QueryByType == null) ? "Type not selected " : "";
        string? colour = (QueryByColour == null) ? "Colour not selected" : "";
        QueryVerifyInfo = type + colour;
    }
    // Insert form
    private readonly string defaultResult = "Empty Result";
    //public ICommand QueryCommand { get; }
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
                QueryResult = util.GetQueryDetail(QueryByType, QueryByColour);
            }
            else
            {
                QueryResult = defaultResult;
            }
        }
    }
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
                Shell.Current.DisplayAlert("Notice", sb.ToString(), "Cancel");
            }
        }
    }
    #endregion Insert
}
