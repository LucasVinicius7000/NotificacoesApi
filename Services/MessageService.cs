using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Notificacoes.Services;

namespace Notificacoes.Services
{
    public class MessageService : IMessageService<ServiceBusMessage, ServiceBusReceivedMessage>
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusAdministrationClient _administrationClient;

        public MessageService(string connectionString) 
        {
            _serviceBusClient = new ServiceBusClient(connectionString);
            _administrationClient = new ServiceBusAdministrationClient(connectionString);
        }

        public ServiceBusReceivedMessage GetMessage(string topicName, string subscriptionName)
        {
            ServiceBusReceiver receiver = _serviceBusClient.CreateReceiver(topicName, subscriptionName);
            var message = receiver.ReceiveMessageAsync().Result;
            return message;
        }

        public List<ServiceBusReceivedMessage> GetMessages(int number, string topicName, string subscriptionName)
        {
            try
            {
                CreateSubscriptionForTopic(topicName, subscriptionName);
                ServiceBusReceiver receiver = _serviceBusClient.CreateReceiver(topicName, subscriptionName);
                var messages = receiver.PeekMessagesAsync(number).Result;
                return messages.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void PostMessage(ServiceBusMessage message, string topicName)
        {
            try
            {
                ServiceBusSender sender = _serviceBusClient.CreateSender(topicName);
                sender.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void PostMessages(List<ServiceBusMessage> messages, string topicName)
        {
            throw new NotImplementedException();
        }

        public bool SubscriptionExists(string topicName, string subscriptionName) 
        {
            return _administrationClient.SubscriptionExistsAsync(topicName, subscriptionName).Result;
        }

        public void CreateSubscriptionForTopic(string topicName, string subscriptionName)
        {
            if(!SubscriptionExists(topicName, subscriptionName))
            {
                try
                {
                    var subscriptionOptions = new CreateSubscriptionOptions(topicName, subscriptionName)
                    {
                        AutoDeleteOnIdle = TimeSpan.FromDays(7),
                        DefaultMessageTimeToLive = TimeSpan.FromDays(2),
                        EnableBatchedOperations = true,
                    };

                    var createdSubscription = _administrationClient.CreateSubscriptionAsync(subscriptionOptions);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

    }
}
