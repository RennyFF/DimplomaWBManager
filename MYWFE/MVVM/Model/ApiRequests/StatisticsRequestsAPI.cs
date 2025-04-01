using MYWFE.MVVM.Model.ApiRequests.ResponseTypes;
using System.Net.Http;
using System.Net.Http.Json;
using System.Xml;

namespace MYWFE.MVVM.Model.ApiRequests
{
    interface IStatisticsRequestsAPI
    {
        Task<Dictionary<int, int>?> GetOrdersByMonth(string token, string date);
        Task<Dictionary<int, int>?> GetSalesByMonth(string token, string date);
    }
    public class StatisticsRequestsAPI : IStatisticsRequestsAPI
    {
        #region Values
        private readonly string _reportOrderUrl = "https://statistics-api.wildberries.ru/api/v1/supplier/orders";
        private readonly string _reportSalesUrl = "https://statistics-api.wildberries.ru/api/v1/supplier/sales";
        #endregion
        #region Methods
        public async Task<Dictionary<int, int>> GetOrdersByMonth(string token, string date)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var response = await client.GetAsync($"{_reportOrderUrl}?dateFrom={date}");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseJson = response.Content.ReadFromJsonAsync<List<ReportResponse>>().Result;
                        return ReportToDictionary(responseJson, date);
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public async Task<Dictionary<int, int>> GetSalesByMonth(string token, string date)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var response = await client.GetAsync($"{_reportSalesUrl}?dateFrom={date}");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseJson = response.Content.ReadFromJsonAsync<List<ReportResponse>>().Result;
                        return ReportToDictionary(responseJson, date);
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        private Dictionary<int, int> ReportToDictionary(List<ReportResponse> reportElements, string date) {
            DateTime currentMonth = XmlConvert.ToDateTime(date, XmlDateTimeSerializationMode.Unspecified);

            Dictionary<int, int> GroupedReports = reportElements
                .Select(i => XmlConvert.ToDateTime(i.date, XmlDateTimeSerializationMode.Unspecified))
                .Where(j => j >= currentMonth)
                .GroupBy(k => k.Day)
                .ToDictionary(g => g.Key, g => g.Count());

            int LastExistingDate = GroupedReports.Keys.Max();

            return Enumerable.Range(1, LastExistingDate)
                .ToDictionary(day => day, day => GroupedReports.ContainsKey(day) ? GroupedReports[day] : 0);
        }
        #endregion
    }
}
