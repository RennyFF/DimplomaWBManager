namespace MYWFE.MVVM.Model.ApiRequests.ResponseTypes
{
    interface IAccountInfoResponse
    {
        string name { get; set; }
        string tradeMark { get; set; }
    }
    public class AccountInfoResponse : IAccountInfoResponse
    {
        public string name { get; set; }
        public string tradeMark { get; set; }
    }
}
