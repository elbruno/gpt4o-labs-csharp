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
var imageFile = "foggyday.png";
var imageFullPath = Path.Combine(Directory.GetCurrentDirectory(), "imgs", imageFile);
var imgInBlobUri = await UploadFileToAzureBlob(azureBlobUri, containerName, imageFile, imageFullPath);

// create chat collection items
var collectionItems = new ChatMessageContentItemCollection
{
    new TextContent("What's in the image?"),
    new ImageContent(new Uri(imgInBlobUri))
};
history.AddUserMessage(collectionItems);

// get the image description
var result = await chat.GetChatMessageContentsAsync(history);
Console.WriteLine("Image description: " + result[^1].Content);

async Task<string> UploadFileToAzureBlob(string azureBlobUri, string containerName, string imageFileName, string imageFullPath)
{
    var blobServiceClient = new BlobServiceClient(new Uri(azureBlobUri), new DefaultAzureCredential());
    var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
    var blobClient = containerClient.GetBlobClient(imageFileName);
    Console.WriteLine($"Uploading to Blob storage as blob:\n\t {blobClient.Uri}\n");
    await blobClient.UploadAsync(imageFullPath, true);
    Console.WriteLine($"Uploading complete ...\n");
    return blobClient.Uri.ToString();
}