using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;



namespace ReceiverSessionQueue
{
    class ReceiverSessionQueueClass
    {
        //Edit your connection string
        static string connectionString = "Endpoint=sb://mtechnosb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=VAuvKn105u5pLWhX/T1+Pv+HST3fIXOj0fqA6X4QcAU=";
        //Edit your queue name
        static string queueName = "mtechno-session-queue";
        static ServiceBusClient client;
        static ServiceBusSessionProcessor sessionProcessor;

        static async Task MessageHandler(ProcessSessionMessageEventArgs processSessionMessageEventArgs)
        {
            string body = processSessionMessageEventArgs.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");
            await processSessionMessageEventArgs.CompleteMessageAsync(processSessionMessageEventArgs.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs processErrorEventArgs)
        {
            Console.WriteLine(processErrorEventArgs.Exception.ToString());
            return Task.CompletedTask;
        }
        static async Task Main(string[] args)
        {
            client = new ServiceBusClient(connectionString);

            var options = new ServiceBusSessionProcessorOptions()
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                AutoCompleteMessages = false,
                SessionIds = { "mtechno-session" }
            };
            

            sessionProcessor = client.CreateSessionProcessor(queueName, options);

            sessionProcessor.ProcessMessageAsync += MessageHandler;
            sessionProcessor.ProcessErrorAsync += ErrorHandler;
            await sessionProcessor.StartProcessingAsync();

            Console.WriteLine("Wait for minute and then press any key to end the processing");
            Console.ReadKey();

            //stop processing
            Console.WriteLine("\nStopping the receiver...");
            await sessionProcessor.StopProcessingAsync();
            Console.WriteLine("Stopped receiving message");

            await sessionProcessor.DisposeAsync();
            await client.DisposeAsync();


        }
    }
}
