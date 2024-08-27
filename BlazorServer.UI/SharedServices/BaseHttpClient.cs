using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using BlazorServer.UI.Properties;
using Radzen;

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
            if (_httpContextAccessor.HttpContext.User.Claims.Any())
                return _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == "jwtToken").ToString().Split(":")[1].Trim();            
            else
                return null;
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
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Duration = 10000,
                    Summary = _localizer["error401"]
                });

                _navigationManager.NavigateTo("login"); //or RedirectToLogin/true

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

            var result = await _httpClient.PostAsJsonAsync(_httpClient.BaseAddress + endpoint, model);
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync();            
            }

            return null;
        }
    }
}
