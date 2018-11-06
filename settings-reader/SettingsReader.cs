using Newtonsoft.Json.Linq;
using System.IO;

namespace settings_reader
{
    public class SettingsReader
    {
        private const string filepath = @"E:\Geveo\IoTHub\IoTHubDemo\Settings\Settings.json";
        public JObject GetSettings()
        {
            JObject jObject = JObject.Parse(File.ReadAllText(filepath));
            
            return jObject;
        }
    }
}
