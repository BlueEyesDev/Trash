using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
namespace Test
{
    public class DataBase
    {
        private static bool Encrypted = false;
        private static string DbFile = "test.db";
        public interface IIdentifiable
        {
            int id { get; set; }
        }
        public class Detection : IIdentifiable
        {
            public int id { get; set; }
            public string ProgramID { get; set; }
            public string DetectionID { get; set; }
            public string Type { get; set; }
            public string[] Information { get; set; }
            public string Date { get; set; }

        }
        public class Quarantine : IIdentifiable
        {
            public int id { get; set; }
            public string QuarantineID { get; set; }
            public string ProgramID { get; set; }
            public string Path { get; set; }
            public string Date { get; set; }
        }
        public class Program : IIdentifiable
        {
            public int id { get; set; }
            public string ProgramID { get; set; }
            public string Name { get; set; }
            public string PathFile { get; set; }
            public string SHA512 { get; set; }
            public bool Quaretine { get; set; }
            public string Date { get; set; }
        }
        public class Network : IIdentifiable
        {
            public int id { get; set; }
            public string ProgramID { get; set; }
            public string IPDNS { get; set; }
            public bool Allow { get; set; }

        }
        public class Historique
        {
            public Detection Detection { get; set; }
            public Program Program { get; set; }
        }
        public class Exclusion : IIdentifiable
        {
            public int id { get; set; }
            public string ProgramID { get; set; }
            public string PathFile { get; set; }
            public string Directory { get; set; }
        }

        private static Dictionary<string, object> db = new Dictionary<string, object>();
        static DataBase()
        {
            var jsonData = LoadDataBase();
            LoadOrCreateTable<Detection>(jsonData);
            LoadOrCreateTable<Quarantine>(jsonData);
            LoadOrCreateTable<Program>(jsonData);
            LoadOrCreateTable<Network>(jsonData);
            LoadOrCreateTable<Exclusion>(jsonData);
        }
        public static bool Insert<T>(T add) where T : class {
            ((List<T>)db[typeof(T).Name]).Add(add);
            WriteTable();
            return true;
        }
        public static bool Delete<T>(T Params) where T : class, IIdentifiable
        {

            var list = db[typeof(T).Name] as List<T>;
            if (list != null)
            {
                var itemToRemove = list.OfType<T>().FirstOrDefault(item =>
                {
                    foreach (var property in t.GetProperties())
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
                    WriteTable();
                    return true;
                }
            }
            return false;
        }
        public static T Read<T>(T Params) where T : class, IIdentifiable
        {
            var list = db[typeof(T).Name] as List<T>;
            if (list != null)
                return list.OfType<T>().FirstOrDefault(item =>
                {
                    foreach (var property in t.GetProperties())
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
            return null;
        }
        public static List<T> ReadAll<T>() where T : class
        {
            var list = db[typeof(T).Name] as List<T>;
            if (list != null)
                return list;
            return null;
        }
        public static int Count<T>() where T : class
        {
            var list = db[typeof(T).Name] as List<T>;
            if (list != null)
                return -1;
            return list.Count;
        }
        public static bool LoadOrCreateTable<T>(Dictionary<string, object> Json = null) where T : class {
            try {
                if (Json.Count > 0 && db.ContainsKey(typeof(T).Name) && Json.ContainsKey(typeof(T).Name)) {
                    db[typeof(T).Name] = new JavaScriptSerializer().ConvertToType<List<T>>(Json[typeof(T).Name]);
                } else { 
                    db.Add(typeof(T).Name, new List<T>());
                    WriteTable();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        static bool WriteTable() {

            try {
                if (Encrypted)
                {
                    File.WriteAllBytes(DbFile,
                        Encryption.CryptFile(UTF8Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(db)), Encryption.GenKey("Database"), Encryption.GetKeyAndIV().iv));
                }
                else {
                    File.WriteAllBytes(DbFile, UTF8Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(db)));
                }
                return true;
            } catch {
                return false;
            }
        }
        static Dictionary<string, object> LoadDataBase() {
            try {
                if (File.Exists(DbFile)) {
                    if (Encrypted)
                    {
                        byte[] jsondata = Encryption.DecryptFile(File.ReadAllBytes(DbFile), Encryption.GenKey("Database"), Encryption.GetKeyAndIV().iv);
                        return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(UTF8Encoding.UTF8.GetString(jsondata));
                    } else {
                        return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(UTF8Encoding.UTF8.GetString(File.ReadAllBytes(DbFile)));
                    }

                } else {
                    return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>("{}");
                }
            } catch {
                return null;
            }
        }
    }
}
