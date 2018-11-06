using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using settings_reader;

namespace read_c2d_messages_device
{
    class C2DSimulatedDevice
    {
        private static DeviceClient deviceClient;

        // The device connection string to authenticate the device with your IoT hub.
        // Using the Azure CLI:
        // az iot hub device-identity show-connection-string --hub-name {YourIoTHubName} --device-id MyDotnetDevice --output table
        private static string connectionString;

        private static void Main(string[] args)
        {
            Console.WriteLine("Cloud to Device Messages - Simulated device.\n");
            SettingsReader settingsReader = new SettingsReader();

            connectionString = settingsReader.GetSettings().GetValue(Settings.DeviceConnectionString).ToString();

            //Connect to the IoT hub using the MQTT protocol
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Amqp);
            ReceiveC2dAsync();
            Console.ReadLine();
        }


        private static async void ReceiveC2dAsync()
        {
            Console.WriteLine("Receiving cloud to device messages from service.\n");
            while (true)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received message: {0}",
                Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                Console.ResetColor();

                await deviceClient.CompleteAsync(receivedMessage);
                //await deviceClient.RejectAsync(receivedMessage);
                //await deviceClient.AbandonAsync(receivedMessage);
            }
        }
    }
}
