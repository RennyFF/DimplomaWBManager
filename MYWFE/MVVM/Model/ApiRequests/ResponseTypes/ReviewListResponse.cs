namespace MYWFE.MVVM.Model.ApiRequests.ResponseTypes
{
    interface IReviewList
    {
        ReviewListData data { get; set; }
    }
    public class ReviewListResponse : IReviewList
    {
        public ReviewListData data { get; set; }
    }
    public class ReviewListData
    {
        public List<ReviewElement> feedbacks { get; set;}
    }
    public class ReviewElement
    {
        public string id { get; set; }
        public string userName { get; set; }
        public string text { get; set; }
        public string pros { get; set; }
        public string cons { get; set; }
        public int productValuation { get; set; }
        public ReviewElementProductDetails productDetails { get; set; }
        public List<ReviewElementPhotoLink>? photoLinks { get; set; }
    }
    public class ReviewElementProductDetails
    {
        public long nmId { get; set; }
        public string productName { get; set; }
        public string size { get; set; }
    }
    public class ReviewElementPhotoLink
    {
        public string fullSize { get; set; }
        public string miniSize { get; set; }
    }
}
