using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer3.Services
{
    public class ConfigurationService : IConfigurationService
    {
        const string SettingsFile = "settings.json";

        IList<AutoLoadConfigItem> _items = new List<AutoLoadConfigItem>() { 
            new AutoLoadConfigItem() { Name = "Debug.log", Path = @"C:\Users\Adnan.Asotic\Documents\Logs\Debug.log" } ,
            new AutoLoadConfigItem() { Name = "Dism.log", Path = @"C:\Windows\Logs\DISM\dism.log" }
        };

        public ConfigurationService()
        {
            if (File.Exists(SettingsPath))
            {
                _items = JsonConvert.DeserializeObject<List<AutoLoadConfigItem>>(File.ReadAllText(SettingsPath));
            }
        }

        public IList<AutoLoadConfigItem> GetAutoLoads()
        {
            return _items;
        }

        public void SaveAutoLoads(IList<AutoLoadConfigItem> payload)
        {
            File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(payload, typeof(List<AutoLoadConfigItem>), new JsonSerializerSettings()));
        }

        public string SettingsPath => SettingsFile; // Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingsFile);
    }

    public interface IConfigurationService
    {
        
        IList<AutoLoadConfigItem> GetAutoLoads();
        void SaveAutoLoads(IList<AutoLoadConfigItem> payload);
    }
}
