# Coursier
Simple in-process pub/sub

Coursier is a basic implementation of the pub/sub messaging pattern.

## Installation

`Install-Package Coursier`

## Use

1. Create your own message type inheriting from the `BaseMessage` abstract class

2. Create (or have your favorite IoC container create for you) and get a hold of an instance of `Coursier`.

3. Call `ISubscriptionToken Subscribe<TMessage>(Action<BaseMessage> msgHandler) where TMessage : BaseMessage;` on it.
  * That call will return a subscription token which you could store in a field for later use.

4. Still using `Coursier`, call `void Publish<TMessage>(TMessage message) where TMessage : BaseMessage;` with an instance of your newly created message type.

5. Whenever, call `void Unsubscribe<TMessage>(ISubscriptionToken token) where TMessage : BaseMessage;` to unsubscribe with the previously stored subscription token.
