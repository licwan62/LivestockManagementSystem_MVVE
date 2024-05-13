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
    void Init()
    {
        QueryResult = defaultResult;
        SetInsertProducePlaceholderCommand.Execute(null);
    }

    #region collection view
    [ObservableProperty] ObservableCollection<Livestock> _livestocks;
    public ObservableCollection<Cow> Cows
    { get => new ObservableCollection<Cow>(Livestocks.OfType<Cow>()); }
    public ObservableCollection<Sheep> Sheeps
    { get => new ObservableCollection<Sheep>(Livestocks.OfType<Sheep>()); }
    #endregion collection view

    [ObservableProperty]// Statistics reporting
    string _pricesInfo, _statisticsReport;
    [RelayCommand]
    void Reset(string section)
    {
        switch (section)
        {
            case "Query":
                QueryByType = null;
                QueryByColour = null;
                QueryVerifyInfo = string.Empty;
                QueryResult = defaultResult; break;
            case "Insert":
                InsertType = null;
                InsertCost = null;
                InsertWeight = null;
                InsertColour = null;
                InsertProduce = null;
                InsertResult = defaultResult; break;
            case "Delete":
                DeleteID = null;
                DeleteResult = defaultResult; break;
        }
    }

    #region query
    [ObservableProperty]
    string[] types = { "Cow", "Sheep" },
        colours = { "Red", "Black", "White", "All" };

    [ObservableProperty]// strings to show verifyMSG and result
    string _queryVerifyInfo, _queryResult;
    [RelayCommand]
    void SetQueryVerifyInfo()
    {// set verifyMSG, invoke when senders changed
        string? type = (QueryByType == null) ? "Type not selected " : "";
        string? colour = (QueryByColour == null) ? "Colour not selected" : "";
        QueryVerifyInfo = type + colour;
    }
    [ObservableProperty]// binding with pickers item
    [NotifyCanExecuteChangedFor(nameof(QueryCommand))]
    string? _queryByType, _queryByColour;
    [RelayCommand(CanExecute = nameof(CanQuery))]
    public void Query()
    {
        QueryResult = _util.GetQueryDetail(QueryByType, QueryByColour);
    }
    // determine Query function enable, called when NotifyCanExecuteChanged
    bool CanQuery() => !string.IsNullOrEmpty(QueryByType)
            && !string.IsNullOrEmpty(QueryByColour);
    #endregion query
    #region Insert
    string produceName
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
    void SetInsertVerifyInfo()
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
        InsertVerifyInfo = string.Format("{0}{1}{2}{3}{4}",
            type, cost, weight, colour, produce);
    }
    [RelayCommand]
    void SetInsertProducePlaceholder()
    {
        InsertProduce_Placeholder = $"Enter the Weight of its {produceName}";
    }
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(InsertCommand))]
    string _insertVerifyInfo, _insertType, _insertCost,
     _insertWeight, _insertColour, _insertProduce_Placeholder, _insertProduce, _insertResult;

    [RelayCommand(CanExecute = nameof(CanInsert))]
    async void Insert()
    {
        bool inserted = false;// indicate livestock is added
        // generate notice
        StringBuilder notice = new StringBuilder();
        notice.AppendLine($"New {InsertType} to be Added");
        if (InsertType == "Cow")
        {
            Cow newCow = new Cow(InsertCost, InsertWeight, InsertColour, InsertProduce);
            notice.AppendLine(newCow.ToString());
        }
        else if (InsertType == "Sheep")
        {
            Sheep newSheep = new Sheep(InsertCost, InsertWeight, InsertColour, InsertProduce);
            notice.AppendLine(newSheep.ToString());
        }
        // Display alert, operate, generate result
        StringBuilder result = new StringBuilder();
        bool toInsert =
           await Shell.Current.DisplayAlert("Notice", notice.ToString(), "Continue", "Cancel");
        if (toInsert)
        {
            if (InsertType == "Cow")
            {
                Cow newCow = new Cow(InsertCost, InsertWeight, InsertColour, InsertProduce);
                inserted = _database.Insert(newCow) > 0;
                result.AppendLine("New Cow Added");
                result.AppendLine(newCow.ToString());
            }
            else if (InsertType == "Sheep")
            {
                Sheep newSheep = new Sheep(InsertCost, InsertWeight, InsertColour, InsertProduce);
                inserted = _database.Insert(newSheep) > 0;
                result.AppendLine("New Sheep Added");
                result.AppendLine(newSheep.ToString());
            }
        }
        if (inserted)
        {
            InsertResult = result.ToString();
        }
        else
        {
            InsertResult = "Nothing Added";
        }
    }
    bool CanInsert()
    {
        return !string.IsNullOrEmpty(InsertType)
                && Util.VerifyFloat(InsertCost) != Util.bad_float
                && Util.VerifyFloat(InsertWeight) != Util.bad_float
                && !string.IsNullOrEmpty(InsertColour)
                && Util.VerifyFloat(InsertProduce) != Util.bad_float;
    }
    #endregion Insert
    #region Delete
    [RelayCommand]// set verifyMSG, invoked when entry for ID changed
    void SetDeleteVerifyInfo()
    {
        string info = string.IsNullOrEmpty(DeleteID) ?
            "Empty ID" : Util.VerifyInt(DeleteID) == Util.bad_int ?
            "Invalid ID" : "";
        DeleteVerifyInfo = info;
    }
    [ObservableProperty]
    string _deleteVerifyInfo, _deleteResult;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    string _deleteID;

    [RelayCommand(CanExecute = nameof(CanDelete))]
    async void Delete()
    {
        bool deleted = false;
        int id = Util.VerifyInt(DeleteID);
        Livestock? livestockToDelete = Livestocks.Where(l => l.Id == id).FirstOrDefault();
        StringBuilder notice = new StringBuilder();
        StringBuilder result = new StringBuilder();
        notice.AppendLine($"Remove livestock ID {DeleteID}");
        notice.AppendLine(livestockToDelete.ToString());
        if (livestockToDelete == null)
        {
            result.AppendLine($"ID {DeleteID} Not Existed");
            await Shell.Current.DisplayAlert("Nothing Found", $"Failed to find livestock ID {DeleteID}",
                "Continue");
        }
        else
        {
            bool toDelete =
                await Shell.Current.DisplayAlert("Confirm for Deletion", notice.ToString(),
                "Continue", "Cancel");
            if (toDelete)
            {
                deleted = _database.Delete(livestockToDelete) > 0;
                {
                    result.AppendLine($"Livestock ID {DeleteID} has been Deleted");
                    result.AppendLine(livestockToDelete.ToString());
                }
            }
        }
        if (deleted)
        {
            DeleteResult = result.ToString();
        }
        else
        {
            DeleteResult = "Nothing Removed";
        }
    }
    bool CanDelete()
    {
        return Util.VerifyInt(DeleteID) != Util.bad_int;
    }

    #endregion Delete
}
