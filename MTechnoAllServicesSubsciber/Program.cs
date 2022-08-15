using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace MTechnoAllServicesSubsciber
{
    class Program
    {
        static string connectionString = "Endpoint=sb://mtechnosb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=VAuvKn105u5pLWhX/T1+Pv+HST3fIXOj0fqA6X4QcAU=";
        static string topicName = "mtechno-topic";
        static string subscriptionName = "mtechno-all-services";
        static ServiceBusClient client;
        static ServiceBusProcessor processor;


        static async Task MessageHandler(ProcessMessageEventArgs processMessageEventArgs)
        {
            string body = processMessageEventArgs.Message.Body.ToString();
            Console.WriteLine($"Received : {body} from subsciption : {subscriptionName}");

            // when CompleteMessageAsync method is called message is deleted from the subscription queue
            await processMessageEventArgs.CompleteMessageAsync(processMessageEventArgs.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs processErrorEventArgs)
        {
            Console.WriteLine(processErrorEventArgs.Exception.ToString());
            return Task.CompletedTask;
        }

        static async Task Main(string[] args)
        {
            client = new ServiceBusClient(connectionString);
            var options = new ServiceBusProcessorOptions();
            options.ReceiveMode = ServiceBusReceiveMode.PeekLock;
            processor = client.CreateProcessor(topicName, subscriptionName, options);
            try
            {
                processor.ProcessMessageAsync += MessageHandler;
                processor.ProcessErrorAsync += ErrorHandler;
                await processor.StartProcessingAsync();
                Console.WriteLine("Wait for miniute and then press any key to end the process");
                Console.ReadKey();
                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }
}
