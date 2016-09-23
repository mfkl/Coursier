using System;

namespace Coursier
{
    public interface ICoursier
    {
        /// <summary>
        /// Publishes a new message of type <see cref="TMessage">TMessage</see> to subscribers.
        /// </summary>
        /// <typeparam name="TMessage">TMessage type inheriting from <see cref="BaseMessage">BaseMessage</see>.</typeparam>
        /// <param name="message">The message</param>
        void Publish<TMessage>(TMessage message) where TMessage : BaseMessage;
        
        /// <summary>
        /// Subscribes to the given message type with the given handler.
        /// </summary>
        /// <typeparam name="TMessage">The message type being subscribed to.</typeparam>
        /// <param name="msgHandler">The handler to be invoked.</param>
        /// <returns>The subscription token of type <see cref="ISubscriptionToken"/>.</returns>
        ISubscriptionToken Subscribe<TMessage>(Action<BaseMessage> msgHandler) where TMessage : BaseMessage;

        /// <summary>
        /// Subscribes to the given message type with the given handler to be run on the thread pool.
        /// </summary>
        /// <typeparam name="TMessage">The message type being subscribed to.</typeparam>
        /// <param name="msgHandler">The handler to be invoked on the thread pool.</param>
        /// <returns>The subscription token of type <see cref="ISubscriptionToken"/></returns>
        ISubscriptionToken SubscribeOnThreadPoolThread<TMessage>(Action<BaseMessage> msgHandler) where TMessage : BaseMessage;

        /// <summary>
        /// Unsubscribes from the given message type using given subscription token.
        /// </summary>
        /// <typeparam name="TMessage">The message type to unsubscribe from.</typeparam>
        /// <param name="token">The token needed to unsubscribe.</param>
        void Unsubscribe<TMessage>(ISubscriptionToken token) where TMessage : BaseMessage;

        /// <summary>
        /// Subscription count for <see cref="TMessage">message type</see>.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns>The total number of subscribers of a given message type.</returns>
        int SubscriptionCount<TMessage>() where TMessage : BaseMessage;

        /// <summary>
        /// Removes all subscriptions for the given <see cref="TMessage">message type</see>.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        void ClearSubscriptionsFor<TMessage>() where TMessage : BaseMessage;

        /// <summary>
        /// Removes all subscriptions for all message types.
        /// </summary>
        void ClearAllSubscriptions();
    }
}