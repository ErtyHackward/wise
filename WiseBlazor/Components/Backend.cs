using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WiseDomain;

namespace WiseBlazor.Components
{
    public class Backend : ComponentBase
    {
        protected ILogger<Backend> Logger { get; set; }
        public LocalStorage LocalStorage { get; }
        public HttpClient HttpClient { get; }
        public NavigationManager NavigationManager { get; }

        public string LoginUri { get; set; } = "/login";

        public string ApiToken { get; set; }

        public Backend(LocalStorage localStorage, HttpClient httpClient, NavigationManager navigationManager, ILogger<Backend> logger)
        {
            LocalStorage = localStorage;
            HttpClient = httpClient;
            NavigationManager = navigationManager;
            Logger = logger;

            HttpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        public async Task<ServerResponse<T>> GetAsync<T>(string uri)
        {
            try
            {
                var result = await HttpClient.GetJsonAsync<T>(uri);
                return new ServerResponse<T>() { Response = result, Success = true };
            }
            catch (Exception x)
            {
                return new ServerResponse<T> { ErrorText = x.Message, ErrorCode = 1 };
            }
        }

        public async Task<ServerResponse<T>> PostAsync<T>(string uri, object content)
        {
            try
            {
                var result = await HttpClient.PostJsonAsync<T>(uri, content);
                return new ServerResponse<T>() { Response = result, Success = true };
            }
            catch (Exception x)
            {
                return new ServerResponse<T> { ErrorText = x.Message, ErrorCode = 1 };
            }
        }

        public static Uri Uri => new Uri("http://localhost:5000/");

        private Uri _apiBaseUri = new Uri(Uri, "api/");
        

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUri">Uri after */api/</param>
        /// <returns></returns>
        public async Task<ServerResponse<T>> GetApiAsync<T>(string relativeUri)
        {
            try
            {
                var result = await HttpClient.GetJsonAsync<T>(new Uri(_apiBaseUri, relativeUri).ToString());
                return new ServerResponse<T>() { Response = result, Success = true };
            }
            catch (Exception x)
            {
                return new ServerResponse<T> { ErrorText = x.Message, ErrorCode = 1 };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUri">Uri after */api/</param>
        /// <returns></returns>
        public async Task<ServerResponse<T>> PostApiAsync<T>(string relativeUri, object content)
        {
            try
            {
                var result = await HttpClient.PostJsonAsync<T>(new Uri(_apiBaseUri, relativeUri).ToString(), content);
                return new ServerResponse<T>() { Response = result, Success = true };
            }
            catch (Exception x)
            {
                return new ServerResponse<T> { ErrorText = x.Message, ErrorCode = 1 };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUri">Uri after */api/</param>
        /// <returns></returns>
        public async Task<ServerResponse> PutApiAsync(string relativeUri, object content)
        {
            try
            {
                await HttpClient.PutJsonAsync(new Uri(_apiBaseUri, relativeUri).ToString(), content);
                return new ServerResponse() { Success = true };
            }
            catch (Exception x)
            {
                return new ServerResponse { ErrorText = x.Message, ErrorCode = 1 };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUri">Uri after */api/</param>
        /// <returns></returns>
        public async Task<ServerResponse> DeleteApiAsync(string relativeUri)
        {
            try
            {
                var result = await HttpClient.DeleteAsync(new Uri(_apiBaseUri, relativeUri).ToString());
                return new ServerResponse() { Success = true };
            }
            catch (Exception x)
            {
                return new ServerResponse { ErrorText = x.Message, ErrorCode = 1 };
            }
        }

        public async Task<ServerResponse<T>> GetJsonAsync<T>(string uri)
        {
            try
            {
                ApiToken = await LocalStorage.GetItem<string>("api_token");
                HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("access-token", ApiToken);

                var result = await HttpClient.GetJsonAsync<ServerResponse<T>>(uri);

                if (!result.Success)
                {
                    return default;
                }

                return result;
            }
            catch (UnauthorizedAccessException)
            {
                NavigationManager.NavigateTo(LoginUri);
                return default;
            }
            catch (Exception e)
            {
                return default;
            }
        }

    }
}
