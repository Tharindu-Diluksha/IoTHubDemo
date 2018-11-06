using Microsoft.Azure.EventHubs;
using settings_reader;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace read_d2c_messages
{
    class ReadDeviceToCloudMessages
    {
        // Event Hub-compatible endpoint
        // az iot hub show --query properties.eventHubEndpoints.events.endpoint --name {your IoT Hub name}
        private static string eventHubsCompatibleEndpoint;

        // Event Hub-compatible name
        // az iot hub show --query properties.eventHubEndpoints.events.path --name {your IoT Hub name}
        private static string eventHubsCompatiblePath;

        // az iot hub policy show --name iothubowner --query primaryKey --hub-name {your IoT Hub name}
        private static string iotHubSasKey ;
        private static string iotHubSasKeyName;
        private static EventHubClient eventHubClient;

        private static async Task Main(string[] args)
        {
            Console.WriteLine("Read device to cloud messages.\n");

            //Get settings
            SettingsReader settingsReader = new SettingsReader();
            var settings = settingsReader.GetSettings();
            eventHubsCompatibleEndpoint = settings.GetValue(Settings.EventHubsCompatibleEndpoint).ToString();
            eventHubsCompatiblePath = settings.GetValue(Settings.EventHubsCompatiblePath).ToString();
            iotHubSasKey = settings.GetValue(Settings.IotHubServiceSasKey).ToString();
            iotHubSasKeyName = settings.GetValue(Settings.IotHubServiceSasKeyName).ToString();
            
            // Create an EventHubClient instance to connect to the
            // IoT Hub Event Hubs-compatible endpoint.
            var connectionString = new EventHubsConnectionStringBuilder(new Uri(eventHubsCompatibleEndpoint), eventHubsCompatiblePath, iotHubSasKeyName, iotHubSasKey);
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionString.ToString());

            // Create a PartitionReciever for each partition on the hub.
            var runtimeInfo = await eventHubClient.GetRuntimeInformationAsync();
            var d2cPartitions = runtimeInfo.PartitionIds;

            CancellationTokenSource cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            var tasks = new List<Task>();
            foreach (string partition in d2cPartitions)
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
            }

            // Wait for all the PartitionReceivers to finsih.
            Task.WaitAll(tasks.ToArray());
        }

        // Asynchronously create a PartitionReceiver for a partition and then start 
        // reading any messages sent from the simulated client.
        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            // Create the receiver 
            var eventHubReceiver = eventHubClient.CreateReceiver("$Default", partition, EventPosition.FromEnqueuedTime(DateTime.Now));
            Console.WriteLine("Create receiver on partition: " + partition);
            while (true)
            {
                if (ct.IsCancellationRequested) break;
                Console.WriteLine("Listening for messages on: " + partition);

                var events = await eventHubReceiver.ReceiveAsync(100);

                // If there is data process it.
                if (events == null) continue;

                foreach (EventData eventData in events)
                {
                    string data = Encoding.UTF8.GetString(eventData.Body.Array);

                    Console.WriteLine("Message received on partition {0}:", partition);
                    Console.WriteLine("  {0}:", data);

                    Console.WriteLine("Application properties (set by device):");
                    foreach (var prop in eventData.Properties)
                    {
                        Console.WriteLine("  {0}: {1}", prop.Key, prop.Value);
                    }

                    Console.WriteLine("System properties (set by IoT Hub):");
                    foreach (var prop in eventData.SystemProperties)
                    {
                        Console.WriteLine("  {0}: {1}", prop.Key, prop.Value);
                    }

                    Console.WriteLine("\n");
                    Console.WriteLine("\n");
                }
            }
        }
    }
}
