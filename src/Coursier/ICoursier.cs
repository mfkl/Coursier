using System;

namespace Coursier
{
    public interface ICoursier
    {
        void Publish<TMessage>(TMessage message) where TMessage : BaseMessage;
        ISubscriptionToken Subscribe<TMessage>(Action<BaseMessage> msgHandler) where TMessage : BaseMessage;
        void UnSubscribe<TMessage>(ISubscriptionToken token) where TMessage : BaseMessage;
    }
}