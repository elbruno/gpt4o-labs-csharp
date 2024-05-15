using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var apikey = config["APIKEY"];
var model = "gpt-4o";


#pragma warning disable SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0050, SKEXP0052
// Create a kernel with OpenAI audio to text service
var kernel = Kernel.CreateBuilder()
    .AddOpenAIAudioToText(
        modelId: model,
        apiKey: apikey)
    .Build();

var audioToTextService = kernel.GetRequiredService<IAudioToTextService>();

var audioLocation = @"media\gpt4otranslations.m4a";
var audioBytes = File.ReadAllBytes(audioLocation);

// Read audio content from a file
AudioContent audioContent = new(audioBytes);

// Set execution settings (optional)
OpenAIAudioToTextExecutionSettings executionSettings = new(audioLocation)
{
    Language = "en", // The language of the audio data as two-letter ISO-639-1 language code (e.g. 'en' or 'es').
    Prompt = "sample prompt", // An optional text to guide the model's style or continue a previous audio segment.
                              // The prompt should match the audio language.
    ResponseFormat = "json", // The format to return the transcribed text in.
                             // Supported formats are json, text, srt, verbose_json, or vtt. Default is 'json'.
    Temperature = 0.3f, // The randomness of the generated text.
                        // Select a value from 0.0 to 1.0. 0 is the default.
};

// Convert audio to text
var textContent = await audioToTextService.GetTextContentAsync(audioContent, executionSettings);




Console.WriteLine("Audio : " + textContent.Text);