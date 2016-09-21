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
    }
}
