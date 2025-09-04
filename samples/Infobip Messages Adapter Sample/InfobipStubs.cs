using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infobip_Messages_Adapter_Sample
{
    // Temporary stub for Infobip Messages Adapter - replace with actual package when available
    public class InfobipMessagesAdapterOptions
    {
        public InfobipMessagesAdapterOptions(IConfiguration configuration)
        {
            // Configure from appsettings.json
            InfobipApiKey = configuration["InfobipApiKey"];
            InfobipMessagesApiBaseUrl = configuration["InfobipMessagesApiBaseUrl"];
            InfobipAppSecret = configuration["InfobipAppSecret"];
            InfobipMessagesApiKey = configuration["InfobipMessagesApiKey"] ?? configuration["InfobipApiKey"];
        }

        public string InfobipApiKey { get; set; }
        public string InfobipMessagesApiBaseUrl { get; set; }
        public string InfobipAppSecret { get; set; }
        public string InfobipMessagesApiKey { get; set; }
    }

    // Temporary stub for Infobip Messages Client
    public interface IInfobipMessagesClient
    {
        // Add Infobip-specific methods here when the actual package is available
    }

    public class InfobipMessagesClient : IInfobipMessagesClient
    {
        // Temporary implementation
    }

    // Temporary stub for Infobip Messages Adapter
    public class InfobipMessagesAdapter : BotFrameworkHttpAdapter
    {
        public InfobipMessagesAdapter(
            InfobipMessagesAdapterOptions adapterOptions,
            IInfobipMessagesClient infobipMessagesClient,
            ILogger<InfobipMessagesAdapter> logger,
            IConfiguration configuration = null)
            : base(configuration, logger)
        {
            // Initialize Infobip-specific functionality here when package is available
        }
    }

    public class InfobipMessagesAdapterWithErrorHandler : InfobipMessagesAdapter
    {
        public InfobipMessagesAdapterWithErrorHandler(
            InfobipMessagesAdapterOptions adapterOptions,
            IInfobipMessagesClient infobipMessagesClient,
            ILogger<InfobipMessagesAdapter> logger,
            IConfiguration configuration)
            : base(adapterOptions, infobipMessagesClient, logger, configuration)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");

                // Send a message to the user
                await turnContext.SendActivityAsync("The bot encountered an error or bug.");
                await turnContext.SendActivityAsync("To continue to run this bot, please fix the bot source code.");

                // Send a trace activity, which will be displayed in the Bot Framework Emulator
                await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
            };
        }
    }
}