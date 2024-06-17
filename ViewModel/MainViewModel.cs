namespace Task4_MVVE.ViewModel;

public partial class MainViewModel : ObservableObject
{
    #region Properties
    Util _util;
    Database _database;
    readonly string defaultResult = "Result showing here";
    [ObservableProperty] ObservableCollection<Livestock> _livestocks;
    public ObservableCollection<Cow> Cows 
    { 
        get => new ObservableCollection<Cow>(Livestocks.OfType<Cow>()); 
    }
    public ObservableCollection<Sheep> Sheeps 
    { 
        get => new ObservableCollection<Sheep>(Livestocks.OfType<Sheep>());
    }

    [ObservableProperty]// Statistics reporting
    string _pricesInfo, _statisticsReport;

    [ObservableProperty]// Pickers' items
    string[] _types = { "Cow", "Sheep" },
        _colours = { "All", "White", "Black", "Red" }; 
    #endregion
    public MainViewModel()
    {
        Livestocks = new ObservableCollection<Livestock>();
        _database = new Database();
        _util = new Util(this);
        _database.ToList().ForEach(l => Livestocks?.Add(l));
        PricesInfo = _util.GetPricesInfo();
        StatisticsReport = _util.GetStatisticsReport();
        Reset("");
    }
    #region Reset
    [RelayCommand]
    void Reset(string section)
    {
        switch (section)
        {
            case "Query":
                QueryType = Types[0];
                QueryColour = Colours[0];
                QueryResult = defaultResult;
                break;
            case "Insert":
                InsertType = Types[0];
                InsertCost = "";
                InsertWeight = "";
                InsertColour = Colours[0];
                InsertProduceWeight = "";
                InsertResult = defaultResult;
                break;
            case "Delete":
                DeleteID = "";
                DeleteResult = defaultResult;
                break;
            case "Update":
                UpdateCost = "";
                UpdateWeight = "";
                UpdateColour = Colours[0];
                UpdateProduceWeight = "";
                UpdateVisible = false;
                IDCheckVisible = true;
                UpdateResult = defaultResult;
                break;
            default:
                Reset("Query");
                Reset("Insert");
                Reset("Delete");
                Reset("Update");
                break;
        }
    }
    #endregion
    #region Query
    // Properties
    [ObservableProperty]
    string? _queryResult;

