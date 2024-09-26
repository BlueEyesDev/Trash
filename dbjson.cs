namespace Schema
{
    public class Network : IIdentifiable
    {
        public int id { get; set; }
        public string ProgramID { get; set; }
        public string IPDNS { get; set; }
        public bool Allow { get; set; }

    }
    public interface IIdentifiable
    {
        int id { get; set; }
    }
}

public class DataBase
{
    private static bool Encrypted = false;
    private static string DbFile = "test.db";

    private static Dictionary<string, object> db = new Dictionary<string, object>();
    static DataBase()
    {
        var jsonData = LoadDataBase();
        LoadOrCreateTable<Schema.Detection>(jsonData);
        LoadOrCreateTable<Schema.Quarantine>(jsonData);
        LoadOrCreateTable<Schema.Program>(jsonData);
        LoadOrCreateTable<Schema.Network>(jsonData);
        LoadOrCreateTable<Schema.Exclusion>(jsonData);
    }


    public static bool Insert<T>(T add) where T : class {
        ((List<T>)db[typeof(T).Name]).Add(add);
        WriteDataBase();
        return true;
    }
    public static bool Delete<T>(T Params) where T : class
    {
        var list = db[typeof(T).Name] as List<T>;
        if (list != null)
        {
            var itemToRemove = list.OfType<T>().FirstOrDefault(item =>
            {
                foreach (var property in typeof(T).GetProperties())
                {
                    var searchValue = property.GetValue(Params);
                    var itemValue = property.GetValue(item);
                    if (searchValue == null)
                        continue;
                    if (itemValue == null || !itemValue.Equals(searchValue))
                        return false;
                }
                return true;
            });
            if (itemToRemove != null)
            {
                list.Remove(itemToRemove as T);
                WriteDataBase();
                return true;
            }
        }
        return false;
    }
    public static T Read<T>(T searchParams) where T : class {
        var list = db[typeof(T).Name] as List<T>;
        if (list != null)
            return list.OfType<T>().FirstOrDefault(item =>
            {
                foreach (var property in typeof(T).GetProperties())
                {
                    var searchValue = property.GetValue(searchParams);
                    var itemValue = property.GetValue(item);
                    if (searchValue == null)
                        continue;
                    if (itemValue == null || !itemValue.Equals(searchValue))
                        return false;
                }
                return true;
            });
        return null;
    }
    public static List<T> ReadAll<T>() where T : class {
        var list = db[typeof(T).Name] as List<T>;
        if (list != null)
            return list;
        return null;
    }
    public static int Count<T>() where T : class {
        var list = db[typeof(T).Name] as List<T>;
        if (list != null)
            return -1;
        return list.Count;
    }
    static bool LoadOrCreateTable<T>(Dictionary<string, object> Json = null) where T : class
    {
        try
        {

            if (!db.ContainsKey(typeof(T).Name) && Json.ContainsKey(typeof(T).Name))
            {
                db[typeof(T).Name] = new JavaScriptSerializer().ConvertToType<List<T>>(Json[typeof(T).Name]);
            }
            else
            {
                db.Add(typeof(T).Name, new List<T>());
                WriteDataBase();
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    static bool WriteDataBase()
    {

        try
        {
            if (Encrypted)
            {
                Encryption.CryptFile(DbFile, UTF8Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(db)), Encryption.GenKey("Database"));
            }
            else
            {
                File.WriteAllBytes(DbFile, UTF8Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(db)));
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
    static Dictionary<string, object> LoadDataBase()
    {
        try
        {
            if (File.Exists(DbFile))
            {
                if (Encrypted)
                {
                    byte[] jsondata = Encryption.DecryptFile(DbFile, Encryption.GenKey("Database"));
                    return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(UTF8Encoding.UTF8.GetString(jsondata));
                }
                else
                {
                    return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(UTF8Encoding.UTF8.GetString(File.ReadAllBytes(DbFile)));
                }

            }
            else
            {
                return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>("{}");
            }
        }
        catch
        {
            return null;
        }
    }

}
