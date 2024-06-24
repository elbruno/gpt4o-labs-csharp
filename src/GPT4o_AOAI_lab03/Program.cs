using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var deploymentName = config["AZURE_OPENAI_MODEL"];
var endpoint = config["AZURE_OPENAI_ENDPOINT"];
var apiKey = config["AZURE_OPENAI_APIKEY"];
var azureBlobUri = config["AZURE_BLOB_URI"];
var containerName = config["AZURE_BLOB_URI_CONTAINERNAME"];

#pragma warning disable SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0050, SKEXP0052
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);
var kernel = builder.Build();

// init chat
var chat = kernel.GetRequiredService<IChatCompletionService>();
var history = new ChatHistory();
history.AddSystemMessage("You are a useful assistant that replies using a direct style");

// use local image
var audioFile = "NTNIntro.mp4";
var audioFullPath = Path.Combine(Directory.GetCurrentDirectory(), "audio", audioFile);
var audioBytes = File.ReadAllBytes(audioFullPath);

// create chat collection items
var collectionItems = new ChatMessageContentItemCollection
{
    new TextContent("What's in the image?"),
    new AudioContent(audioBytes, "audio/mp4")
};
history.AddUserMessage(collectionItems);

// get the audio transcript 
var result = await chat.GetChatMessageContentsAsync(history);
Console.WriteLine("Image description: " + result[^1].Content);
