using System;
using System.Threading.Tasks;

namespace Coursier
{
    public class HandlerWrapper
    {
        readonly Action<BaseMessage> _handler;
        readonly bool _runOnThreadPoolThread;

        public HandlerWrapper(Action<BaseMessage> handler, bool runOnThreadPoolThread = false)
        {
            _handler = handler;
            _runOnThreadPoolThread = runOnThreadPoolThread;
        }

        public void Invoke(BaseMessage msg)
        {
            if(_runOnThreadPoolThread)
            {
                Task.Run(() => _handler(msg));
            }
            else
            {
                _handler(msg);
            }
        }
    }
}