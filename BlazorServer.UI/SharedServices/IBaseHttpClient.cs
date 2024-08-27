namespace BlazorServer.UI.SharedServices
{
    public interface IBaseHttpClient
    {
        Task<string> GetApiResponseAsync(string endpoint);
        Task<string> PostRequestAsync<T>(string endpoint, T model);        
    }
}
