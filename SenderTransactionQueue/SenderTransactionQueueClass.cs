using System;
using System.Transactions;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace SenderTransactionQueue
{
    class SenderTransactionQueueClass
    {          

        static void Main(string[] args)
        {
            var connectionString = "Endpoint=sb://mtechnosb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=VAuvKn105u5pLWhX/T1+Pv+HST3fIXOj0fqA6X4QcAU=";
            var queueName = "mtechno-transaction-queue";
            var queueClient = new QueueClient(connectionString, queueName);

            using (TransactionScope scope = new TransactionScope())
            {
                for (int i = 0; i < 10; i++)
                {
                    //send message
                    Message msg = new Message(System.Text.Encoding.UTF8.GetBytes("Message + i"));

                    //Note: Partitioned queues and topics aren't supported in the Premium messaging tier. Sessions are supported in the premier tier by using SessionId.

                    //here we have option to use from the following
                    //1 sesstionId
                    //2 PartitionKey
                    //3 MessageId

                    msg.PartitionKey = "mtechno-partition";
                    queueClient.SendAsync(msg).Wait();
                    Console.WriteLine(".");
                }
                Console.WriteLine("Done!");
                Console.WriteLine();

                //should we commit the transaction ?
                Console.WriteLine("Commit sent 10 message? (yes or no)");
                string reply = Console.ReadLine();
                if (reply.ToLower().Equals("yes"))

                    scope.Complete();                     
            }
            Console.WriteLine();

        }
    }
}
