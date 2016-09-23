using System;

namespace Coursier
{
    public interface ISubscriptionToken
    {
        Type MessageType { get; }
        Guid Id { get; }
    }
}