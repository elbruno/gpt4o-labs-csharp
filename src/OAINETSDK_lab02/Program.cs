using Microsoft.Extensions.Configuration;
using OpenAI.Audio;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var openAIKey = config["APIKEY"];
var model = "whisper-1";

// audio client
AudioClient client = new(model: model, openAIKey);

string audioFilePath = Path.Combine("audio", "NTNIntro.mp4");

AudioTranscriptionOptions options = new()
{
    ResponseFormat = AudioTranscriptionFormat.Verbose,
    Granularities = AudioTimestampGranularities.Word | AudioTimestampGranularities.Segment,
};

AudioTranscription transcription = client.TranscribeAudio(audioFilePath, options);

Console.WriteLine("Transcription:");
Console.WriteLine($"{transcription.Text}");

Console.WriteLine();
Console.WriteLine($"Words:");
foreach (TranscribedWord word in transcription.Words)
{
    Console.WriteLine($"  {word.Word,15} : {word.Start.TotalMilliseconds,5:0} - {word.End.TotalMilliseconds,5:0}");
}

Console.WriteLine();
Console.WriteLine($"Segments:");
foreach (TranscribedSegment segment in transcription.Segments)
{
    Console.WriteLine($"  {segment.Text,90} : {segment.Start.TotalMilliseconds,5:0} - {segment.End.TotalMilliseconds,5:0}");
}