using Microsoft.Azure.Devices;
using settings_reader;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace send_c2d_messages
{
    class SendCloudToDeviceMessages
    {
        private static ServiceClient serviceClient;
        private static string connectionString = "{iot hub connection string}";

        private static async Task Main(string[] args)
        {
            Console.WriteLine("Send Cloud-to-Device message\n");

            //Get settings
            SettingsReader settingsReader = new SettingsReader();
            var settings = settingsReader.GetSettings();
            connectionString = settings.GetValue(Settings.IotHubServiceConnectionString).ToString();

            
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

            Console.WriteLine("Press any key to send a C2D message.");
            Console.ReadLine();
            await SendCloudToDeviceMessageAsync();
            await ReceiveFeedbackAsync();
            Console.ReadLine();
        }

        private async static Task SendCloudToDeviceMessageAsync()
        {
            var guid = Guid.NewGuid().ToString();
            var message = $"Cloud to device message. ID: {guid}";

            var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
            commandMessage.Ack = DeliveryAcknowledgement.Full;

            Console.WriteLine($"sending message:- {message}");

            await serviceClient.SendAsync("MyDotnetDevice", commandMessage);
        }

        private async static Task ReceiveFeedbackAsync()
        {
            var feedbackReceiver = serviceClient.GetFeedbackReceiver();

            Console.WriteLine("\nReceiving c2d feedback from service");
            while (true)
            {
                var feedbackBatch = await feedbackReceiver.ReceiveAsync();
                if (feedbackBatch == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received feedback: {0}",
                  string.Join(", ", feedbackBatch.Records.Select(f => f.StatusCode)));
                Console.ResetColor();

                await feedbackReceiver.CompleteAsync(feedbackBatch);
            }
        }

    }
}
