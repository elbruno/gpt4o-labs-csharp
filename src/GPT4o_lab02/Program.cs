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
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var apikey = config["APIKEY"];
var model = "whisper-1";


#pragma warning disable SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0050, SKEXP0052
// Create a kernel with OpenAI audio to text service
var kernel = Kernel.CreateBuilder()
    .AddOpenAIAudioToText(
        modelId: model,
        apiKey: apikey)
    .Build();

var audioToTextService = kernel.GetRequiredService<IAudioToTextService>();

// audio file data
var audioFileName = "NTNIntro.mp4";
var audioFullPath = Path.Combine(Directory.GetCurrentDirectory(), "media", audioFileName);
var audioBytes = File.ReadAllBytes(audioFullPath);

// Read audio content from a file
AudioContent audioContent = new(audioBytes);

// Set execution settings (optional)
OpenAIAudioToTextExecutionSettings executionSettings = new()
{
    Filename = audioFileName,
    Language = "es", // The language of the audio data as two-letter ISO-639-1 language code (e.g. 'en' or 'es').
    Prompt = "Extract text from audio", // An optional text to guide the model's style or continue a previous audio segment.
                              // The prompt should match the audio language.
    ResponseFormat = "json", // The format to return the transcribed text in.
                             // Supported formats are json, text, srt, verbose_json, or vtt. Default is 'json'.
    Temperature = 0.3f, // The randomness of the generated text.
                        // Select a value from 0.0 to 1.0. 0 is the default.
};

//// get text single call
var textContent = await audioToTextService.GetTextContentAsync(audioContent, executionSettings);
Console.WriteLine("Audio : " + textContent.Text);

// processing in List Mode
//var textContents = await audioToTextService.GetTextContentsAsync(audioContent, executionSettings);
//foreach (var text in textContents)
//{
//    Console.Write(text);
//}

