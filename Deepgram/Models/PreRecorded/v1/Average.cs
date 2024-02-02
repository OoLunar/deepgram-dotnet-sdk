﻿namespace Deepgram.Models.PreRecorded.v1;

public class Average
{
    [JsonPropertyName("sentiment")]
    public string? Sentiment { get; set; }

    [JsonPropertyName("sentiment_score")]
    public double? SentimentScore { get; set; }
}