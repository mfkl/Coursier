using System;

namespace Coursier
{
    internal class SubscriptionToken : ISubscriptionToken
    {
        public Type MessageType { get; }
        public Guid Id { get; }
        public SubscriptionToken(Type messageType)
        {
            MessageType = messageType;
            Id = Guid.NewGuid();
        }
    }
}