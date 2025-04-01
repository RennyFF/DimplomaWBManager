using MYWFE.MVVM.Model.ApiRequests.ResponseTypes;

namespace MYWFE.Utils.Types
{
    public class Question : Core.ViewModel
    {
        public string id { get; set; }
        public string text { get; set; }
        public QuestionElementProductDetails productDetails { get; set; }
        private string _answerText = "";
        public string AnswerText
        {
            get { return _answerText; }
            set { _answerText = value; onPropertyChanged(nameof(AnswerText)); }
        }
    }
}
