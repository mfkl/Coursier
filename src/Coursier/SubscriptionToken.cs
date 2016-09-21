using System;

namespace Coursier
{
    internal class SubscriptionToken : ISubscriptionToken
    {
        public Action<BaseMessage> Handler { get; }
        public Guid Id { get; }
        public SubscriptionToken(Action<BaseMessage> handler)
        {
            Handler = handler;
            Id = Guid.NewGuid();
        }
    }
}