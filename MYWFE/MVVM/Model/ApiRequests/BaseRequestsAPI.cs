using MYWFE.MVVM.Model.ApiRequests.ResponseTypes;
using System.Net.Http;
using System.Net.Http.Json;

namespace MYWFE.MVVM.Model.ApiRequests
{
    interface IBaseRequestsAPI
    {
        Task<bool> PingToken(string token, PingCategories category);
        Task<AccountInfoResponse> GetAccountInformation(string token);
    }
    public enum PingCategories
    {
        Content,
        Analytics,
        Statistics,
        Feedback
    }
    public class BaseRequestsAPI : IBaseRequestsAPI
    {
        #region Values
        private readonly string _pingContentUrl = "https://content-api.wildberries.ru/ping";
        private readonly string _pingAnalyticsUrl = "https://seller-analytics-api.wildberries.ru/ping";
        private readonly string _pingStatisticsUrl = "https://statistics-api.wildberries.ru/ping";
        private readonly string _pingFeedbackUrl = "https://feedbacks-api.wildberries.ru/ping";

        private readonly string _getAccountUrl = "https://common-api.wildberries.ru/api/v1/seller-info";
        #endregion
        #region Methods
        public async Task<bool> PingToken(string token, PingCategories category)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    HttpResponseMessage response = new();

                    switch (category)
                    {
                        case PingCategories.Content:
                            response = await client.GetAsync(_pingContentUrl);
                            break;
                        case PingCategories.Analytics:
                            response = await client.GetAsync(_pingAnalyticsUrl);
                            break;
                        case PingCategories.Statistics:
                            response = await client.GetAsync(_pingStatisticsUrl);
                            break;
                        case PingCategories.Feedback:
                            response = await client.GetAsync(_pingFeedbackUrl);
                            break;
                        default:
                            break;
                    }
                    return response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        public async Task<AccountInfoResponse> GetAccountInformation(string token)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var response = await client.GetAsync(_getAccountUrl);
                    return response.IsSuccessStatusCode ? response.Content.ReadFromJsonAsync<AccountInfoResponse>().Result : new();
                }
                catch (Exception ex)
                {
                    return new();
                }
            }
        }
        #endregion
    }
}
