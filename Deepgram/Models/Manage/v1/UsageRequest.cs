﻿// Copyright 2021-2024 Deepgram .NET SDK contributors. All Rights Reserved.
// Use of this source code is governed by a MIT license that can be found in the LICENSE file.
// SPDX-License-Identifier: MIT

namespace Deepgram.Models.Manage.v1;

public record UsageRequest
{
    /// <summary>
    /// Identifier of request.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    /// <summary>
    /// UUID of the project.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("project_uuid")]
    public string? ProjectUUID { get; set; }

    /// <summary>
    /// Date/time when request was created.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("created")]
    public string? Created { get; set; }

    /// <summary>
    /// Path of endpoint to which request was submitted.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>
    /// Accessor of the request.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("accessor")]
    public string? Accessor { get; set; }

    /// <summary>
    /// Identifier of the API Key with which the request was submitted.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("api_key_id")]
    public string? ApiKeyId { get; set; }

    /// <summary>
    /// Response generated by the request <see cref="Response"/>.
    /// </summary>
    /// <remarks>If a response has not yet been generated, this object will be empty.</remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("response")]
    public Response? Response { get; set; }

    /// <summary>
    /// Only exists if a callback <see cref="Callback"/> was included in the request.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("callback")]
    public Callback? Callback { get; set; }

    /// <summary>
    /// Override ToString method to serialize the object
    /// </summary>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this, JsonSerializeOptions.DefaultOptions);
    }
}
