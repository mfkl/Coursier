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

            coursier.Subscribe<WelcomeMessage>(msg =>
            {
                msgReceived = true;
            });

            coursier.Publish(new WelcomeMessage("Welcome!"));

            Assert.True(msgReceived);
        }
    }
}
