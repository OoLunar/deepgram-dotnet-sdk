﻿using Deepgram.Records;
using Deepgram.Records.OnPrem;

namespace Deepgram;

/// <summary>
/// Constructor for the client that communicates with the on premise Deepgram Server
/// </summary>
/// <param name="httpClient"><see cref="HttpClient"/> for making Rest calls</param>
/// <param name="deepgramClientOptions"><see cref="DeepgramClientOptions"/> for HttpClient Configuration</param>
public class OnPremClient(DeepgramClientOptions deepgramClientOptions, HttpClient httpClient)
    : AbstractRestClient(deepgramClientOptions, httpClient)
{

    /// <summary>
    /// Constructor with default Options
    /// </summary>
    /// <param name="apiKey">The key to authenticate with Deepgram</param>
    /// <param name="httpClient"><see cref="HttpClient"/> for making Rest calls</param>
    public OnPremClient(string apiKey, HttpClient httpClient) : this(new DeepgramClientOptions(apiKey), httpClient) { }


    internal readonly string UrlPrefix = $"/{Defaults.API_VERSION}/{UriSegments.PROJECTS}";
    /// <summary>
    /// get a list of credentials associated with project
    /// </summary>
    /// <param name="projectId">Id of project</param>
    /// <returns><see cref="ListOnPremCredentialsResponse"/></returns>
    public async Task<ListOnPremCredentialsResponse> ListCredentials(string projectId) =>
        await GetAsync<ListOnPremCredentialsResponse>($"{UrlPrefix}/{projectId}/{UriSegments.ONPREM}");

    /// <summary>
    /// Get credentials for the project that is associated with credential ID
    /// </summary>
    /// <param name="projectId">Id of project</param>
    /// <param name="credentialsId">Id of credentials</param>
    /// <returns><see cref="OnPremCredentialsResponse"/></returns>
    public async Task<OnPremCredentialsResponse> GetCredentials(string projectId, string credentialsId) =>
        await GetAsync<OnPremCredentialsResponse>($"{UrlPrefix}/{projectId}/{UriSegments.ONPREM}/{credentialsId}");

    /// <summary>
    /// Remove credentials in the project associated with the credentials ID
    /// </summary>
    /// <param name="projectId">Id of project</param>
    /// <param name="credentialsId">Id of credentials</param>
    /// <returns><see cref="MessageResponse"/></returns>
    public async Task<MessageResponse> DeleteCredentials(string projectId, string credentialsId) =>
        await DeleteAsync<MessageResponse>($"{UrlPrefix}/{projectId}/{UriSegments.ONPREM}/{credentialsId}");

    /// <summary>
    /// Create credentials for the associated projects
    /// </summary>
    /// <param name="projectId">Id of project</param>
    /// <param name="createOnPremCredentialsSchema"><see cref="CreateOnPremCredentialsSchema"/> for credentials to be created</param>
    /// <returns><see cref="OnPremCredentialsResponse"/></returns>
    public async Task<OnPremCredentialsResponse> CreateCredentials(string projectId, CreateOnPremCredentialsSchema createOnPremCredentialsSchema) =>
        await PostAsync<OnPremCredentialsResponse>(
            $"{UrlPrefix}/{projectId}/{UriSegments.ONPREM}",
            RequestContentUtil.CreatePayload(createOnPremCredentialsSchema));

}
