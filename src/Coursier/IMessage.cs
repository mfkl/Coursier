using System;

namespace Coursier
{
    public abstract class Message
    {
        string Content;
        Guid Id { get; } = Guid.NewGuid();
    }
}