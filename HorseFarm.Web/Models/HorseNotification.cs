using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using HorseFarm.Common;

namespace HorseFarm.Web.Models
{
    public class HorseNotification : HorseFarm.Web.Models.IHorseNotification
    {
        private static TopicDescription topicDescription;
        private SubscriptionDescription myAuditSubscription;
        private SubscriptionDescription myAgentSubscription;
        TopicClient myTopicClient;

        NamespaceManager namespaceManager;

        const string SubsNameColorBlueSize10Orders = "OldHorses";
        const string SubsNameHighPriorityOrders = "YoungHorses";


        private static HorseNotification notify;
        public static HorseNotification GetHorseNotification()
        {
            if (notify == null)
                notify = new HorseNotification();
            return notify;

        }

        public HorseNotification()
        {
            namespaceManager = NamespaceManager.Create();


            DeleteTopicsAndSubscriptions(namespaceManager);
            CreateTopicsAndSubscriptions(namespaceManager);

            MessagingFactory factory = MessagingFactory.Create();
            myTopicClient = factory.CreateTopicClient(topicDescription.Path);

        }

        public void SendHorseChangedMessage(Horse horse)
        {
            BrokeredMessage message = new BrokeredMessage();
            message.Properties["HorseId"] = horse.Id;
            message.Properties["DOB"] = horse.DOB;
            myTopicClient.Send(message);
        }

        public void SendHorseAddedMessage(int horse)
        {
        }
        public void SendHorseDeletedMessage(int horse)
        {
        }

        static void CreateTopicsAndSubscriptions(NamespaceManager namespaceManager)
        {
            // Create a topic and 3 subscriptions.
            topicDescription = namespaceManager.CreateTopic(Conts.TopicName);
            try
            {
                namespaceManager.DeleteSubscription(topicDescription.Path, Conts.SubAllMessages);
            }
            catch { }

            try
            {
                namespaceManager.DeleteSubscription(topicDescription.Path, Conts.SubHolding);
            }
            catch { }

            try
            {
                namespaceManager.DeleteSubscription(topicDescription.Path, Conts.YoungHorses);
            }
            catch { }

            try
            {
                namespaceManager.DeleteSubscription(topicDescription.Path, Conts.OldHorses);
            }
            catch { }


            namespaceManager.CreateSubscription(topicDescription.Path, Conts.SubAllMessages, new TrueFilter());
            namespaceManager.CreateSubscription(topicDescription.Path, Conts.SubHolding, new FalseFilter());
            //namespaceManager.CreateSubscription(topicDescription.Path, Conts.YoungHorses, new TrueFilter());
            //namespaceManager.CreateSubscription(topicDescription.Path, Conts.OldHorses, new TrueFilter());

            namespaceManager.CreateSubscription(topicDescription.Path, Conts.YoungHorses, new SqlFilter("HorseId > 5"));
            namespaceManager.CreateSubscription(topicDescription.Path, Conts.OldHorses, new SqlFilter("HorseId <= 5"));
        }

        static void DeleteTopicsAndSubscriptions(NamespaceManager namespaceManager)
        {
            try
            {
                namespaceManager.DeleteTopic(Conts.TopicName);
            }
            catch { }


        }

        //static NamespaceManager CreateNamespaceManager()
        //{
        //    Uri rootAddressManagement = ServiceBusEnvironment.CreatePathBasedServiceUri("sb", Program.serviceBusNamespace, string.Format("{0}:{1}", Program.ServerFQDN, Program.HttpPort));

        //    NamespaceManagerSettings nmSettings = new NamespaceManagerSettings();
        //    nmSettings.TokenProvider = TokenProvider.CreateWindowsTokenProvider(new List<Uri>() { rootAddressManagement });

        //    return new NamespaceManager(rootAddressManagement, nmSettings);

        //}

        //static MessagingFactory CreateMessagingFactory()
        //{
        //    Uri rootAddressManagement = ServiceBusEnvironment.CreatePathBasedServiceUri("sb", Program.serviceBusNamespace, string.Format("{0}:{1}", Program.ServerFQDN, Program.HttpPort));
        //    Uri rootAddressRuntime = ServiceBusEnvironment.CreatePathBasedServiceUri("sb", Program.serviceBusNamespace, string.Format("{0}:{1}", Program.ServerFQDN, Program.TcpPort));

        //    MessagingFactorySettings mfSettings = new MessagingFactorySettings();
        //    mfSettings.TokenProvider = TokenProvider.CreateWindowsTokenProvider(
        //                   new List<Uri>() { rootAddressManagement });


        //    return MessagingFactory.Create(rootAddressRuntime, mfSettings);
        //}

        //static void GetNamespaceAndCredentials()
        //{
        //    Console.Write("Please provide the server FQDN to use (blank for localhost): ");
        //    Program.ServerFQDN = Console.ReadLine();
        //    if ((string.Compare(ServerFQDN, "localhost", true) == 0) || string.IsNullOrEmpty(ServerFQDN))
        //    {
        //        var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
        //        Program.ServerFQDN = string.Format("{0}.{1}", ipProperties.HostName, ipProperties.DomainName);
        //    }

        //    Console.Write("Please provide the service namespace to use: ");
        //    Program.serviceBusNamespace = Console.ReadLine();



        //}












    }

    //class AgeFilter : Filter
    //{

    //    public override bool Match(BrokeredMessage message)
    //    {
    //        DateTime dob = ((DateTime)message.Properties["DOB"]);

    //        return (dob > new DateTime(2010,12,31));
            
    //    }

    //    public override Filter Preprocess()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool RequiresPreprocessing
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public override void Validate()
    //    {
    //        throw new NotImplementedException();
    //    }
   // }
}