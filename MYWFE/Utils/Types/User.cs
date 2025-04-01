namespace MYWFE.Utils.Types
{
    public interface IUser
    {
        string StatiscticsToken { get; set; }
        string FeedbackToken { get; set; }
    }
    public class User : IUser
    {
        public int? Id { get; set; }
        public string UserName { get; set; }
        public string TradeMark { get; set; }
        public string StatiscticsToken { get; set; }
        public string FeedbackToken { get; set; }
        public string? Preset { get; set; }
    }
}
