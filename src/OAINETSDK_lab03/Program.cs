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
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Chat;
using OpenAI.Files;
using System.ClientModel;

#pragma warning disable OPENAI001

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var openAIKey = config["APIKEY"];
var model = "gpt-4o";

OpenAIClient openAIClient = new(openAIKey);
FileClient fileClient = openAIClient.GetFileClient();
var assistantClient = openAIClient.GetAssistantClient();


var imageFile = "foggyday.png";
var imageFullPath = Path.Combine(Directory.GetCurrentDirectory(), "imgs", imageFile);
OpenAIFileInfo imgFile = fileClient.UploadFile(imageFullPath, FileUploadPurpose.Vision);

var imageMessage = MessageContent.FromImageFileId(imgFile.Id);

Assistant assistant = assistantClient.CreateAssistant(
    model: model,
    new AssistantCreationOptions()
    {
        Instructions = "You are a useful assistant that replies using a funny style."
    });

AssistantThread thread = assistantClient.CreateThread(new ThreadCreationOptions()
{
    InitialMessages =
    {
        new ThreadInitializationMessage(
        [
            "Hello, assistant! Please describe this image for me:",
            MessageContent.FromImageFileId(imgFile.Id)
        ]),
    }
});

var streamingUpdates = assistantClient.CreateRunStreaming(thread,assistant);

foreach (StreamingUpdate streamingUpdate in streamingUpdates)
{
    if (streamingUpdate.UpdateKind == StreamingUpdateReason.RunCreated)
    {
        Console.WriteLine($"--- Run started! ---");
    }
    if (streamingUpdate is MessageContentUpdate contentUpdate)
    {
        Console.Write(contentUpdate.Text);
    }
}