namespace MYWFE.MVVM.Model.ApiRequests.ResponseTypes
{
    interface IQuestionList
    {
        QuestionListData data { get; set; }
    }
    public class QuestionListResponse : IQuestionList
    {
        public QuestionListData data { get; set; }
    }
    public class QuestionListData
    {
        public List<QuestionElement> questions { get; set;}
    }
    public class QuestionElement
    {
        public string id { get; set; }
        public string text { get; set; }
        public QuestionElementProductDetails productDetails { get; set; }

    }
    public class QuestionElementProductDetails
    {
        public long nmId { get; set; }
        public string productName { get; set; }
    }
}
