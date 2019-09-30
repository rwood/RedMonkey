using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace RedMonkey.Config
{
    public class DbConnectionInfo
    {
        public string ConnectionString { get; set; } = "Data Source=;Initial Catalog=;Integrated Security=true;";
        public string ProviderName { get; set; } = "System.Data.SqlClient";
    }

    public class DbCodeGen
    {
        public string NameSpace { get; set; }
        public string TargetDirectory { get; set; }
        public List<string> IgnoreTables { get; set; } = new List<string>();
    }

    public class Settings
    {
        public DbConnectionInfo MasterDb { get; } = new DbConnectionInfo();
        public DbCodeGen DbCodeGen { get; } = new DbCodeGen();
    }

    public class SettingsService
    {
        public Settings GetSettings(string filePath = null)
        {
            Settings settings = null;
            if (filePath != null && File.Exists(filePath))
            {
                settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(filePath));
                if (!Path.IsPathRooted(settings.DbCodeGen.TargetDirectory))
                    settings.DbCodeGen.TargetDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(filePath), settings.DbCodeGen.TargetDirectory));
                return settings;
            }

            var current = new DirectoryInfo(Environment.CurrentDirectory);
            while (current != null)
            {
                var files = current.GetFiles(".redMonkey");
                var file = files.FirstOrDefault();
                if (file != null)
                {
                    settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(file.FullName));
                    if (!Path.IsPathRooted(settings.DbCodeGen.TargetDirectory) && file.Directory != null)
                        settings.DbCodeGen.TargetDirectory = Path.GetFullPath(Path.Combine(file.Directory.FullName, settings.DbCodeGen.TargetDirectory));
                    return settings;
                }

                current = current.Parent;
            }

            settings = new Settings();
            File.WriteAllText(".redMonkey", JsonConvert.SerializeObject(settings));
            return settings;
        }
    }
}