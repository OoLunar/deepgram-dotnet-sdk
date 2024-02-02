﻿namespace Deepgram.Models.Live.v1;
public record TranscriptionResponse
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LiveType Type { get; set; } = LiveType.Results;

    [JsonPropertyName("channel_index")]
    public IReadOnlyList<int> ChannelIndex { get; set; }

    [JsonPropertyName("duration")]
    public double Duration { get; set; }

    [JsonPropertyName("start")]
    public double Start { get; set; }

    [JsonPropertyName("is_final")]
    public bool IsFinal { get; set; }

    [JsonPropertyName("speech_final")]
    public bool SpeechFinal { get; set; }

    [JsonPropertyName("channel")]
    public Channel Channel { get; set; }

    [JsonPropertyName("metadata")]
    public MetaData MetaData { get; set; }

    /// <summary>
    /// Returns a list text segments and the intents found within each segment.
    /// </summary>
    [JsonPropertyName("intents")]
    public Intents? Intents { get; set; }

    public Exception? Error { get; set; }
}