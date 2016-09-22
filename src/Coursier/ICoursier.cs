using System;

namespace Coursier
{
    public interface ICoursier
    {
        void Publish<TMessage>(TMessage message) where TMessage : BaseMessage;
        ISubscriptionToken Subscribe<TMessage>(Action<BaseMessage> msgHandler) where TMessage : BaseMessage;
        ISubscriptionToken SubscribeOnThreadPoolThread<TMessage>(Action<BaseMessage> msgHandler) where TMessage : BaseMessage;
        void Unsubscribe<TMessage>(ISubscriptionToken token) where TMessage : BaseMessage;
        int SubscriptionCount<TMessage>() where TMessage : BaseMessage;
        void ClearSubscriptionsFor<TMessage>() where TMessage : BaseMessage;
        void ClearAllSubscriptions();
    }
}