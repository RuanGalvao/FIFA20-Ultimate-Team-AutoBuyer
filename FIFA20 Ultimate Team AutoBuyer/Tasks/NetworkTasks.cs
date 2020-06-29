using FIFA20_Ultimate_Team_AutoBuyer.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_AutoBuyer.Tasks
{
    public class NetworkTasks
    {
        private readonly viewModel ViewModel;
        private readonly HttpClient httpClient = new HttpClient();

        public NetworkTasks(viewModel viewModel)
        {
            ViewModel = viewModel;
        }

        private void AddHTTPClientHeaders(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-UT-SID", ViewModel.SessionID);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        }

        public async Task<NetworkResponse> Get(string path)
        {
            var request = CreateHttpRequestMessage(HttpMethod.Get, path, null);
            var response = await httpClient.SendAsync(request);
            ValidateResponse(response);
            return await GenerateNetworkResponseAsync(response);
        }

        public async Task<NetworkResponse> Post(string path, object content)
        {
            var asJson = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(asJson, Encoding.UTF8, "application/json");
            var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Post, path, stringContent);
            var response = await httpClient.SendAsync(httpRequestMessage);
            ValidateResponse(response);
            return await GenerateNetworkResponseAsync(response);
        }

        private async Task<NetworkResponse> GenerateNetworkResponseAsync(HttpResponseMessage httpResponseMessage)
        {
            return new NetworkResponse
            {
                ResponseString = await httpResponseMessage.Content.ReadAsStringAsync(),
                StatusCode = (int)httpResponseMessage.StatusCode
            };
        }

        public async Task Delete(string path)
        {
            var request = CreateHttpRequestMessage(HttpMethod.Delete, path, null);
            var response = await httpClient.SendAsync(request);
            ValidateResponse(response);
        }

        private HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string path, StringContent content)
        {
            var httpRequestMessage = new HttpRequestMessage(method, path);
            if (content != null) httpRequestMessage.Content = content;
            AddHTTPClientHeaders(httpClient);
            return httpRequestMessage;
        }

        public async Task<NetworkResponse> Put(string path, object content)
        {
            var serialisedContent = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(serialisedContent, Encoding.UTF8, "application/json");
            var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Put, path, stringContent);
            var httpClientResponse = await httpClient.SendAsync(httpRequestMessage);
            ValidateResponse(httpClientResponse);
            return await GenerateNetworkResponseAsync(httpClientResponse);
        }

        private void ValidateResponse(HttpResponseMessage response)
        {
            Enum.TryParse(response.StatusCode.ToString(), out Enums.FIFAUltimateTeamStatusCode value);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new HandledException("Session Reset");

            if (value == Enums.FIFAUltimateTeamStatusCode.RetryAfter)
                throw new HandledException("Retry After Error");

            if (value == Enums.FIFAUltimateTeamStatusCode.Unauthorized) 
                throw new HandledException("Invalid Session ID. Please re-enter a Session ID", true, 0, true);

            if (value == Enums.FIFAUltimateTeamStatusCode.CaptureRequired) 
                throw new HandledException("Please complete the CAPTCHA on the Web App and re-enter the Session ID", true, 0, true);

            if (value == Enums.FIFAUltimateTeamStatusCode.InsufficientFunds)
                throw new HandledException("Insufficient Funds - Sleeping for 5 mins", true, 5);

            if (value == Enums.FIFAUltimateTeamStatusCode.TooManyRequests)
                throw new HandledException("Too many requests have been made", true, 0, true);

            if (value == Enums.FIFAUltimateTeamStatusCode.UpgradeRequired)
                throw new HandledException("Upgrade Required");
        }

    }

}
