﻿using BlogEngine.DataTransferObject;
using System.Text.Json;

namespace BlogEngine.Web.Helpers
{
    public interface IControllerHelpers
    {
        Task<HttpResponseMessage> GetAsync(string requestUri, string accessToken = null);
        Task<HttpResponseMessage> PostAsync(string requestUri, FormUrlEncodedContent formData);

        Task<PostViewModel> GetAPost(int id);
    }
    public class ControllerHelpers: IControllerHelpers
    {

        private readonly IConfiguration _configuration = null;
        private readonly IHttpClientFactory _httpClientFactory = null;
        
        public ControllerHelpers(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        private string GetBaseUri()
        {
            var endPoint = new MyEndpoint(_configuration);
            return endPoint.Api;
        }
        public async Task<HttpResponseMessage> PostAsync(string requestUri, FormUrlEncodedContent formData)
        {
            var baseUri = GetBaseUri();
            var method = HttpMethod.Post;
            var uri = $"{baseUri}/api/{requestUri}";
            //var request = new HttpRequestMessage(method, uri);

            //var buffer = Encoding.UTF8.GetBytes(content);
            //var byteContent = new ByteArrayContent(buffer);

            //byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //var data = new StringContent(content, Encoding.UTF8, "application/json");
            var client = _httpClientFactory.CreateClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Accept", "application/json");
            var response = await client.PostAsync(uri, formData);
            return response;
        }
        public async Task<HttpResponseMessage> GetAsync(string requestUri, string accessToken = null)
        {
            var baseUri = GetBaseUri();
            var method = HttpMethod.Get;
            var uri = $"{baseUri}/api/{requestUri}";
            var request = new HttpRequestMessage(method, uri);

            
            var client = _httpClientFactory.CreateClient();
            if (accessToken != null)
            {
                //client.SetBearerToken(accessToken);
            }
            var response = await client.SendAsync(request);
            return response;
        }

        public async Task<PostViewModel> GetAPost(int id)
        {
            var post = new PostViewModel();
            try
            {
                var response = await GetAsync($"Post/{id}");
                if (response.IsSuccessStatusCode)
                {
                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    post = await JsonSerializer.DeserializeAsync<PostViewModel>(responseStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return post;
        }
    }
}
