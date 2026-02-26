using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ToolTest
{
    public class WebRequestClient
    {
        private readonly HttpClient http = new HttpClient();

        private readonly string keyId;
        private readonly string secretKey;

        public WebRequestClient(string keyId, string secretKey)
        {
            this.keyId = keyId;
            this.secretKey = secretKey;
        }

        private string GetBasicAuthHeader()
        {
            string raw = $"{keyId}:{secretKey}";
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
            return $"Basic {base64}";
        }

        private HttpRequestMessage BuildRequest(HttpMethod method, string url, string bodyJson = null)
        {
            var req = new HttpRequestMessage(method, url);

            req.Headers.Add("Authorization", GetBasicAuthHeader());

            if (bodyJson != null)
                req.Content = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            return req;
        }


        public async Task<string> Get(string url, Dictionary<string, string> headers = null)
        {
            try
            {
                var req = BuildRequest(HttpMethod.Get, url);

                // Add custom headers if provided
                if (headers != null) 
                { 
                    foreach (var h in headers)
                    {
                        req.Headers.TryAddWithoutValidation(h.Key, h.Value); 
                    } 
                }

                var res = await http.SendAsync(req);
                var json = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                    throw new Exception($"GET {url} failed: {json}");

                return json;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[WebRequestClient][GET] {ex.Message}");
                throw;
            }
        }

        public async Task<string> Post(string url, string body, Dictionary<string, string> headers = null)
        {
            try
            {
                var req = BuildRequest(HttpMethod.Post, url, body);

                if (headers == null || !headers.ContainsKey("Content-Type"))
                    req.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                if (headers != null)
                {
                    foreach (var h in headers)
                        req.Headers.TryAddWithoutValidation(h.Key, h.Value);
                }

                var res = await http.SendAsync(req);
                var json = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                    throw new Exception($"POST {url} failed: {json}");

                return json;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[WebRequestClient][POST] {ex.Message}");
                throw;
            }
        }

        public async Task<bool> Delete(string url)
        {
            try
            {
                var req = BuildRequest(HttpMethod.Delete, url);
                var res = await http.SendAsync(req);
                var json = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                    throw new Exception($"DELETE {url} failed: {json}");

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[WebRequestClient][DELETE] {ex.Message}");
                throw;
            }
        }
    }
}
