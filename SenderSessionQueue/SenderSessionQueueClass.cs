using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace SenderSessionQueue
{
    class SenderSessionQueueClass
    {
        //Edit your connection string
        static string connectionString = "Endpoint=sb://mtechnosb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=VAuvKn105u5pLWhX/T1+Pv+HST3fIXOj0fqA6X4QcAU=";
        //Edit your queue name
        static string queueName = "mtechno-session-queue";

        static async Task Main(string[] args)
        {
            ServiceBusClient client = new ServiceBusClient(connectionString); ;
            ServiceBusSender sender = client.CreateSender(queueName);
           // using ServiceBusMessageBatch  await sender.CreateMessageBatchAsync();
            while (true)
            {
                Console.WriteLine("Enter Message (exit to termination): ");
                string newMessage = Console.ReadLine();
                if (newMessage == "exit")
                    break;
                var msg = new ServiceBusMessage(newMessage);

                //sessionId can be anything should be unique 
                //while sesstion is enable need to write follwing line code
                msg.SessionId = "mtechno-session";

                msg.ApplicationProperties.Add("Sender", "Rajan");
                msg.ApplicationProperties.Add("Created at", DateTime.Now);
                msg.ApplicationProperties.Add("Source", "MTechno");
                msg.ApplicationProperties.Add("Receiver", "MTechnoApp-mtechnoSB");

                //msg.TimeToLive = new TimeSpan(0, 0, 5);
                msg.MessageId = newMessage;
                await sender.SendMessageAsync(msg);
                Console.WriteLine("Sent: " + msg.MessageId);

            }

            await sender.DisposeAsync();
            await client.DisposeAsync();
        }
    }
}
