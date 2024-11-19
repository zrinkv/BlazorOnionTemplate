using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using BlazorServer.UI.Properties;
using Radzen;
using Newtonsoft.Json.Linq;
using System.Linq.Dynamic.Core;

namespace BlazorServer.UI.SharedServices
{
    public class BaseHttpClient : IBaseHttpClient
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly NavigationManager _navigationManager;
        protected readonly NotificationService _notificationService;
        public readonly HttpClient _httpClient;
        private readonly IStringLocalizer _localizer;

        public BaseHttpClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, NavigationManager navigationManager, NotificationService notificationService, IStringLocalizer<Resources> localizer)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient("MKVodovodAPI");
            _navigationManager = navigationManager;
            _notificationService = notificationService;
            _localizer = localizer;
        }

        public string GetToken()
        {
            if (_httpContextAccessor.HttpContext?.User.Claims.Any() ?? false)
                return _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == "jwtToken").ToString().Split(":")[1].Trim();
            else
                return String.Empty;
        }

        public async Task<string> GetApiResponseAsync(string endpoint)
        {
            if (_httpClient.DefaultRequestHeaders.Authorization == null)
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GetToken());

            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + endpoint);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ShowNotification(NotificationSeverity.Warning, _localizer["error401"]);

                _navigationManager.NavigateTo("RedirectToLogin/false"); //or RedirectToLogin/true

                return "You are not logged in/Cookie has expired";
            }
            else
            {
                var res = response.Content.ReadAsStringAsync();
                return "Error occurred."; //Handle error
            }
        }

        public async Task<string> PostRequestAsync<T>(string endpoint, T model)
        {
            if (_httpClient.DefaultRequestHeaders.Authorization == null)
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GetToken());

            var response = await _httpClient.PostAsJsonAsync(_httpClient.BaseAddress + endpoint, model);
            if (response.IsSuccessStatusCode)
            {
                var resultMessage = await response.Content.ReadAsStringAsync();
                ShowNotification(NotificationSeverity.Success, resultMessage);

                return resultMessage;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var detailsErrors = await response.Content.ReadAsStringAsync();
                
                try
                {
                    JToken token = JObject.Parse(detailsErrors)["errors"];
                    foreach (var item in token.Children())
                    {
                        ShowNotification(NotificationSeverity.Warning, item.Values().FirstOrDefault().ToString());
                    }
                }
                catch
                {
                    ShowNotification(NotificationSeverity.Warning, detailsErrors); //_localizer["error400"]                    
                }
            }
            else
            {
                ShowNotification(NotificationSeverity.Error, response.ReasonPhrase);
            }

            return response.StatusCode.ToString();
        }

        private void ShowNotification(NotificationSeverity notificationSeverity, string? summaryMessage)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = notificationSeverity,
                Duration = 10000,
                Summary = _localizer[summaryMessage]
            });
        }
    }
}
