using System.Security.Cryptography.X509Certificates;

namespace Task4_MVVE.Model;

public class Database
{
    public SQLiteConnection connection;
    string dbName = "FarmData.db";
    string originalDbName = "FarmDataOriginal.db";
    string dbPath;
    public Database()
    {
        dbPath = Path.Combine(Current.AppDataDirectory, dbName);
        if (!File.Exists(dbPath))
        {// copy database in dbPath from database in Resources file
            using Stream stream = OpenAppPackageFileAsync(originalDbName).Result;
            using MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            File.WriteAllBytesAsync(dbPath, memoryStream.ToArray());
        }
        connection = new SQLiteConnection(dbPath);
        Debug.WriteLine(dbPath);
    }

    /// <summary>
    /// Convert SQLiteConnection to List of Livestock
    /// </summary>
    /// <returns>List of Livestock</returns>
    public List<Livestock> ToList()
    {
        connection.CreateTables<Cow, Sheep>();
        List<Cow> cows = connection.Table<Cow>().ToList();
        List<Sheep> sheeps = connection.Table<Sheep>().ToList();
        List<Livestock> livestocks = new List<Livestock>();
        livestocks.AddRange(cows);
        livestocks.AddRange(sheeps);
        return livestocks;
    }
    public int Insert(Livestock item)
    {
        return connection.Insert(item);
    }
    public int Update(Livestock item)
    {
        return connection.Update(item);
    }
    public int Delete(Livestock item)
    {
        return connection.Delete(item);
    }
}
