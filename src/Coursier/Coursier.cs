using System;
using System.Collections.Generic;

namespace Coursier
{
    public sealed class Coursier : ICoursier
    {
        readonly Dictionary<Type, List<ISubscriptionToken>> _subscriptions;
        readonly object _locker = new object();

        public Coursier()
        {
            _subscriptions = new Dictionary<Type, List<ISubscriptionToken>>();
        }

        public ISubscriptionToken Subscribe<TMessage>(Action<BaseMessage> msgHandler) where TMessage : BaseMessage
        {
            var subscription = new SubscriptionToken(msgHandler);

            lock(_locker)
            {

                List<ISubscriptionToken> tokensForMessage;
                if(_subscriptions.TryGetValue(typeof(TMessage), out tokensForMessage))
                {
                    tokensForMessage.Add(subscription);
                }
                else
                {
                    _subscriptions.Add(typeof(TMessage), new List<ISubscriptionToken> { subscription });
                }
            }

            return subscription;
        }

        public void UnSubscribe<TMessage>(ISubscriptionToken token) where TMessage : BaseMessage
        {
            var msgType = typeof(TMessage);
            lock(_locker)
            { 
                List<ISubscriptionToken> subscriptions;
                if(!_subscriptions.TryGetValue(msgType, out subscriptions))
                    return;

                subscriptions.Remove(token);
            }
        }


        public void Publish<TMessage>(TMessage message) where TMessage : BaseMessage
        {
            lock (_locker)
            {
                List<ISubscriptionToken> tokensForMessage;
                if(!_subscriptions.TryGetValue(typeof(TMessage), out tokensForMessage))
                    return;
                
                foreach (var subscription in tokensForMessage)
                {
                    subscription.Handler(message);
                }
            }
        }
    }
}
