using System;
using System.Collections.Generic;
using System.Reflection;

namespace Coursier
{
    public sealed class Coursier : ICoursier
    {
        readonly Dictionary<Type, List<SubscriptionToken>> _subscriptions;
        readonly object _locker = new object();

        public Coursier()
        {
            _subscriptions = new Dictionary<Type, List<SubscriptionToken>>();
        }

        public ISubscriptionToken Subscribe<TMessage>(Action<BaseMessage> msgHandler) where TMessage : BaseMessage
        {
            var subscription = new SubscriptionToken(msgHandler);

            lock(_locker)
            {

                List<SubscriptionToken> tokensForMessage;
                if(_subscriptions.TryGetValue(typeof(TMessage), out tokensForMessage))
                {
                    tokensForMessage.Add(subscription);
                }
                else
                {
                    _subscriptions.Add(typeof(TMessage), new List<SubscriptionToken> { subscription });
                }
            }

            return subscription;
        }

        public void Publish<TMessage>(TMessage message) where TMessage : BaseMessage
        {
            lock (_locker)
            {
                List<SubscriptionToken> tokensForMessage;
                if(!_subscriptions.TryGetValue(typeof(TMessage), out tokensForMessage))
                    return;
                
                foreach (var subscription in tokensForMessage)
                {
                    subscription.Handler(message);
                }
            }
        }
    }

    public interface ISubscriptionToken
    {
        Action<BaseMessage> Handler { get; }
    }

    internal class SubscriptionToken : ISubscriptionToken
    {
        public Action<BaseMessage> Handler { get; }

        public SubscriptionToken(Action<BaseMessage> handler)
        {
            Handler = handler;
        }

    }
    
}
