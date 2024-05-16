using Microsoft.Extensions.Logging.Abstractions;

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
        QueryType = Types[0];
        QueryColour = Colours[0];
        InsertType = Types[0];
        InsertColour = Colours[0];
        UpdateType = Types[0];
        UpdateColour = Colours[0];
        QueryResult = defaultResult;
        InsertResult = defaultResult;
        DeleteResult = defaultResult;
        UpdateResult = defaultResult;
        IDCheckVisible = true;
        UpdateVisible = false;
    }

    [ObservableProperty] ObservableCollection<Livestock> _livestocks;
    public ObservableCollection<Cow> Cows
    { get => new ObservableCollection<Cow>(Livestocks.OfType<Cow>()); }
    public ObservableCollection<Sheep> Sheeps
    { get => new ObservableCollection<Sheep>(Livestocks.OfType<Sheep>()); }

    [ObservableProperty]// Statistics reporting
    string _pricesInfo, _statisticsReport;
    [ObservableProperty]// Pickers' items
    string[] _types = { "Cow", "Sheep" },
        _colours = { "All", "White", "Black", "Red" };
    [RelayCommand]
    void Reset(string section)
    {
        switch (section)
        {
            case "Query":
                QueryType = Types[0];
                QueryColour = Colours[0];
                QueryResult = defaultResult; break;
            case "Insert":
                InsertType = Types[0];
                InsertCost = "";
                InsertWeight = "";
                InsertColour = Colours[0];
                InsertProduceWeight = "";
                InsertResult = defaultResult; break;
            case "Delete":
                DeleteID = "";
                DeleteResult = defaultResult; break;
            case "IDCheck":
                UpdateID = ""; break;
            case "Update":
                UpdateType = Types[0];
                UpdateCost = "";
                UpdateWeight = "";
                UpdateColour = Colours[0];
                UpdateProduceWeight = "";
                UpdateVisible = false;
                IDCheckVisible = true;
                UpdateResult = defaultResult; break;
        }
    }

    #region query
    // Properties
    [ObservableProperty]
    string _queryResult, // queried items data show here
        _queryTypeVerify, _queryColourVerify;// show sender's input is valid or not
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(QueryCommand))]
    string _queryType, _queryColour;// binding with pickers item

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        Debug.WriteLine($"{propertyName} changed to: {QueryType}");
    }

    // Commands
    [RelayCommand(CanExecute = nameof(CanQuery))]
    public void Query()
    {
        QueryResult = _util.GetQueryDetail(QueryType, QueryColour);
    }
    bool CanQuery() => !string.IsNullOrEmpty(QueryType)
            && !string.IsNullOrEmpty(QueryColour);
    #endregion query

    #region Insert
    string InsertProduceWeightName
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
        string? produce = string.IsNullOrEmpty(InsertProduceWeight) ?
            $"*Empty {InsertProduceWeightName}" : Util.VerifyFloat(InsertProduceWeight) == Util.bad_float ?
            $"*Invalid {InsertProduceWeightName}" : "";
        InsertVerifyInfo = string.Format("{0}{1}{2}{3}{4}",
            type, cost, weight, colour, produce);
    }
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(InsertCommand))]
    string _insertVerifyInfo, _insertType, _insertCost,
     _insertWeight, _insertColour, _insertProduceWeight, _insertResult;

    [RelayCommand(CanExecute = nameof(CanInsert))]
    async void Insert()
    {// Confirm if insert
        bool inserted = false;// indicate livestock is added
        StringBuilder info = new StringBuilder();
        info.AppendLine($"New {InsertType} to be Added");
        if (InsertType == "Cow")
        {
            Cow newCow = new Cow(InsertCost, InsertWeight, InsertColour, InsertProduceWeight);
            info.AppendLine(newCow.ToString());
        }
        else if (InsertType == "Sheep")
        {
            Sheep newSheep = new Sheep(InsertCost, InsertWeight, InsertColour, InsertProduceWeight);
            info.AppendLine(newSheep.ToString());
        }
        StringBuilder result = new StringBuilder();
        bool toInsert = await Shell.Current.DisplayAlert("info", info.ToString(), "Continue", "Cancel");
        // to insert
        if (toInsert)
        {
            if (InsertType == "Cow")
            {
                Cow newCow = new Cow(InsertCost, InsertWeight, InsertColour, InsertProduceWeight);
                if (inserted = _database.Insert(newCow) > 0)
                {
                    Livestocks.Add(newCow);
                    InsertResult = result.ToString();
                }
                result.AppendLine("New Cow Added");
                result.AppendLine(newCow.ToString());
            }
            else if (InsertType == "Sheep")
            {
                Sheep newSheep = new Sheep(InsertCost, InsertWeight, InsertColour, InsertProduceWeight);
                if (inserted = _database.Insert(newSheep) > 0)
                {
                    Livestocks.Add(newSheep);
                    InsertResult = result.ToString();
                }
                result.AppendLine("New Sheep Added");
                result.AppendLine(newSheep.ToString());
            }
            if (!inserted)
            {
                InsertResult = "Nothing Added";
            }
        }
    }
    bool CanInsert()
    {
        return !string.IsNullOrEmpty(InsertType)
                && Util.VerifyFloat(InsertCost) != Util.bad_float
                && Util.VerifyFloat(InsertWeight) != Util.bad_float
                && !string.IsNullOrEmpty(InsertColour)
                && Util.VerifyFloat(InsertProduceWeight) != Util.bad_float;
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
        StringBuilder info = new StringBuilder();
        StringBuilder result = new StringBuilder();
        info.AppendLine($"Remove livestock ID {DeleteID}");
        if (livestockToDelete != null)
            info.AppendLine(livestockToDelete.ToString());
        if (livestockToDelete == null)
        {
            result.AppendLine($"ID {DeleteID} Not Existed");
            await Shell.Current.DisplayAlert("Nothing Found", $"Failed to find livestock ID {DeleteID}",
                "Continue");
        }
        else
        {
            bool toDelete = await Shell.Current.DisplayAlert("Confirm for Deletion", info.ToString(), "Continue", "Cancel");
            if (toDelete)
            {
                if (deleted = _database.Delete(livestockToDelete) > 0)
                {
                    Livestocks.Remove(livestockToDelete);
                    result.AppendLine($"Livestock ID {DeleteID} has been Deleted");
                    result.AppendLine(livestockToDelete.ToString());
                    DeleteResult = result.ToString();
                }
                else
                {
                    DeleteResult = "Nothing Removed";
                }
            }
        }
    }
    bool CanDelete()
    {
        return Util.VerifyInt(DeleteID) != Util.bad_int;
    }

    #endregion Delete
    #region Update
    string updateProduceName
    {
        get
        {
            switch (UpdateType)
            {
                case "Cow": return "Milk";
                case "Sheep": return "Wool";
                default: return "Produce";
            }
        }
    }
    string idVerify
    {
        get => string.IsNullOrEmpty(UpdateID) ?
            "Empty ID" : Util.VerifyInt(UpdateID) == Util.bad_int ?
            $"Invalid ID {UpdateID}" : "";
    }
    string updateVerify
    {
        get
        {
            string[] props = new string[5];
            props[0] = string.IsNullOrEmpty(UpdateType) ?
            "Empty Type" : "";
            props[1] = string.IsNullOrEmpty(UpdateCost) ?
                "Empty Cost" : Util.VerifyFloat(UpdateCost) == Util.bad_float ?
                $"Invalid Cost {UpdateCost}" : "";
            props[2] = string.IsNullOrEmpty(UpdateWeight) ?
                "Empty Weight" : Util.VerifyFloat(UpdateWeight) == Util.bad_float ?
                $"Invalid Weight {UpdateWeight}" : "";
            props[3] = string.IsNullOrEmpty(UpdateColour) ?
                "Empty Colour" : "";
            props[4] = string.IsNullOrEmpty(UpdateProduceWeight) ?
                $"Empty {updateProduceName}" : Util.VerifyFloat(UpdateProduceWeight) == Util.bad_float ?
                $"Invalid {updateProduceName} {UpdateProduceWeight}" : "";
            StringBuilder sb = new StringBuilder();
            foreach (var item in props)
            {
                if (item != string.Empty)
                {
                    sb.AppendLine(item);
                }
            }
            return sb.ToString();
        }
    }
    [ObservableProperty]
    string _updateResult;
    [ObservableProperty]
    bool _updateVisible, _iDCheckVisible;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(IDCheckCommand))]
    string _updateID;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UpdateCommand))]
    string _updateType, _updateCost, _updateWeight, _updateColour, _updateProduceWeight;

    [RelayCommand(CanExecute = nameof(CanIDCheck))]
    public void IDCheck()
    {
        int id = Util.VerifyInt(UpdateID);
        if (Livestocks.Where(o => o.Id == id).FirstOrDefault() == null)
        {
            string msg = $"Failed to Find the livestock with ID {UpdateID}";
            Shell.Current.DisplayAlert("Fail", msg, "Continue");
        }
        else
        {
            IDCheckVisible = false;
            UpdateVisible = true;
        }
    }
    bool CanIDCheck()
    {
        return Util.VerifyInt(UpdateID) != Util.bad_int;
    }

    [RelayCommand(CanExecute = nameof(CanUpdate))]
    public void Update()
    {
        bool updated = false;
        if (UpdateType == "Cow")
        {
            Cow newCow = new Cow(UpdateID, UpdateCost, UpdateWeight, UpdateColour, UpdateProduceWeight);
            updated = _database.Update(newCow) > 0;
            UpdateResult = newCow.ToString();
        }
        else if (UpdateType == "Sheep")
        {
            Sheep newSheep = new Sheep(UpdateID, UpdateCost, UpdateWeight, UpdateColour, UpdateProduceWeight);
            updated = _database.Update(newSheep) > 0;
            UpdateResult = newSheep.ToString();
        }
        if (!updated)
        {
            string msg = $"ID {UpdateID} Failed to Update";
            Shell.Current.DisplayAlert("Failed", msg, "Continue");
        }
        else
        {
            string msg = $"ID {UpdateID} Updated\n{UpdateResult}";
            Shell.Current.DisplayAlert("Updated", msg, "Continue");
        }
    }
    bool CanUpdate()
    {
        bool condition = !string.IsNullOrEmpty(UpdateType)
            && !string.IsNullOrEmpty(UpdateColour)
            && Util.VerifyFloat(UpdateCost) != Util.bad_float
            && Util.VerifyFloat(UpdateWeight) != Util.bad_float
            && Util.VerifyFloat(UpdateProduceWeight) != Util.bad_float;
        return condition;
    }
    #endregion Update
}
