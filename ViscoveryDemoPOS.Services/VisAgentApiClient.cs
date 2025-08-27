using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ViscoveryDemoPOS.Domain;

namespace ViscoveryDemoPOS.Services
{
    /// <summary>
    /// Provides a typed wrapper for calling the VisAgent REST API.  The client
    /// is responsible for sending configuration data, triggering recognition
    /// requests and performing simple health checks against the service.
    /// </summary>
    public class VisAgentApiClient
    {
        /// <summary>
        /// Underlying HTTP client used to communicate with the VisAgent server.
        /// </summary>
        private readonly HttpClient _http;

        /// <summary>
        /// Base URL of the VisAgent service.  In the demo environment the agent
        /// listens on localhost port 1688.
        /// </summary>
        private readonly string _base = "http://127.0.0.1:1688";

        /// <summary>
        /// Initializes the client.  A custom <see cref="HttpClient"/> can be
        /// supplied which is useful for dependency injection or testing.
        /// </summary>
        /// <param name="httpClient">Optional HTTP client instance.</param>
        public VisAgentApiClient(HttpClient httpClient = null)
        {
            _http = httpClient ?? new HttpClient();
        }

        /// <summary>
        /// Checks whether the VisAgent service is alive by invoking its health
        /// endpoint.
        /// </summary>
        /// <returns>True when a successful response is returned; otherwise false.</returns>
        public async Task<bool> HealthAsync()
        {
            var url = _base + "/api/v2/health";
            var res = await _http.GetAsync(url).ConfigureAwait(false);
            return res.IsSuccessStatusCode;
        }

        /// <summary>
        /// Sends configuration options to the VisAgent service.  The configuration
        /// contains information such as POS callback URL and user interface
        /// preferences.
        /// </summary>
        /// <param name="posUrlOrigin">Origin URL of the POS system for callbacks.</param>
        public async Task ConfigureAsync(string posUrlOrigin = "http://127.0.0.1:8080")
        {
            var url = _base + "/api/v1/config";
            var payload = new
            {
                OutputModeInfo = new
                {
                    POSUrlOrigin = posUrlOrigin,
                    ExternalOutputDelay = 0.25,
                    CursorClickX = -1,
                    CursorClickY = -1
                },
                GuiInfo = new
                {
                    DisplayFrameType = "bbox",
                    Language = "zh-TW"
                }
            };
            var json = JsonConvert.SerializeObject(payload);
            var res = await _http.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            res.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Requests VisAgent to perform unified recognition on the current input.
        /// The request can optionally switch the server to VisAgent mode and ask
        /// for a subset of fields in the response payload.
        /// </summary>
        /// <param name="switchToVisAgent">True to switch to VisAgent mode.</param>
        /// <param name="responseFields">Optional list of fields to include in the response.</param>
        /// <returns>Deserialized unified recognition response.</returns>
        public async Task<UnifiedRecognitionResponse> UnifiedRecognitionAsync(bool switchToVisAgent = true, string[] responseFields = null)
        {
            var url = _base + "/api/v1/unified_recognition";
            var payload = new Dictionary<string, object>();
            payload["switch_to_visagent"] = switchToVisAgent;
            if (responseFields != null && responseFields.Length > 0)
                payload["response_fields"] = responseFields;

            var json = JsonConvert.SerializeObject(payload);
            var res = await _http.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            var body = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<UnifiedRecognitionResponse>(body);
        }

        /// <summary>
        /// Causes the VisAgent user interface to navigate to a different page.
        /// </summary>
        /// <param name="page">Page identifier defined by the service.</param>
        /// <param name="parameter">Optional parameter passed along with the request.</param>
        public async Task SwitchPageAsync(string page, object parameter = null)
        {
            var url = _base + "/api/v2/switch";
            var payload = new { page = page, parameter = parameter };
            var json = JsonConvert.SerializeObject(payload);
            var res = await _http.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            res.EnsureSuccessStatusCode();
        }
    }
}

