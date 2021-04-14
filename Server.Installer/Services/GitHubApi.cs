using Remotely.Shared.Utilities;
using Server.Installer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server.Installer.Services
{
    public interface IGitHubApi : IDisposable
    {
        Task<bool> DownloadArtifact(CliParams cliParams, string artifactDownloadUrl, string downloadToPath);

        Task<Artifact> GetLatestBuildArtifact(CliParams cliParams);

        Task<bool> TriggerDispatch(CliParams cliParams);
    }
    public class GitHubApi : IGitHubApi
    {
        private readonly string _apiHost = "https://api.github.com";
        private readonly HttpClient _httpClient;
        public GitHubApi()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Remotely Server Installer");
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<bool> DownloadArtifact(CliParams cliParams, string artifactDownloadUrl, string downloadToPath)
        {
            try
            {
                ConsoleHelper.WriteLine("Downloading build artifact.");

                var message = GetHttpRequestMessage(HttpMethod.Get, artifactDownloadUrl, cliParams);

                var response = await _httpClient.SendAsync(message);

                ConsoleHelper.WriteLine($"Download artifact response status code: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    ConsoleHelper.WriteError("GitHub API call to download build artifact failed.  Please check your input parameters.");
                    Environment.Exit(1);
                }

                using var responseStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(downloadToPath, FileMode.Create);

                await responseStream.CopyToAsync(fileStream);
                return true;
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Error while downloading artifact.  Message: {ex.Message}");
                return false;
            }
        }

        public async Task<Artifact> GetLatestBuildArtifact(CliParams cliParams)
        {
            try
            {
                var message = GetHttpRequestMessage(HttpMethod.Get, 
                    $"{_apiHost}/repos/{cliParams.GitHubUsername}/Remotely/actions/artifacts",
                    cliParams);

                var response = await _httpClient.SendAsync(message);

                ConsoleHelper.WriteLine($"Get artifacts response status code: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    ConsoleHelper.WriteError("GitHub API call to get build artifacts failed.  Please check your input parameters.");
                    Environment.Exit(1);
                }

                var payload = await response.Content.ReadFromJsonAsync<ArtifactsResponsePayload>();
                if (payload?.artifacts?.Any() != true)
                {
                    return null;
                }

                return payload.artifacts.OrderByDescending(x => x.created_at).First();
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Error while trying to retrieve build artifacts." +
                    $"Error: {ex.Message}");
                Environment.Exit(1);
            }

            return null;
        }

        public async Task<bool> TriggerDispatch(CliParams cliParams)
        {
            try
            {
                ConsoleHelper.WriteLine("Trigger GitHub Actions build.");


                var message = GetHttpRequestMessage(
                    HttpMethod.Post, 
                    $"{_apiHost}/repos/{cliParams.GitHubUsername}/Remotely/actions/workflows/build.yml/dispatches",
                    cliParams);

                var rid = EnvironmentHelper.IsLinux ?
                    "linux-x64" :
                    "win-x64";

                var body = new
                {
                    @ref = cliParams.Reference,
                    inputs = new
                    {
                        serverUrl = cliParams.ServerUrl.ToString(),
                        rid = rid
                    }
                };
                message.Content = new StringContent(JsonSerializer.Serialize(body));

                var response = await _httpClient.SendAsync(message);

                ConsoleHelper.WriteLine($"Dispatch response status code: {response.StatusCode}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Error: {ex.Message}");
            }
            return false;
        }

        private HttpRequestMessage GetHttpRequestMessage(HttpMethod method, string url, CliParams cliParams)
        {
            var message = new HttpRequestMessage(method, url);

            var base64Auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{cliParams.GitHubUsername}:{cliParams.GitHubPat}"));
            message.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

            return message;
        }
    }
}
