using System;

namespace Coursier
{
    public abstract class BaseMessage
    {
        public readonly object Sender;

        protected BaseMessage(object sender)
        {
            if(sender == null)
                throw new NullReferenceException(nameof(sender));

            Sender = sender;
        }
    }
}