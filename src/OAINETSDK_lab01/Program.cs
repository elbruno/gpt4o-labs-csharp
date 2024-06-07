using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var openAIKey = config["APIKEY"];
var model = "gpt-4o";

// chat client
ChatClient client = new(model: model, openAIKey);

// define system message
var systemMessage = new SystemChatMessage("You are a useful assitant that replies using a funny style.");
var userQ = new UserChatMessage("What is the capital of France?");
var messages = new List<ChatMessage>
{
    systemMessage,
    userQ
};

// run the chat
ChatCompletion chatCompletion = client.CompleteChat(messages);
var response = chatCompletion.Content[^1].Text;

// show the original question and the chat response in the console
Console.WriteLine($@"System Prompt: {systemMessage.Content[^1].Text}

User Question: {userQ.Content[^1].Text}

Response: {response}");