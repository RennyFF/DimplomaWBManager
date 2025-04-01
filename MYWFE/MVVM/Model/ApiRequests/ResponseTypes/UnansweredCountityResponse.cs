namespace MYWFE.MVVM.Model.ApiRequests.ResponseTypes
{
    interface IUnansweredCountityResponse
    {
        UnansweredCountityStatistic data { get; set; }
    }
    public class UnansweredCountityResponse : IUnansweredCountityResponse
    {
        public UnansweredCountityStatistic data { get; set; }
    }
    public class UnansweredCountityStatistic
    {
        public int countUnanswered { get; set; }
        public int countUnansweredToday { get; set; }
    }
}
