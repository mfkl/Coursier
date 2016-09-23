using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Coursier.Test
{
    public class CoursierTest
    {
        [Fact]
        public void ShouldPublishMessage()
        {
            var coursier = new Coursier();
            var msgReceived = false;

            coursier.Subscribe<TestMessageOne>(msg =>
            {
                msgReceived = true;
            });

            coursier.Publish(new TestMessageOne(this));

            Assert.True(msgReceived);
        }

        [Fact]
        public void ShouldNotReceiveMessageIfUnSubscribed()
        {
            var coursier = new Coursier();
            var msgReceived = false;

            var token = coursier.Subscribe<TestMessageOne>(msg =>
            {
                msgReceived = true;
            });

            coursier.Unsubscribe<TestMessageOne>(token);

            coursier.Publish(new TestMessageOne(this));

            Assert.False(msgReceived);
        }

        [Fact]
        public void ShouldKeepCountOfSubscriptions()
        {
            var coursier = new Coursier();

            var subOne = coursier.Subscribe<TestMessageOne>(msg => { });
            coursier.Subscribe<TestMessageOne>(msg => { });

            Assert.Equal(2, coursier.SubscriptionCount<TestMessageOne>());

            coursier.Unsubscribe<TestMessageOne>(subOne);

            Assert.Equal(1, coursier.SubscriptionCount<TestMessageOne>());
        }

        [Fact]
        public void ShouldClearSubscriptionsForGivenMessage()
        {
            var coursier = new Coursier();
            MakeABunchOfSubscriptions(coursier);

            coursier.ClearSubscriptionsFor<TestMessageTwo>();

            Assert.Equal(3, coursier.SubscriptionCount<TestMessageOne>());
            Assert.Equal(0, coursier.SubscriptionCount<TestMessageTwo>());
        }

        static void MakeABunchOfSubscriptions(ICoursier coursier)
        {
            coursier.Subscribe<TestMessageOne>(msg => { });
            coursier.Subscribe<TestMessageOne>(msg => { });
            coursier.Subscribe<TestMessageOne>(msg => { });

            coursier.Subscribe<TestMessageTwo>(msg => { });
            coursier.Subscribe<TestMessageTwo>(msg => { });
        }

        [Fact]
        public void ShouldRegisterAndInvokeAsyncHandler()
        {
            var coursier = new Coursier();
            var msgReceived = false;

            coursier.Subscribe<TestMessageOne>(async msg =>
            {
                await Task.Run(() =>
                {
                    msgReceived = true;
                });
            });

            coursier.Publish(new TestMessageOne(this));

            Thread.Sleep(1000);

            Assert.True(msgReceived);
        }

        [Fact]
        public void ShouldClearAllSubscriptions()
        {
            var coursier = new Coursier();

            var msgOneReceived = false;
            var msgTwoReceived = false;

            coursier.Subscribe<TestMessageOne>(msg => { msgOneReceived = true; });
            coursier.Subscribe<TestMessageTwo>(msg => { msgTwoReceived = true; });

            coursier.ClearAllSubscriptions();

            coursier.Publish(new TestMessageOne(this));
            coursier.Publish(new TestMessageTwo(this));

            Assert.False(msgOneReceived);
            Assert.False(msgTwoReceived);
        }

        [Fact]
        public void ShouldThrowIfMessageIsNull()
        {
            var coursier = new Coursier();

            Assert.Throws<NullReferenceException>(() => coursier.Publish<TestMessageOne>(null));
        }

        [Fact]
        public void ShouldThrowIfMessageSenderIsNull()
        {
            Assert.Throws<NullReferenceException>(() => new TestMessageOne(null));
        }

        [Fact]
        public void ShouldThrowIfSubscribingToBaseMessage()
        {
            var coursier = new Coursier();

            Assert.Throws<InvalidOperationException>(() => coursier.Subscribe<BaseMessage>(msg => { }));
        }

        [Fact]
        public void ShouldRunOnThreadPoolThread()
        {
            var coursier = new Coursier();

            coursier.SubscribeOnThreadPoolThread<TestMessageOne>(msg =>
            {
                Assert.True(Thread.CurrentThread.IsBackground);
            });

            coursier.Publish(new TestMessageOne(this));
        }

        [Fact]
        public void ShouldNotRunOnThreadPoolThread()
        {
            var coursier = new Coursier();

            coursier.SubscribeOnThreadPoolThread<TestMessageOne>(msg =>
            {
                Assert.False(Thread.CurrentThread.IsBackground);
            });

            coursier.Publish(new TestMessageOne(this));
        }
    }
}
