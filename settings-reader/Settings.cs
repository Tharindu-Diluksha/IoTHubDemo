using System;
using System.Collections.Generic;
using System.Text;

namespace settings_reader
{
    public class Settings
    {
        public readonly static string DeviceConnectionString = "DeviceConnectionString";
        public readonly static string IotHubSasKey = "IotHubSasKey";
        public readonly static string IotHubSasKeyName = "IotHubSasKeyName";
        public readonly static string IotHubServiceSasKey = "IotHubServiceSasKey";
        public readonly static string IotHubServiceSasKeyName = "IotHubServiceSasKeyName";
        public readonly static string IotHubServiceConnectionString = "IotHubServiceConnectionString";
        public readonly static string EventHubsCompatibleEndpoint = "EventHubsCompatibleEndpoint";
        public readonly static string EventHubsCompatiblePath = "EventHubsCompatiblePath";
    }
}
