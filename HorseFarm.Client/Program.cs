using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.ServiceBus.Messaging;
using Microsoft.Practices.TransientFaultHandling;
using HorseFarm.Common;

namespace HorseFarm.Client
{

    class Program
    {
        static void Main(string[] args)
        {
            MessagingFactory factory = MessagingFactory.Create();
            var looptime = 250;

            while (true)
            {

                Console.WriteLine("Type 'exit' to end or hit enter to receive messages"); // Prompt
                string line = Console.ReadLine(); // Get string from user
                if (line == "exit") // Check string
                {
                    break;
                }
                
                ReceiveAllMessagesFromSubscripions(factory);
                Console.WriteLine("".PadRight(50, '-'));
                Console.WriteLine();
                Console.WriteLine();
            }
            // RunLoop();
        }




        static void ReceiveAllMessagesFromSubscripions(MessagingFactory messagingFactory)
        {
            // Receive message from 3 subscriptions.
            Program.ReceiveAllMessageFromSubscription(messagingFactory, Conts.SubAllMessages);
            Program.ReceiveAllMessageFromSubscription(messagingFactory, Conts.YoungHorses);
            Program.ReceiveAllMessageFromSubscription(messagingFactory, Conts.OldHorses);
        }

        static void ReceiveAllMessageFromSubscription(MessagingFactory messagingFactory, string subsName)
        {
            int receivedMessages = 0;

            // Create subscription client.
            SubscriptionClient subsClient =
                messagingFactory.CreateSubscriptionClient(Conts.TopicName, subsName, ReceiveMode.ReceiveAndDelete);

            // Create a receiver from the subscription client and receive all messages.
            Console.WriteLine("\nReceiving messages from subscription {0}.", subsName);

            while (true)
            {
                BrokeredMessage receivedMessage;

                receivedMessage = subsClient.Receive(TimeSpan.FromSeconds(1));

                if (receivedMessage != null)
                {
                    Console.WriteLine();
                    DateTime dob = ((DateTime)receivedMessage.Properties["DOB"]);
                    Console.WriteLine("Id = {1} DOB = {0}", receivedMessage.MessageId, dob);

                    receivedMessage.Dispose();
                    receivedMessages++;


                }
                else
                {
                    // No more messages to receive.
                    break;
                }
            }

            Console.WriteLine("Received {0} messages from subscription {1}.", receivedMessages, subsClient.Name);
        }





        private static void RunLoop()
        {
            MessagingFactory factory = MessagingFactory.Create();
            SubscriptionClient auditSubscriptionClient = factory.CreateSubscriptionClient("IssueTrackingTopic", "AuditSubscription", ReceiveMode.ReceiveAndDelete);


            var retryPolicy = new RetryPolicy<ServiceBusTransientErrorDetectionStrategy>(RetryStrategy.DefaultClientRetryCount);

            var waitTimeout = TimeSpan.FromSeconds(10);

            // Declare an action acting as a callback whenever a message arrives on a queue.
            AsyncCallback completeReceive = null;

            // Declare an action acting as a callback whenever a non-transient exception occurs while receiving or processing messages.
            Action<Exception> recoverReceive = null;

            // Declare a cancellation token that is used to signal an exit from the receive loop.
            var cts = new CancellationTokenSource();

            // Declare an action implementing the main processing logic for received messages.
            Action<BrokeredMessage> processMessage = ((msg) =>
            {
                // Put your custom processing logic here. DO NOT swallow any exceptions.

                Console.WriteLine(msg.Properties["HorseId"]);

            });

            // Declare an action responsible for the core operations in the message receive loop.
            Action receiveMessage = (() =>
            {
                // Use a retry policy to execute the Receive action in an asynchronous and reliable fashion.
                retryPolicy.ExecuteAction
                (
                    (cb) =>
                    {
                        // Start receiving a new message asynchronously.
                        auditSubscriptionClient.BeginReceive(waitTimeout, cb, null);
                    },
                    (ar) =>
                    {
                        // Make sure we are not told to stop receiving while we were waiting for a new message.
                        if (!cts.IsCancellationRequested)
                        {
                            // Complete the asynchronous operation. This may throw an exception that will be handled internally by retry policy.
                            BrokeredMessage msg = auditSubscriptionClient.EndReceive(ar);

                            // Check if we actually received any messages.
                            if (msg != null)
                            {
                                // Make sure we are not told to stop receiving while we were waiting for a new message.
                                if (!cts.IsCancellationRequested)
                                {
                                    try
                                    {
                                        // Process the received message.
                                        processMessage(msg);

                                        // With PeekLock mode, we should mark the processed message as completed.
                                        if (auditSubscriptionClient.Mode == ReceiveMode.PeekLock)
                                        {
                                            // Mark brokered message as completed at which point it's removed from the queue.
                                            msg.Complete();
                                        }
                                    }
                                    catch
                                    {
                                        // With PeekLock mode, we should mark the failed message as abandoned.
                                        if (auditSubscriptionClient.Mode == ReceiveMode.PeekLock)
                                        {
                                            // Abandons a brokered message. This will cause Service Bus to unlock the message and make it available 
                                            // to be received again, either by the same consumer or by another completing consumer.
                                            msg.Abandon();
                                        }

                                        // Re-throw the exception so that we can report it in the fault handler.
                                        throw;
                                    }
                                    finally
                                    {
                                        // Ensure that any resources allocated by a BrokeredMessage instance are released.
                                        msg.Dispose();
                                    }
                                }
                                else
                                {
                                    // If we were told to stop processing, the current message needs to be unlocked and return back to the queue.
                                    if (auditSubscriptionClient.Mode == ReceiveMode.PeekLock)
                                    {
                                        msg.Abandon();
                                    }
                                }
                            }
                        }

                        // Invoke a custom callback method to indicate that we have completed an iteration in the message receive loop.
                        completeReceive(ar);
                    },
                    () =>
                    {
                        Console.WriteLine("Success Handler");
                    },
                    (ex) =>
                    {
                        // Invoke a custom action to indicate that we have encountered an exception and
                        // need further decision as to whether to continue receiving messages.
                        recoverReceive(ex);
                    });
            });

            // Initialize a custom action acting as a callback whenever a message arrives on a queue.
            completeReceive = ((ar) =>
            {
                if (!cts.IsCancellationRequested)
                {
                    // Continue receiving and processing new messages until we are told to stop.
                    receiveMessage();
                }
            });

            // Initialize a custom action acting as a callback whenever a non-transient exception occurs while receiving or processing messages.
            recoverReceive = ((ex) =>
            {
                // Just log an exception. Do not allow an unhandled exception to terminate the message receive loop abnormally.
                Console.WriteLine(ex.Message);

                if (!cts.IsCancellationRequested)
                {
                    // Continue receiving and processing new messages until we are told to stop regardless of any exceptions.
                    receiveMessage();
                }
            });

            // Start receiving messages asynchronously.
            receiveMessage();

            // Perform any other work. Message will keep arriving asynchronously while we are busy doing something else.

            // Stop the message receive loop gracefully.
            cts.Cancel();
        }


    }
}
