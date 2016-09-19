using System;
using System.Collections.Generic;
using System.Reflection;

namespace Coursier
{
    public sealed class Coursier : ICoursier
    {
        readonly Dictionary<Guid, Action<Message>>  _subscriptions;
        readonly object _locker = new object();

        public Coursier()
        {
            _subscriptions = new Dictionary<Guid, Action<Message>>();
        }

        public ISubscriptionToken Subscribe<TMessage>(Action<Message> msgHandler) where TMessage : Message
        {
            var subscriptionKey = Guid.NewGuid();

            lock(_locker)
            {
                _subscriptions.Add(subscriptionKey, msgHandler);
            }

            return new SubscriptionToken(subscriptionKey);
        }

        public void Publish(Message message)
        {
            // foreach subscriber registered, call handlers of those interested in this message. 
            foreach (var h in _subscriptions)
            {
                Console.WriteLine(h.Value.Target);
            }
        }
    }

    public interface ISubscriptionToken
    {

    }

    public class SubscriptionToken : ISubscriptionToken
    {
        readonly Guid _subscriptionKey;

        public SubscriptionToken(Guid subscriptionKey)
        {
            _subscriptionKey = subscriptionKey;
        }
    }
    
}
