using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Newtonsoft.Json;
using WiseDomain;


namespace WiseBlazor.Components
{
    public class Backend : ComponentBase
    {
        public static Uri Uri { get; set; } = new Uri("http://wise.bolshoe.tv:6000/");

        private Uri _apiBaseUri = new Uri(Uri, "api/");

        protected ILogger<Backend> Logger { get; set; }
        public ISyncLocalStorageService LocalStorage { get; }
        public HttpClient HttpClient { get; }
        public NavigationManager NavigationManager { get; }

        public string LoginUri { get; set; } = "/login";
        
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpire { get; set; }
        public string RefreshToken { get; set; }
        public User CurrentUser { get; private set; }

        public bool IsAuthenticated => !string.IsNullOrEmpty(RefreshToken);

        public event EventHandler AuthenticationChanged;

        public event EventHandler UserUpdated;

        public Backend(ISyncLocalStorageService localStorage, HttpClient httpClient, NavigationManager navigationManager, ILogger<Backend> logger)
        {
            LocalStorage = localStorage;
            HttpClient = httpClient;
            NavigationManager = navigationManager;
            Logger = logger;

            HttpClient.Timeout = TimeSpan.FromSeconds(5);

            AccessToken = LocalStorage.GetItem<string>("access_token");
            RefreshToken = LocalStorage.GetItem<string>("refresh_token");
            AccessTokenExpire = LocalStorage.GetItem<DateTime>("access_token_expire");

            if (!string.IsNullOrEmpty(AccessToken))
                HttpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", AccessToken);

            Logger.LogInformation($"Backend instance created, IsAuth: {IsAuthenticated} Login: {CurrentUser?.Login}");

            GetUserAsync();
        }

        private async Task GetUserAsync()
        {
            var resp = await GetApiAsync<User>("users/me");
            CurrentUser = resp.Response;
            OnUserUpdated();
            OnAuthenticationChanged();
        }

        public async Task<bool> ValidateAccessToken()
        {
            Logger.LogInformation($"Validating access token. Now:{DateTime.Now} Expire:{AccessTokenExpire}");

            if (string.IsNullOrEmpty(AccessToken) || string.IsNullOrEmpty(RefreshToken))
                return false;

            if (DateTime.Now < AccessTokenExpire || await RefreshTokenAsync())
                return true;

            Logout();

            return false;
        }

        public void Logout()
        {
            Logger.LogInformation($"Logging out...");

            AccessToken = null;
            RefreshToken = null;
            LocalStorage.SetItem("access_token", null);
            LocalStorage.SetItem("refresh_token", null);

            OnAuthenticationChanged();
        }

        public async Task<bool> Authorize(string login, string password)
        {
            try
            {
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", "client"),
                    new KeyValuePair<string, string>("username", login),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("client_secret", "secret"),
                    new KeyValuePair<string, string>("scope", "wiseapi offline_access"),
                });
                
                var res = await HttpClient.PostAsync(new Uri(Uri, "connect/token"), formContent);
                var success = await HandleAuthResult(res);

                if (success)
                    await GetUserAsync();

                return success;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Authorization failed");
                throw;
            }
        }

        private async Task<bool> HandleAuthResult(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var authResp = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                    new {access_token = "", expires_in = 0, token_type = "", refresh_token = "", scope = ""});

                AccessToken = authResp.access_token;
                RefreshToken = authResp.refresh_token;
                AccessTokenExpire = DateTime.Now.AddSeconds(authResp.expires_in - 5);

                LocalStorage.SetItem("access_token", AccessToken);
                LocalStorage.SetItem("refresh_token", RefreshToken);
                LocalStorage.SetItem("access_token_expire", AccessTokenExpire);

                HttpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", AccessToken);

                OnAuthenticationChanged();

                return true;
            }

            return false;
        }

        public async Task<bool> RefreshTokenAsync()
        {
            Logger.LogInformation($"Refreshing access token...");
            try
            {
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", "client"),
                    new KeyValuePair<string, string>("refresh_token", RefreshToken),
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("client_secret", "secret"),
                    new KeyValuePair<string, string>("scope", "wiseapi offline_access"),
                });

                var res = await HttpClient.PostAsync(new Uri(Uri, "connect/token"), formContent);

                return await HandleAuthResult(res);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Unable to refresh token");
                return false;
            }
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
                Logger.LogInformation($"Trying Api get AccessToken: {AccessToken} Auth: {IsAuthenticated}");

                if (!await ValidateAccessToken())
                    return new ServerResponse<T> { ErrorText = "Not authenticated", ErrorCode = 2 };

                Logger.LogInformation($"Validated");

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
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<ServerResponse<T>> PostApiAsync<T>(string relativeUri, object content)
        {
            try
            {
                //if (!await ValidateAccessToken())
                //    return null;

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
                //if (!await ValidateAccessToken())
                //    return null;

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
                //if (!await ValidateAccessToken())
                //    return null;

                var result = await HttpClient.DeleteAsync(new Uri(_apiBaseUri, relativeUri).ToString());
                return new ServerResponse() { Success = true };
            }
            catch (Exception x)
            {
                return new ServerResponse { ErrorText = x.Message, ErrorCode = 1 };
            }
        }

        protected virtual void OnAuthenticationChanged()
        {
            AuthenticationChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnUserUpdated()
        {
            UserUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
