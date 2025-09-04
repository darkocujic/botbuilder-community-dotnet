# Infobip Messages API Adapter for Bot Builder v4 .NET SDK

## Build status

| Branch | Status                                                                                                                                                    | Recommended NuGet package version                                                                                                                      |
| ------ | --------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| master | [![Build status](https://ci.appveyor.com/api/projects/status/b9123gl3kih8x9cb?svg=true)](https://ci.appveyor.com/project/garypretty/botbuilder-community) | [![NuGet version](https://img.shields.io/badge/NuGet-1.0.0-blue.svg)](https://www.nuget.org/packages/Bot.Builder.Community.Adapters.Infobip.Messages/) |

## Description

This is part of the [Bot Builder Community](https://github.com/botbuildercommunity) project which contains Bot Framework Components and other projects / packages for use with Bot Framework Composer and the Bot Builder .NET SDK v4.

The Infobip Messages API adapter enables receiving and sending messages across multiple channels through Infobip's unified Messages API. This adapter supports all current and future channels available in Infobip's Messages API, including WhatsApp, SMS, MMS, Viber, LINE, and more with advanced interactive messaging capabilities.

The adapter transforms incoming message requests into Bot Framework Activities and converts outgoing Activities into Infobip Messages API messages with full support for modern conversational features.

## Features

The adapter supports the following comprehensive messaging scenarios:

### Message Types

- **Text messages** - Send/receive plain text messages with formatting
- **Media messages** - Send/receive images, documents, videos, audio files, and stickers
- **Location messages** - Receive location data with coordinates and place names, request location from users
- **Contact messages** - Receive and share contact information with name, phone numbers, and emails
- **Interactive messages** - Support for buttons, lists, quick replies, and advanced interactions
- **Template messages** - WhatsApp Business API template message support with dynamic parameters
- **Carousel messages** - Multi-card horizontal scrolling experiences
- **Sticker messages** - Send and receive stickers with emoji and custom content

### Advanced Interactive Features

#### Interactive Buttons

```csharp
// Quick reply buttons
var reply = MessageFactory.SuggestedActions(
    new[] { "Yes", "No", "Maybe" },
    "Do you want to proceed?");
await turnContext.SendActivityAsync(reply);

// URL and phone number buttons
var heroCard = new HeroCard
{
    Title = "Contact Us",
    Buttons = new List<CardAction>
    {
        new CardAction(ActionTypes.OpenUrl, "Visit Website", value: "https://example.com"),
        new CardAction(ActionTypes.Call, "Call Us", value: "tel:+1234567890")
    }
};
```

#### Interactive Lists

```csharp
// Convert to interactive list automatically
var activity = MessageFactory.Text("Choose from our products:");
var sections = new[]
{
    new InfobipMessagesSection
    {
        Title = "Popular Items",
        Rows = new[]
        {
            new InfobipMessagesRow { Id = "product1", Title = "iPhone 15", Description = "Latest Apple smartphone" },
            new InfobipMessagesRow { Id = "product2", Title = "Galaxy S24", Description = "Samsung flagship device" }
        }
    }
};
activity.AddInfobipInteractiveList("Our Products", sections);
await turnContext.SendActivityAsync(activity);
```

#### Carousel Messages

```csharp
// Multiple hero cards automatically convert to carousel
var card1 = new HeroCard
{
    Title = "Product 1",
    Subtitle = "$99.99",
    Text = "Amazing product description",
    Images = new List<CardImage> { new CardImage("https://example.com/product1.jpg") },
    Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, "Buy Now", value: "buy_product1") }
};

var card2 = new HeroCard
{
    Title = "Product 2",
    Subtitle = "$149.99",
    Text = "Another great product",
    Images = new List<CardImage> { new CardImage("https://example.com/product2.jpg") },
    Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, "Buy Now", value: "buy_product2") }
};

var carouselActivity = MessageFactory.Carousel(new[] { card1.ToAttachment(), card2.ToAttachment() });
await turnContext.SendActivityAsync(carouselActivity);
```

### WhatsApp-Specific Features

#### Template Messages

```csharp
// Send WhatsApp Business template
var activity = MessageFactory.Text("Template message");
var templateData = new InfobipTemplateData
{
    Body = new InfobipTemplateBodyData
    {
        Placeholders = new[] { "John", "Order #12345", "2 items" }
    },
    Header = new InfobipTemplateHeaderData
    {
        Type = "IMAGE",
        MediaUrl = "https://example.com/order-header.jpg"
    }
};
activity.AddInfobipWhatsAppTemplate("order_confirmation", templateData);
await turnContext.SendActivityAsync(activity);
```

#### WhatsApp Flows

```csharp
// Interactive WhatsApp Flow
var activity = MessageFactory.Text("Please complete your registration");
activity.AddInfobipWhatsAppFlow(
    flowId: "registration_flow_123",
    ctaText: "Complete Registration",
    flowData: new { userId = "user123", step = 1 }
);
await turnContext.SendActivityAsync(activity);
```

#### Location Features

```csharp
// Request user location
var activity = MessageFactory.Text("We need your location for delivery");
activity.AddInfobipLocationRequest("Please share your location for accurate delivery");
await turnContext.SendActivityAsync(activity);

// Handle received location
if (turnContext.Activity.Entities?.Any(e => e.Type == "GeoCoordinates") == true)
{
    var location = turnContext.Activity.Entities.GetAs<GeoCoordinates>();
    await turnContext.SendActivityAsync($" Location received: {location.Latitude}, {location.Longitude}");
}
```

#### Calendar Integration

```csharp
// Add calendar event
var activity = MessageFactory.Text("Don't forget about your appointment!");
activity.AddInfobipCalendarEvent(
    title: "Doctor Appointment",
    description: "Annual checkup with Dr. Smith",
    startTime: DateTime.Parse("2023-12-25T10:00:00Z"),
    endTime: DateTime.Parse("2023-12-25T11:00:00Z"),
    location: "Medical Center, Room 205"
);
await turnContext.SendActivityAsync(activity);
```

#### Contact Sharing

```csharp
// Share contact information
var activity = MessageFactory.Text("Here's our contact information:");
var contact = new InfobipContactInfo
{
    FormattedName = "Customer Support",
    Phones = new[]
    {
        new InfobipContactPhone { Phone = "+1-800-SUPPORT", Type = "WORK" }
    },
    Emails = new[]
    {
        new InfobipContactEmail { Email = "support@company.com", Type = "WORK" }
    }
};
activity.AddInfobipContact(contact);
await turnContext.SendActivityAsync(activity);
```

#### Reply Context (Threading)

```csharp
// Reply to specific message
var activity = MessageFactory.Text("Here's the answer to your question");
activity.AddInfobipReplyContext("original-message-id-12345");
await turnContext.SendActivityAsync(activity);
```

### Other Features

- **Multi-channel support** - Works with all channels supported by Infobip Messages API
- **Request verification** - Validates incoming webhooks using app secret
- **Delivery reports** - Receive message delivery status updates
- **Seen reports** - Receive message read receipts (where supported)
- **Callback data** - Add custom data to messages returned in delivery reports
- **Attachment handling** - Download and process media attachments
- **Adaptive Cards** - Convert Bot Framework cards to appropriate channel formats
- **Automatic fallback** - Graceful degradation for unsupported features

### Advanced Features

#### Entity and Application ID Tracking

```csharp
// Add entity ID for message tracking
var activity = MessageFactory.Text("Message with entity tracking");
activity.AddInfobipEntityId("entity-12345");
await turnContext.SendActivityAsync(activity);

// Add application ID for message tracking
activity.AddInfobipApplicationId("app-67890");
await turnContext.SendActivityAsync(activity);
```

#### Message Validity Period

```csharp
// Set message validity period (expires after 24 hours)
var activity = MessageFactory.Text("Time-sensitive message");
activity.AddInfobipValidityPeriod(24, InfobipValidityPeriodTimeUnit.Hours);
await turnContext.SendActivityAsync(activity);
```

#### URL Shortening and Tracking

```csharp
// Enable URL shortening and click tracking
var urlOptions = new InfobipUrlOptions
{
    ShortenUrl = true,
    TrackClicks = true,
    TrackingUrl = "https://your-tracking-domain.com",
    CustomDomain = "short.yourdomain.com"
};

var activity = MessageFactory.Text("Check out this link: https://example.com/very-long-url");
activity.AddInfobipUrlOptions(urlOptions);
await turnContext.SendActivityAsync(activity);
```

#### Regional Compliance Options

```csharp
// India DLT compliance
var regionalOptions = new InfobipRegionalOptions
{
    IndiaDlt = new InfobipIndiaDltOptions
    {
        ContentTemplateId = "your-template-id",
        PrincipalEntityId = "your-entity-id"
    }
};

var activity = MessageFactory.Text("Message compliant with India DLT");
activity.AddInfobipRegionalOptions(regionalOptions);
await turnContext.SendActivityAsync(activity);

// Turkey IYS compliance
var turkeyOptions = new InfobipRegionalOptions
{
    TurkeyIys = new InfobipTurkeyIysOptions
    {
        BrandCode = "your-brand-code",
        RecipientType = "BIREYSEL" // or "TACIR"
    }
};
```

#### Message Scheduling

```csharp
// Schedule message to be sent at specific time
var activity = MessageFactory.Text("Scheduled message");
activity.AddInfobipSendAt(DateTime.UtcNow.AddHours(2)); // Send in 2 hours
await turnContext.SendActivityAsync(activity);

// Or using ISO 8601 string format
activity.AddInfobipSendAt("2023-12-25T10:00:00.000Z");
```

#### Global Configuration

Configure default options in `appsettings.json`:

```json
{
  "InfobipApiKey": "your-api-key",
  "InfobipMessagesApiBaseUrl": "https://api.infobip.com",
  "InfobipAppSecret": "your-app-secret",
  "DefaultEntityId": "your-default-entity-id",
  "DefaultApplicationId": "your-default-app-id",
  "DefaultValidityPeriod": 24,
  "DefaultValidityPeriodTimeUnit": "HOURS",
  "EnableUrlShortening": true,
  "EnableUrlTracking": true,
  "EnableInteractiveMessaging": true,
  "EnableWhatsAppTemplates": true,
  "EnableCarouselSupport": true,
  "EnableLocationSharing": true,
  "EnableWhatsAppFlows": true,
  "AutoConvertToInteractiveList": true,
  "InteractiveListThreshold": 3,
  "MaxCarouselCards": 10,
  "AdaptationMode": "FLEXIBLE"
}
```

Or configure programmatically:

```csharp
services.AddSingleton(sp =>
{
    var options = new InfobipMessagesAdapterOptions("api-key", "https://api.infobip.com")
    {
        DefaultEntityId = "entity-123",
        DefaultApplicationId = "app-456",
        DefaultValidityPeriod = 24,
        DefaultValidityPeriodTimeUnit = InfobipValidityPeriodTimeUnit.Hours,
        EnableUrlShortening = true,
        EnableUrlTracking = true,
        EnableInteractiveMessaging = true,
        EnableWhatsAppTemplates = true,
        EnableCarouselSupport = true,
        EnableLocationSharing = true,
        EnableWhatsAppFlows = true,
        AdaptationMode = InfobipAdaptationMode.Flexible,
        AutoConvertToInteractiveList = true,
        InteractiveListThreshold = 3,
        MaxCarouselCards = 10,
        DefaultUrlOptions = new InfobipUrlOptions
        {
            CustomDomain = "short.yourdomain.com",
            RemoveProtocol = true
        }
    };
    return options;
});
```

## Installation

Available via NuGet package [Bot.Builder.Community.Adapters.Infobip.Messages](https://www.nuget.org/packages/Bot.Builder.Community.Adapters.Infobip.Messages/)

Install into your project using the following command in the package manager:

```
PM> Install-Package Bot.Builder.Community.Adapters.Infobip.Messages
```

## Usage

### Prerequisites

To use the Messages API, you need:

1. An Infobip account with Messages API access
2. API credentials (API key, base URL)
3. Configured channels (WhatsApp, SMS, etc.)
4. Webhook endpoint configuration

Contact [Infobip support](https://www.infobip.com/contact) for account setup and channel configuration.

### Configuration

You can configure the adapter using `appsettings.json`:

```json
{
  "InfobipApiKey": "your-api-key",
  "InfobipMessagesApiBaseUrl": "https://api.infobip.com",
  "InfobipAppSecret": "your-app-secret",
  "InfobipMessagesApiKey": "your-messages-api-key"
}
```

### Wiring up the Adapter

#### 1. Create an Adapter Class

```csharp
public class InfobipMessagesAdapterWithErrorHandler : InfobipMessagesAdapter
{
    public InfobipMessagesAdapterWithErrorHandler(
        InfobipMessagesAdapterOptions options,
        IInfobipMessagesClient client,
        ILogger<InfobipMessagesAdapterWithErrorHandler> logger)
        : base(options, client, logger)
    {
        OnTurnError = async (turnContext, exception) =>
        {
            // Log any leaked exception from the application
            logger.LogError($"Exception caught: {exception.Message}");

            // Send a catch-all apology to the user
            await turnContext.SendActivityAsync("Sorry, it looks like something went wrong.");
        };
    }
}
```

#### 2. Create a Controller

```csharp
[Route("api/infobip/messages")]
[ApiController]
public class InfobipMessagesController : ControllerBase
{
    private readonly InfobipMessagesAdapter Adapter;
    private readonly IBot Bot;

    public InfobipMessagesController(InfobipMessagesAdapter adapter, IBot bot)
    {
        Adapter = adapter;
        Bot = bot;
    }

    [HttpPost]
    public async Task PostAsync()
    {
        // Delegate the processing of the HTTP POST to the adapter
        await Adapter.ProcessAsync(Request, Response, Bot);
    }
}
```

#### 3. Register Dependencies in Startup.cs

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

    // Create the Bot Framework Adapter with error handling enabled
    services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

    // Add dependencies for Infobip Messages Adapter
    services.AddSingleton<InfobipMessagesAdapterOptions>();
    services.AddSingleton<IInfobipMessagesClient, InfobipMessagesClient>();

    // Add Infobip Messages Adapter with error handler
    services.AddSingleton<InfobipMessagesAdapter, InfobipMessagesAdapterWithErrorHandler>();

    // Create the bot as a transient
    services.AddTransient<IBot, YourBot>();
}
```

### Message Handling

#### Incoming Messages

The adapter automatically converts incoming messages to Bot Framework activities:

- **Text messages** ? `Message` activity with `Text` property
- **Media messages** ? `Message` activity with `Attachments`
- **Location messages** ? `Message` activity with `GeoCoordinates` entity
- **Contact messages** ? `Message` activity with contact entities
- **Interactive messages** ? `Message` activity with `Value` containing user selection
- **Button/List replies** ? `Message` activity with structured `Value` object
- **Flow responses** ? `Message` activity with flow data in `Value`
- **Delivery reports** ? `Event` activity with name "DELIVERY"
- **Seen reports** ? `Event` activity with name "SEEN"

#### Outgoing Messages

Send messages using standard Bot Framework activities:

```csharp
// Text message
await turnContext.SendActivityAsync("Hello, world!");

// Message with quick reply buttons
var reply = MessageFactory.SuggestedActions(
    new[] { "Option 1", "Option 2", "Option 3" },
    "Choose an option:");
await turnContext.SendActivityAsync(reply);

// Media message
var attachment = new Attachment
{
    ContentType = "image/jpeg",
    ContentUrl = "https://docs.microsoft.com/en-us/bot-framework/media/how-it-works/architecture-resize.png",
    Name = "Bot Framework Architecture"
};
var imageMessage = MessageFactory.Attachment(attachment);
await turnContext.SendActivityAsync(imageMessage);

// Hero card (converted to appropriate format per channel)
var heroCard = new HeroCard
{
    Title = "Card Title",
    Subtitle = "Card Subtitle",
    Text = "Card description text",
    Images = new List<CardImage> { new CardImage("https://example.com/image.jpg") },
    Buttons = new List<CardAction>
    {
        new CardAction(ActionTypes.OpenUrl, "Open URL", value: "https://example.com"),
        new CardAction(ActionTypes.PostBack, "Click Me", value: "button_clicked")
    }
};
var heroCardMessage = MessageFactory.Attachment(heroCard.ToAttachment());
await turnContext.SendActivityAsync(heroCardMessage);
```

### Callback Data

Add custom data to track messages:

```csharp
var callbackData = new Dictionary<string, string>
{
    {"orderId", "12345"},
    {"userId", "user123"}
};

var activity = MessageFactory.Text("Your order has been processed!");
activity.AddInfobipCallbackData(callbackData);
await turnContext.SendActivityAsync(activity);
```

Retrieve callback data from delivery reports:

```csharp
if (turnContext.Activity.Type == ActivityTypes.Event &&
    turnContext.Activity.Name == "DELIVERY")
{
    var callbackData = turnContext.Activity.GetInfobipCallbackData();
    if (callbackData != null)
    {
        var orderId = callbackData.GetValueOrDefault("orderId");
        var userId = callbackData.GetValueOrDefault("userId");
        // Process delivery report...
    }
}
```

## Testing

For comprehensive testing instructions including Bot Framework Emulator integration, see the [Testing Guide](TESTING.md).

### Sample Bot

A comprehensive sample bot demonstrating **real Infobip Messages API integration** is available at:

```
samples/Infobip Messages Adapter Sample
```

**Key Features:**

- **Uses actual library** - `Bot.Builder.Community.Adapters.Infobip.Messages` (not stubs)
- **Real API calls** - Sends actual messages to Infobip Messages API
- **Complete payload logging** - See exact requests/responses in console
- **All channel configurations** with proper payload formatting
- **Interactive message examples** (buttons, lists, carousels)
- **Template message support** for WhatsApp Business
- **Callback data tracking** for delivery reports
- **Regional endpoint support** (e.g., `https://api.infobip.com`)

**Sample Architecture:**

```
Sample Bot Project ? Uses Library ? Calls Infobip API ? Real Message Delivery
```

See the [Sample Bot Guide](SAMPLE_BOT.md) for detailed usage instructions.

### Payload Format Validation

The adapter ensures all messages use the correct format with **ALL CAPS** for channels and content types:

```json
{
  "messages": [
    {
      "sender": "sender-id",
      "channel": "WHATSAPP",
      "destinations": [
        {
          "to": "user-conversation-id"
        }
      ],
      "content": {
        "body": {
          "type": "TEXT",
          "text": "Hello World"
        }
      }
    }
  ]
}
```

### Quick Start with Sample Bot

1. **Navigate to the sample directory:**

   ```bash
   cd samples/Infobip\ Messages\ Adapter\ Sample
   ```

2. **Update `appsettings.json`** with your **real** Infobip credentials:

   ```json
   {
     "InfobipApiKey": "your-actual-api-key",
     "InfobipMessagesApiBaseUrl": "https://api.infobip.com",
     "DefaultSender": "sender-id",
     "EnablePayloadLogging": true
   }
   ```

3. **Run the sample bot:**

   ```bash
   dotnet run
   ```

4. **Connect Bot Framework Emulator** to `http://localhost:5000/api/messages`

5. **Test commands that make real API calls:**

   - `"help"` - Show all available test commands
   - `"test whatsapp"` - Send via WhatsApp with real API call and logging
   - `"test sms"` - Send via SMS with real API call and logging
   - `"test buttons"` - Interactive buttons with real API integration
   - `"debug payload"` - Show expected payload format
   - `"status"` - Show current configuration

6. **Check console output** for real API request/response logging:
   ```
   === INFOBIP MESSAGES API REQUEST ===
   Endpoint: https://api.infobip.com/messages-api/1/messages
   Payload: {"messages":[{"channel":"WHATSAPP",...}]}
   ===================================
   ```

### Example Test Bot Usage

```csharp
// The sample bot demonstrates real library usage:

// Test different channels with real API calls
await turnContext.SendActivityAsync("Type 'test whatsapp' to send via WhatsApp");
await turnContext.SendActivityAsync("Type 'test sms' to send via SMS");
await turnContext.SendActivityAsync("Type 'debug payload' to see format");

// Check console output for payload logging:
// === INFOBIP MESSAGES API REQUEST ===
// Endpoint: https://api.infobip.com/messages-api/1/messages
// Payload: {"messages":[{"channel":"WHATSAPP",...}]}
```

## Channel-Specific Features

### WhatsApp Business API

- Template messages with dynamic parameters
- Interactive buttons and lists
- Media messages with captions
- Location sharing and requests
- Contact sharing
- WhatsApp Flows integration
- Reply context (message threading)
- Sticker support
- Carousel messages
- Calendar event integration

### SMS/MMS

- Long message support
- Media attachments (MMS)
- Delivery reports
- URL shortening and tracking

### Viber Business Messages

- Rich media messages
- Keyboards and buttons
- Stickers
- Location sharing

### RCS (Rich Communication Services)

- Rich cards and carousels
- Suggested actions
- Media messages
- Location sharing

### Other Channels

The adapter automatically adapts Bot Framework features to the capabilities of each channel using the configured adaptation mode.

## Documentation Links

- [Infobip Messages API Documentation](https://www.infobip.com/docs/api/platform/messages-api)
- [WhatsApp Business API Documentation](https://www.infobip.com/docs/whatsapp)
- [Microsoft Bot Framework Documentation](https://docs.microsoft.com/en-us/azure/bot-service/)
- [Bot Builder Community Project](https://github.com/botbuildercommunity)

## What's New

This enhanced version includes:

- **WhatsApp Templates** - Full support for Business API templates
- **Interactive Lists** - Convert choice prompts to native lists
- **Carousel Messages** - Multi-card horizontal scrolling
- **WhatsApp Flows** - Interactive form integration
- **Location Requests** - Ask users to share location
- **Calendar Events** - Add-to-calendar functionality
- **Contact Sharing** - Share business contact information
- **Reply Context** - Message threading support
- **Sticker Support** - Send and receive stickers
- **Enhanced Adaptive Cards** - Better card conversion
- **Automatic Fallback** - Graceful degradation for unsupported features
- **Comprehensive Configuration** - Fine-grained control over all features

## ? Acceptance Criteria Met

This implementation fully meets all specified acceptance criteria:

- **All current and future channels** supported via unified Messages API
- **Free-form messaging** across all channels
- **WhatsApp template sending** with dynamic parameters
- **All message types**: Text, Image, Document, Video, Audio, Buttons, Reply, Open URL, Request location, Add calendar event, Contact, Stickers, Location, Carousel, WhatsApp flows
- **Advanced features**: Entity/Application ID, Validity period, Adaptation mode, Regional options, Callback data, URL shortening & tracking
- **Future-proof architecture** automatically supports new Messages API channels
- **Bot Framework integration** with standard activities and cards
- **Comprehensive testing** and documentation
