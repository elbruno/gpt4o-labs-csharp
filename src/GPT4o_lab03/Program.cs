using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var deploymentName = config["AZURE_OPENAI_MODEL"];
var endpoint = config["AZURE_OPENAI_ENDPOINT"];
var apiKey = config["AZURE_OPENAI_APIKEY"];

#pragma warning disable SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0050, SKEXP0052
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);
var kernel = builder.Build();

var chat = kernel.GetRequiredService<IChatCompletionService>();
var history = new ChatHistory();
history.AddSystemMessage("You are a useful assistant that replies using a direct style");

var imageContent = new ImageContent();

// use local image
//var imageFile = "petsmusic.png";
//var imageFullPath = Path.Combine(Directory.GetCurrentDirectory(), "imgs", imageFile);
//imageContent.MimeType = "image/png";
//imageContent.Data = File.ReadAllBytes(imageFullPath);

// use remote image
imageContent.Uri = new Uri("https://github.com/elbruno/gpt4ol-sk-csharp/blob/main/src/GPT4o_lab03/imgs/foggyday.png?raw=true");

var collectionItems = new ChatMessageContentItemCollection
{
    new TextContent("What's in the image?"),
    imageContent
};


history.AddUserMessage(collectionItems);

var result = await chat.GetChatMessageContentsAsync(history);
Console.WriteLine("Image description: " + result[^1].Content);