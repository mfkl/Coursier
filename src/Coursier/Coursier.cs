using System;
using System.Collections.Generic;
using System.Linq;

namespace Coursier
{
    public sealed class Coursier : ICoursier
    {
        readonly Dictionary<Type, Dictionary<Guid, HandlerWrapper>> _subscriptions;
        readonly object _locker = new object();

        public Coursier()
        {
            _subscriptions = new Dictionary<Type, Dictionary<Guid, HandlerWrapper>>();
        }

        public ISubscriptionToken Subscribe<TMessage>(Action<BaseMessage> msgHandler) where TMessage : BaseMessage
        {
            if(typeof(TMessage) == typeof(BaseMessage))
                throw new InvalidOperationException(nameof(BaseMessage) + " cannot be used as a message type.");

            return SubscribeInternal<TMessage>(msgHandler);
        }

        ISubscriptionToken SubscribeInternal<TMessage>(Action<BaseMessage> msgHandler, bool runOnThreadPoolThread = false)
            where TMessage : BaseMessage
        {
            var subscription = new SubscriptionToken(typeof(TMessage));

            lock(_locker)
            {
                var subscriptions = GetSubscriptionsFor<TMessage>();

                if(subscriptions.Count > 0)
                {
                    subscriptions.Add(subscription.Id, new HandlerWrapper(msgHandler, runOnThreadPoolThread));
                }
                else
                {
                    _subscriptions.Add(typeof(TMessage), new Dictionary<Guid, HandlerWrapper>
                    {
                        { subscription.Id, new HandlerWrapper(msgHandler, runOnThreadPoolThread) }
                    });
                }
            }

            return subscription;
        }

        public ISubscriptionToken SubscribeOnThreadPoolThread<TMessage>(Action<BaseMessage> msgHandler) where TMessage : BaseMessage
        {
            if(typeof(TMessage) == typeof(BaseMessage))
                throw new InvalidOperationException(nameof(BaseMessage) + " cannot be used as a message type.");

            return SubscribeInternal<TMessage>(msgHandler, true);
        }

        public void Unsubscribe<TMessage>(ISubscriptionToken token) where TMessage : BaseMessage
        {
            lock(_locker)
            { 
                var subscriptions = GetSubscriptionsFor<TMessage>();
                subscriptions.Remove(token.Id);
            }
        }

        public void Publish<TMessage>(TMessage message) where TMessage : BaseMessage
        {
            if(message == null)
                throw new NullReferenceException(nameof(message));

            lock (_locker)
            {
                var subscriptions = GetSubscriptionsFor<TMessage>();

                foreach (var handler in subscriptions)
                {
                    handler.Value.Invoke(message);
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

        Dictionary<Guid, HandlerWrapper> GetSubscriptionsFor<TMessage>() where TMessage : BaseMessage
        {
            lock(_locker)
            {
                Dictionary<Guid, HandlerWrapper> handlers;
                return _subscriptions.TryGetValue(typeof(TMessage), out handlers)
                    ? handlers
                    : Enumerable.Empty<KeyValuePair<Guid, HandlerWrapper>>() // not cool
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
        }
    }
}
