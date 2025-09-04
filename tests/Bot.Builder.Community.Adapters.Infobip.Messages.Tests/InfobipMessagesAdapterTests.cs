using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bot.Builder.Community.Adapters.Infobip.Core.Models;
using Bot.Builder.Community.Adapters.Infobip.Messages.Models;
using Bot.Builder.Community.Adapters.Infobip.Messages.Tests.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Bot.Builder.Community.Adapters.Infobip.Messages.Tests
{
    public class InfobipMessagesAdapterTests
    {
        private readonly Mock<IInfobipMessagesClient> _mockClient;
        private readonly Mock<ILogger<InfobipMessagesAdapter>> _mockLogger;
        private readonly InfobipMessagesAdapterOptions _adapterOptions;

        public InfobipMessagesAdapterTests()
        {
            _mockClient = new Mock<IInfobipMessagesClient>();
            _mockLogger = new Mock<ILogger<InfobipMessagesAdapter>>();
            _adapterOptions = TestOptions.Get();
        }

        [Fact]
        public async Task ProcessIncomingMessageActivity()
        {
            var incomingMessage = new InfobipIncomingMessage<InfobipMessagesIncomingResult>
            {
                Results = new List<InfobipMessagesIncomingResult>
                {
                    new InfobipMessagesIncomingResult
                    {
                        MessageId = "test-message-id",
                        From = "subscriber-number",
                        To = "messages-number",
                        ReceivedAt = System.DateTimeOffset.UtcNow,
                        Channel = "whatsapp",
                        Platform = "whatsapp",
                        Message = new InfobipMessagesIncomingMessage
                        {
                            Type = InfobipMessagesMessageTypes.Text,
                            Text = "Hello, bot!"
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(incomingMessage);
            var httpRequest = CreateHttpRequest(json);
            var httpResponse = new Mock<HttpResponse>();
            httpResponse.SetupProperty(r => r.StatusCode);

            var bot = new TestBot
            {
                OnMessageActivity = (count, turnContext, cancellationToken) =>
                {
                    Assert.Equal("Hello, bot!", turnContext.Activity.Text);
                    Assert.Equal("subscriber-number", turnContext.Activity.From.Id);
                    Assert.Equal(InfobipMessagesConstants.ChannelId, turnContext.Activity.ChannelId);
                    return Task.CompletedTask;
                }
            };

            var adapter = new InfobipMessagesAdapter(_adapterOptions, _mockClient.Object, _mockLogger.Object);

            await adapter.ProcessAsync(httpRequest, httpResponse.Object, bot, CancellationToken.None);

            Assert.Equal(200, httpResponse.Object.StatusCode);
            Assert.Equal(1, bot.OnMessageActivityInvocationCount);
        }

        [Fact]
        public async Task SendMessageActivity()
        {
            var mockResponse = new InfobipMessagesResponse
            {
                MessageId = "sent-message-id",
                To = "subscriber-number",
                MessageCount = 1,
                Status = new InfobipMessagesStatus
                {
                    Id = 1,
                    Name = "PENDING_ENROUTE",
                    Description = "Message sent to next instance"
                }
            };

            _mockClient.Setup(c => c.SendAsync<InfobipMessagesResponse>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(mockResponse);

            var adapter = new InfobipMessagesAdapter(_adapterOptions, _mockClient.Object, _mockLogger.Object);

            var activity = new Activity
            {
                Type = ActivityTypes.Message,
                Text = "Hello from bot!",
                From = new ChannelAccount { Id = "messages-number" },
                Recipient = new ChannelAccount { Id = "subscriber-number" }
            };

            var turnContext = new TurnContext(adapter, activity);

            var responses = await adapter.SendActivitiesAsync(turnContext, new[] { activity }, CancellationToken.None);

            Assert.Single(responses);
            Assert.Equal("sent-message-id", responses[0].Id);

            _mockClient.Verify(c => c.SendAsync<InfobipMessagesResponse>(
                It.Is<object>(msg => 
                    msg.GetType().GetProperty("Content").GetValue(msg, null).GetType().GetProperty("Text").GetValue(
                        msg.GetType().GetProperty("Content").GetValue(msg, null), null).ToString() == "Hello from bot!"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        private HttpRequest CreateHttpRequest(string body)
        {
            var mockRequest = new Mock<HttpRequest>();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(body));
            
            mockRequest.Setup(r => r.Body).Returns(stream);
            mockRequest.Setup(r => r.Headers).Returns(new HeaderDictionary());
            
            return mockRequest.Object;
        }
    }
}