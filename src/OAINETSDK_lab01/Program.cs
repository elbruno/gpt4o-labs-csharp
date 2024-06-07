//    Copyright (c) 2024
//    Author      : Bruno Capuano
//    Change Log  :
//
//    The MIT License (MIT)
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in
//    all copies or substantial portions of the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//    THE SOFTWARE.

using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var openAIKey = config["APIKEY"];
var model = "gpt-4o";

// chat client
ChatClient client = new(model, openAIKey);

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