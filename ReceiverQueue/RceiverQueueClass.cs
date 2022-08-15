using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace ReceiverQueue
{
    class RceiverQueueClass
    {
        //Edit your connection string here
        static string connectionString = "Endpoint=sb://mtechnosb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=VAuvKn105u5pLWhX/T1+Pv+HST3fIXOj0fqA6X4QcAU=";
        //Edit your queue name
        static string queueName = "mtechno-queue";
        static ServiceBusClient client;
        static ServiceBusProcessor processor;


        //This method use default arugment ProcessMessageEventArgs class to process the message
        static async Task MessageHandler(ProcessMessageEventArgs processMessageEventArgs)
        {
            string body = processMessageEventArgs.Message.Body.ToString();
            Console.WriteLine($"Received:{body}, Count:{processMessageEventArgs.Message.DeliveryCount}" +
            " " + $"Dead Letter Error Description:{processMessageEventArgs.Message.DeadLetterErrorDescription}");

            // when CompleteMessageAsync method is called message is deleted from the queue
            await processMessageEventArgs.CompleteMessageAsync(processMessageEventArgs.Message);
        }

        //This method use default argument ProcessErrorEventArgs class to find if there is any error while processing the message
        static Task ErrorHandler(ProcessErrorEventArgs processErrorEventArgs)
        {
            Console.WriteLine(processErrorEventArgs.Exception.ToString());
            return Task.CompletedTask;
        }

        static async Task Main()
        {
            client = new ServiceBusClient(connectionString);
            processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions()
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                // if set AutoCompleteMessages=false explicitly the message will not delete from the queue and
                // it will process multiple times, the default AutoCompleteMessages=true

                //AutoCompleteMessages = false
            });

            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;
            await processor.StartProcessingAsync();

            Console.WriteLine("This is Receiver Queue");
            Console.WriteLine("Wait for a minute and then press any key to end the processing");
            Console.ReadLine();

            //stop processing
            Console.WriteLine("\nStopping the receiver...");
            await processor.StopProcessingAsync();
            Console.WriteLine("Stopped receiving message");

            await processor.DisposeAsync();
            await client.DisposeAsync();

        }


    }

}
