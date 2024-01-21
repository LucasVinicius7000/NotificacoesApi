namespace Notificacoes.Services
{
    public interface IMessageService<T, Q>
    {
        public void PostMessage(T message, string topicName);
        public void PostMessages(List<T> messages, string topicName);
        public Q GetMessage(string topicName, string subscriptionName);
        public List<Q> GetMessages(int number, string topicName, string subscriptionName);
    }
}