    // binding with pickers item
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(QueryCommand))]
    string? _queryType,
        _queryColour;

    [RelayCommand]
    public async void Query()
    {
        if (CanQuery())
        {
            QueryResult = _util.GetQueryDetail(QueryType, QueryColour);
        }
    }
    bool CanQuery() => !string.IsNullOrEmpty(QueryType)
            && !string.IsNullOrEmpty(QueryColour);
    #endregion
    #region Insert
    string ProduceName(string livestock)
    {
        if (livestock == "Cow") return "Milk";
        else return "Wool";
    }
    string GetInsertVerifyInfo()
    {
        string[] props = new string[5];
        props[0] = string.IsNullOrEmpty(InsertType) ?
            "Empty Type" : "";
        props[1] = string.IsNullOrEmpty(InsertCost) ?
            "Empty Cost" : Util.VerifyFloat(InsertCost) == Util.bad_float ?
            "Invalid Cost" : "";
        props[2] = string.IsNullOrEmpty(InsertWeight) ?
            "Empty Weight" : Util.VerifyFloat(InsertWeight) == Util.bad_float ?
            "Invalid Weight" : "";
        props[3] = string.IsNullOrEmpty(InsertColour) ?
            "Empty Colour" : "";
        props[4] = string.IsNullOrEmpty(InsertProduceWeight) ?
            $"Empty {ProduceName(InsertType)}" : Util.VerifyFloat(InsertProduceWeight) == Util.bad_float ?
            $"Invalid {ProduceName(InsertType)}" : "";

        StringBuilder sb = new StringBuilder();
        foreach (string prop in props)
        {
            if (prop != "")
            {
                sb.AppendLine(prop);
            }
        }
        return sb.ToString();
    }

    [ObservableProperty]
    string _insertResult;

    [ObservableProperty]
    string _insertType,
        _insertCost,
        _insertWeight,
        _insertColour,
        _insertProduceWeight;

    [RelayCommand]
    async Task Insert()
    {
        StringBuilder result = new StringBuilder();// showed when get inserted
        if (CanInsert())
        {
            // 1 ask confirmation in alert window
            bool inserted = false;// trigger when model is inserted successfully 
            StringBuilder msg = new StringBuilder();// message in alert
            msg.AppendLine($"Detail of New {InsertType}\n");
            if (InsertType == "Cow")
            {
                Cow newCow = new Cow(InsertCost, InsertWeight, InsertColour, InsertProduceWeight);
                msg.AppendLine(newCow.ToString());
            }
            else if (InsertType == "Sheep")
            {
                Sheep newSheep = new Sheep(InsertCost, InsertWeight, InsertColour, InsertProduceWeight);
                msg.AppendLine(newSheep.ToString());
            }
            msg.AppendLine("Confirmed to continue adding? ");
            // trigger when click continue in alert
            bool toInsert = await Shell.Current.DisplayAlert("Confirmation", msg.ToString(), "Continue", "Cancel");
            // 2 commit or not decided in alert window
            if (toInsert)
            {
                result.AppendLine($"Detail of New {InsertType}");
                // insert in observable collection and database
                if (InsertType == "Cow")
                {
                    Cow newCow = new Cow(InsertCost, InsertWeight, InsertColour, InsertProduceWeight);
                    if (inserted = _database.Insert(newCow) > 0)
                    {
                        Livestocks.Add(newCow);
                        result.AppendLine(newCow.ToString());
                    }
                }
                else if (InsertType == "Sheep")
                {
                    Sheep newSheep = new Sheep(InsertCost, InsertWeight, InsertColour, InsertProduceWeight);
                    if (inserted = _database.Insert(newSheep) > 0)
                    {
                        Livestocks.Add(newSheep);
                        result.AppendLine(newSheep.ToString());
                    }
                }

                // case when insertion failed
                if (!inserted)
                {
                    result.Clear();
                    result.AppendLine("Failed to get this livestock Added");
                }
                // 3 show result in label
                InsertResult = result.ToString();
            }
            else// cancelled insetion
            {
                result.AppendLine("Adding operation is Cancelled");
            }
        }
        else// CanInsert() = false
        {
            await Shell.Current.DisplayAlert("Error", GetInsertVerifyInfo(), "Cancel");
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
    string GetDeleteVerifyInfo()
    {
        string info = string.IsNullOrEmpty(DeleteID) ?
            "Empty ID" : Util.VerifyInt(DeleteID) == Util.bad_int ?
            "Invalid ID" : "";
        return info;
    }
    [ObservableProperty]
    string _deleteVerifyInfo, _deleteResult;

    [ObservableProperty]
    string _deleteID;

    [RelayCommand]
    async Task Delete()
    {
        StringBuilder result = new StringBuilder();
        if (CanDelete())
        {
            bool deleted = false;
            int id = Util.VerifyInt(DeleteID);
            Livestock? livestockToDelete = Livestocks.Where(l => l.Id == id).FirstOrDefault();
            StringBuilder msg = new StringBuilder();
            if (livestockToDelete == null)
            {
                result.AppendLine($"Livestock with ID {DeleteID} Not Existed");
                await Shell.Current.DisplayAlert("Error", $"Failed to find livestock with ID {DeleteID}", "Continue");
            }
            else
            {
                msg.AppendLine("Detail of livestock to Remove");
                msg.AppendLine(livestockToDelete.ToString());
                bool toDelete = await Shell.Current.DisplayAlert("Confirmation", msg.ToString(), "Continue", "Cancel");
                if (toDelete)
                {
                    if (deleted = _database.Delete(livestockToDelete) > 0)
                    {
                        Livestocks.Remove(livestockToDelete);
                        result.AppendLine($"Livestock ID {DeleteID} has been Deleted");
                        result.AppendLine(livestockToDelete.ToString());
                    }
                    else
                    {
                        result.AppendLine("Removing Cancelled");
                    }
                }
                DeleteResult = result.ToString();
            }
        }
        else// CanDelete() is false
        {
            await Shell.Current.DisplayAlert("Error", GetDeleteVerifyInfo(), "Cancel");
        }
    }
    bool CanDelete()
    {
        return Util.VerifyInt(DeleteID) != Util.bad_int;
    }

    #endregion Delete
    #region Update
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
            string[] props = new string[4];
            props[0] = string.IsNullOrEmpty(UpdateCost) ?
                "Empty Cost" : Util.VerifyFloat(UpdateCost) == Util.bad_float ?
                $"Invalid Cost {UpdateCost}" : "";
            props[1] = string.IsNullOrEmpty(UpdateWeight) ?
                "Empty Weight" : Util.VerifyFloat(UpdateWeight) == Util.bad_float ?
                $"Invalid Weight {UpdateWeight}" : "";
            props[2] = string.IsNullOrEmpty(UpdateColour) ?
                "Empty Colour" : "";
            props[3] = string.IsNullOrEmpty(UpdateProduceWeight) ?
                $"Empty Produce" : Util.VerifyFloat(UpdateProduceWeight) == Util.bad_float ?
                $"Invalid Produce {UpdateProduceWeight}" : "";
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
    string _updateCost, _updateWeight, _updateColour, _updateProduceWeight;

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
        // update in database
        int id = Util.VerifyInt(UpdateID);
        bool updated = false;
        Cow newCow = null!;
        Sheep newSheep = null!;

        Livestock? livestockToUpdate = Livestocks.Where(l => l.Id == id).FirstOrDefault();
        if (livestockToUpdate != null)
        {
            string updateType = livestockToUpdate.Type;
            if (updateType == "Cow")
            {
                newCow = new Cow(UpdateID, UpdateCost, UpdateWeight, UpdateColour, UpdateProduceWeight);
                updated = _database.Update(newCow) > 0;
            }
            else if (updateType == "Sheep")
            {
                newSheep = new Sheep(UpdateID, UpdateCost, UpdateWeight, UpdateColour, UpdateProduceWeight);
                updated = _database.Update(newSheep) > 0;
            }
            // update obcollection and show result
            if (updated)
            {
                StringBuilder msg = new StringBuilder();
                msg.AppendLine($"{updateType} ID {UpdateID} Updated");
                msg.AppendLine($"{updateType} Details:");
                string detail = "";
                if (updateType == "Cow")
                {
                    int idx = Livestocks.IndexOf(livestockToUpdate);
                    Livestocks[idx] = newCow;
                    detail = newCow.ToString();
                }
                else if (updateType == "Sheep")
                {
                    int idx = Livestocks.IndexOf(livestockToUpdate);
                    Livestocks[idx] = newSheep;
                    detail = newSheep.ToString();
                }
                msg.AppendLine(detail);
                Shell.Current.DisplayAlert("Updated", msg.ToString(), "Continue");
                UpdateResult = $"{updateType} Details:\n" + detail;
            }
            else
            {
                string msg = $"ID {UpdateID} Failed to Update";
                Shell.Current.DisplayAlert("Failed", msg, "Continue");
            }
        }
    }
    bool CanUpdate()
    {
        bool condition = !string.IsNullOrEmpty(UpdateColour)
            && Util.VerifyFloat(UpdateCost) != Util.bad_float
            && Util.VerifyFloat(UpdateWeight) != Util.bad_float
            && Util.VerifyFloat(UpdateProduceWeight) != Util.bad_float;
        return condition;
    }
    #endregion Update
}
