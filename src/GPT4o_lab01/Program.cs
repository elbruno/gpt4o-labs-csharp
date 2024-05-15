using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var apikey = config["APIKEY"];
var model = "gpt-4o";


#pragma warning disable SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0050, SKEXP0052
var builder = Kernel.CreateBuilder();
builder.AddOpenAIChatCompletion (model, apikey);
var kernel = builder.Build();

var chat = kernel.GetRequiredService<IChatCompletionService>();
var history = new ChatHistory();
history.AddSystemMessage("You are a useful assistant that replies using a direct style");

// analize image
var collectionItems= new ChatMessageContentItemCollection
{
    new TextContent("What's in the image?"),
    new ImageContent(new Uri("https://github.com/elbruno/gpt4ol-sk-csharp/blob/main/imgs/rpi5.png?raw=true"))
};
history.AddUserMessage(collectionItems);

var result = await chat.GetChatMessageContentsAsync(history);
Console.WriteLine("Image description: " + result[^1].Content);