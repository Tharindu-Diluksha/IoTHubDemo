using System;

namespace settings_reader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            SettingsReader settingsReader = new SettingsReader();

            var settings = settingsReader.GetSettings();
            Console.WriteLine(settings.GetValue("DeviceConnectionString"));
        }
    }
}
