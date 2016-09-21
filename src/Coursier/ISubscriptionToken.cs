using System;

namespace Coursier
{
    public interface ISubscriptionToken
    {
        Action<BaseMessage> Handler { get; }
        Guid Id { get; }
    }
}