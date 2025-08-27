using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ViscoveryDemoPOS.Domain;
using System.Collections.Generic;

namespace ViscoveryDemoPOS.Services
{
    public class VisAgentApiClient
    {
        private readonly HttpClient _http;
        private readonly string _base = "http://127.0.0.1:1688";

        public VisAgentApiClient(HttpClient httpClient = null)
        {
            _http = httpClient ?? new HttpClient();
        }

        public async Task<bool> HealthAsync()
        {
            var url = _base + "/api/v2/health";
            var res = await _http.GetAsync(url).ConfigureAwait(false);
            return res.IsSuccessStatusCode;
        }

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
