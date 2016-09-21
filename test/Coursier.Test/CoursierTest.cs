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

            coursier.Subscribe<TestMessage>(msg =>
            {
                msgReceived = true;
            });

            coursier.Publish(new TestMessage(this));

            Assert.True(msgReceived);
        }

        [Fact]
        public void ShouldNotReceiveMessageIfUnSubscribed()
        {
            var coursier = new Coursier();
            var msgReceived = false;

            var token = coursier.Subscribe<TestMessage>(msg =>
            {
                msgReceived = true;
            });

            coursier.UnSubscribe<TestMessage>(token);

            coursier.Publish(new TestMessage(this));

            Assert.False(msgReceived);
        }
    }
}
