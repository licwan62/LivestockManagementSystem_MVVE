namespace Task4_MVVE.ViewModel;

public class Util
{
    public LivestockViewModel vm;
    private float milk_price_rate = 9.4f,
        wool_price_rate = 6.2f,
        tax_rate = 0.2f;
    public Util(LivestockViewModel vm)
    {
        this.vm = vm;
    }
    #region Verify
    public static readonly int bad_int = int.MaxValue;// empty or not numeric
    public static readonly float bad_float = float.MaxValue;// empty or not numeric
    public static readonly string bad_string = string.Empty;// empty or not included
    public static int VerifyInt(string input)
    {
        if (!int.TryParse(input, out int result)) return bad_int;
        else return result;
    }
    public static float VerifyFloat(string input)
    {
        if (!float.TryParse(input, out float result)) return bad_float;
        else return result;
    }
    public static string VerifyType(string input)
    {
        string[] types = new string[2] { "Cow", "Sheep" };
        if (string.IsNullOrEmpty(input)) return bad_string;
        else
        {
            input = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
            if (!types.Contains(input)) return bad_string;
            else return input;
        }
    }
    public static string VerifyColour(string input)
    {
        string[] colours = new string[4] { "All", "Black", "Red", "White" };
        if (string.IsNullOrEmpty(input)) return bad_string;
        else
        {
            input = CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(input);
            if (!colours.Contains(input)) return bad_string;
            else return input;
        }
    }
    public static string Format(float input)
    {
        return string.Format("{0:N2}", input);
    }
    #endregion
    #region Prices Info
    public string GetPricesInfo()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Cow milk selling price :  ${milk_price_rate} per kg");
        sb.AppendLine($"Sheep wool selling price :  ${wool_price_rate} per kg");
        sb.AppendLine($"Government tax :  ${tax_rate} per kg, per day");
        return sb.ToString();
    }
    #endregion
    #region Statistics Report
    /* 1.Show how much government tax is paid by the farm per month (30 days).
     * 2.Show the total profit or loss (depending on the data) of all animals per day.
     * 3.Show the average weight of all farm animals.
     * 4.Calculate current daily profit of all cows.
     * 5.Calculate current daily profit of all sheep.
     * 6.Calculate an estimated daily profit by number and type of livestock (cow or sheep) */
    public string GetStatisticsReport()
    {
        StringBuilder sb = new StringBuilder();
        if (vm.Livestocks == null)
        {
            sb.AppendLine("Null Livestocks");
        }
        else
        {
            sb.Append(GetTax(30));
            sb.Append(GetDailyProfit());
            sb.Append(GetAverageWeightAll());
            sb.Append(GetCurrentProfit());
        }
        return sb.ToString();
    }
    private string GetTax(int day)
    {
        float tax = vm.Livestocks.Sum(a => a.Weight * tax_rate) * day;
        return $"Monthly Tax : ${Format(tax)}\n";
    }
    private string GetDailyProfit()
    {
        float allProfit = 0;
        allProfit += vm.Cows.Sum(c => c.Milk * milk_price_rate - c.Cost - c.Weight * tax_rate);
        allProfit += vm.Sheeps.Sum(s => s.Wool * wool_price_rate - s.Cost - s.Weight * tax_rate);
        return $"Farm daily profit : ${Format(allProfit)}\n";
    }
    private string GetAverageWeightAll()
    {
        float weight = vm.Livestocks.Average(x => x.Weight);
        return $"Average weight of all Livestocks : {Format(weight)} kg\n";
    }

    private float cowAveProfit, sheepAveProfit;
    private string GetCurrentProfit()
    {
        cowAveProfit = vm.Cows.Average(c => c.Milk * milk_price_rate - c.Cost - c.Weight * tax_rate);
        sheepAveProfit = vm.Sheeps.Average(s => s.Wool * wool_price_rate - s.Cost - s.Weight * tax_rate);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Based on current livestock data:");
        sb.AppendLine($"- On average, a single cow makes daily profit: ${Format(cowAveProfit)}");
        sb.AppendLine($"- On average, a single sheep makes daily profit: ${Format(sheepAveProfit)}");
        sb.AppendLine($"Current daily profit of all Sheep : ${Format(sheepAveProfit * vm.Sheeps.Count)}");
        sb.AppendLine($"Current daily profit of all Cows : ${Format(cowAveProfit * vm.Cows.Count)}");
        return sb.ToString();
    }
    public string GetEstimation(string type, int num)
    {
        float estimate_profit;
        if (type == "Cow") estimate_profit = cowAveProfit * num;
        else if (type == "Sheep") estimate_profit = sheepAveProfit * num;
        else
        {
            return string.Empty;
        }
        return $"Buying {num} {type.ToLower()} " +
            $"would bring in estimated daily profit: ${Format(estimate_profit)}\n";
    }
    public string GetQueryDetail(string type, string colour)
    {
        int num;
        float percent, tax, profit, ave_weight, produce;
        if (type == "Cow")
        {// specify list as type and colour 
            List<Cow> Cows_query;
            if (colour == "All")
                Cows_query = vm.Cows.ToList();
            else
                Cows_query = vm.Cows.Where(a => a.Colour == colour).ToList();
            if (Cows_query == null)
                return $"Failed to query the {type} which colour is {colour}";
            else
            {
                num = Cows_query.Count;
                percent = (float)num / vm.Livestocks.Count * 100;
                tax = Cows_query.Sum(a => a.Weight * tax_rate);
                profit = Cows_query.Sum(a => a.Milk * milk_price_rate - a.Cost - a.Weight * tax_rate);
                ave_weight = Cows_query.Average(a => a.Weight);
                produce = Cows_query.Sum(a => a.Milk);
            }
        }
        else if (type == "Sheep")
        {
            List<Sheep> Sheeps_query;
            if (colour == "All")
                Sheeps_query = vm.Sheeps.ToList();
            else
                Sheeps_query = vm.Sheeps.Where(a => a.Colour == colour).ToList();
            if (Sheeps_query == null)
                return $"Failed to query the {type} which colour is {colour}";
            else
            {
                num = Sheeps_query.Count;
                percent = num / vm.Livestocks.Count;
                tax = Sheeps_query.Sum(a => a.Weight * tax_rate);
                profit = Sheeps_query.Sum(a => a.Wool * wool_price_rate - a.Cost - a.Weight * tax_rate);
                ave_weight = Sheeps_query.Average(a => a.Weight);
                produce = Sheeps_query.Sum(a => a.Wool);
            }
        }
        else return "";
        // return string
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Number of livestiocks ({type} in {colour}): {num.ToString()}");
        sb.AppendLine($"Percentage of selected Livestocks: {string.Format("{0:N1}", percent)}%");
        sb.AppendLine($"Daily tax of selected Livestocks: ${string.Format("{0:N1}", tax)}");
        sb.AppendLine($"Profit: ${string.Format("{0:N2}", profit)}");
        sb.AppendLine($"Average weight: {string.Format("{0:N1}", ave_weight)}kg");
        sb.AppendLine($"Produce amount: {string.Format("{0:N1}", produce)}kg");
        return sb.ToString();
    }
    #endregion
}
