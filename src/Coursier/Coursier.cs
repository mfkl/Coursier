using System;
using System.Collections.Generic;
using System.Linq;

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
            if(typeof(TMessage) == typeof(BaseMessage))
                throw new InvalidOperationException(nameof(BaseMessage) + " cannot be used as a message type.");

            var subscription = new SubscriptionToken(msgHandler);

            lock(_locker)
            {
                var subscriptions = GetSubscriptionsFor<TMessage>();

                if(subscriptions.Count > 0)
                {
                    subscriptions.Add(subscription);
                }
                else
                {
                    _subscriptions.Add(typeof(TMessage), new List<ISubscriptionToken> { subscription });
                }
            }

            return subscription;
        }

        public ISubscriptionToken SubscribeOnThreadPoolThread<TMessage>(Action<BaseMessage> msgHandler) where TMessage : BaseMessage
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TMessage>(ISubscriptionToken token) where TMessage : BaseMessage
        {
            lock(_locker)
            { 
                var subscriptions = GetSubscriptionsFor<TMessage>();
                subscriptions.Remove(token);
            }
        }

        public void Publish<TMessage>(TMessage message) where TMessage : BaseMessage
        {
            if(message == null)
                throw new NullReferenceException(nameof(message));

            lock (_locker)
            {
                var subscriptions = GetSubscriptionsFor<TMessage>();

                foreach (var subscription in subscriptions)
                {
                    subscription.Handler(message);
                }
            }
        }

        public int SubscriptionCount<TMessage>() where TMessage : BaseMessage
        {
            lock (_locker)
            {
                var subscriptions = GetSubscriptionsFor<TMessage>();
                return subscriptions.Count;
            }
        }

        public void ClearSubscriptionsFor<TMessage>() where TMessage : BaseMessage
        {
            lock (_locker)
            {
                var subscriptions = GetSubscriptionsFor<TMessage>();
                subscriptions.Clear();
            }
        }

        public void ClearAllSubscriptions()
        {
            lock (_locker)
            {
                _subscriptions.Clear();
            }
        }

        List<ISubscriptionToken> GetSubscriptionsFor<TMessage>() where TMessage : BaseMessage
        {
            lock(_locker)
            {
                List<ISubscriptionToken> tokensForMessage;
                return _subscriptions.TryGetValue(typeof(TMessage), out tokensForMessage) ?
                    tokensForMessage :
                    Enumerable.Empty<ISubscriptionToken>().ToList();
            }
        }
    }
}
