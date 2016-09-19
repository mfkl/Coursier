using System;

namespace Coursier
{
    public interface ICoursier
    {
        ISubscriptionToken Subscribe<TMessage>(Action<Message> msgHandler) where TMessage : Message;

        void Publish(Message message);
    }
}