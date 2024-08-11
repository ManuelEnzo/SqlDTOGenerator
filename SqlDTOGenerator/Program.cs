using SqlDTOGenerator.ClassBase;
using SqlDTOGenerator.Database;
using System.Diagnostics;
using System.Text;

DatabaseDTOGenerator db = new();
JsonStructure jsonSettings = db.GetFileSettings();

DatabaseIntegration dbI = new(jsonSettings);
var r = new Stopwatch();
try
{
    r.Start();
    await dbI.GetAllDatabaseInformation();
}
catch (Exception e)
{
    Console.WriteLine("Exception : {0} ", e);
}
finally
{
    r.Stop();
}
Console.WriteLine("Procedure is Ended in {0} millisecond... ", r.ElapsedMilliseconds.ToString());



public class DatabaseDTOGenerator
{
    public JsonStructure GetFileSettings()
    {
        var direc = new StringBuilder(AppDomain.CurrentDomain.BaseDirectory).Append("appsettings.json").ToString();
        using StreamReader reader = new(direc);
        string json = reader.ReadToEnd();
        return Newtonsoft.Json.JsonConvert.DeserializeObject<JsonStructure>(json);
    }

}