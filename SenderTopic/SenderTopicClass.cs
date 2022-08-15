using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace SenderQueue
{

    class SenderQueueClass
    {

        static string connectionString = "Endpoint=sb://mtechnosb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=VAuvKn105u5pLWhX/T1+Pv+HST3fIXOj0fqA6X4QcAU=";
        static string topicName = "mtechno-topic";

        static async Task Main(string[] args)
        {
            ServiceBusClient client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(topicName);
            //using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();


            while (true)
            {
                Console.WriteLine("This is Sender Topic");
                Console.WriteLine("Plaease enter service name (exit to terminate):");
                string newMessage = Console.ReadLine();
                if (newMessage == "exit")
                    break;
                var msg = new ServiceBusMessage(newMessage);

                Console.WriteLine("Please Enter Service Type:");
                string serviceType = Console.ReadLine();
                // Subscription properties can be added as per below, as per application requeiremenet

                msg.ApplicationProperties.Add("ServiceType", serviceType);
                msg.ApplicationProperties.Add("Created at", DateTime.Now);
                msg.ApplicationProperties.Add("Source", "MTechno");
                msg.ApplicationProperties.Add("Receiver", "MTechnoApp-mtechnoSB");


                //custome time out period at message level, if you not set explicitly, automatic quque level reflected
                //msg.TimeToLive = new TimeSpan(0, 0, 10);

                // to enable the duplicate need to identify and compare message id with message string
                msg.MessageId = newMessage;


                // to send sending messages batch need to use this method:SendMessagesAsync
                // await sender.SendMessagesAsync(messageBatch);


                //here we are checking weather message is empty string or not
                if (newMessage != "")
                {
                    await sender.SendMessageAsync(msg);
                    Console.WriteLine("Sent....");
                }

            }

            await sender.DisposeAsync();
            await client.DisposeAsync();

        }
    }
}
