namespace MYWFE.MVVM.Model.ApiRequests.ResponseTypes
{
    interface IReportResponse
    {
        string date { get; set; }
    }
    public class ReportResponse : IReportResponse
    {
        public string date { get; set; }
    }
}
