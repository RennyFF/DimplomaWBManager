using MYWFE.MVVM.Model.ApiRequests.ResponseTypes;
using MYWFE.Utils.Services;
using MYWFE.Utils.Types;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

namespace MYWFE.MVVM.Model.ApiRequests
{
    interface IFeedbackRequestsAPI
    {
        Task<UnansweredCountityResponse> GetUnansweredReviewCountity(string token);
        Task<UnansweredCountityResponse> GetUnansweredQuestionsCountity(string token);
        Task<QuestionListResponse> GetUnansweredQuestionsList(string token, int Page, int CountityOnPage);
        Task<ReviewListResponse> GetUnansweredReviewsList(string token, int Page, int CountityOnPage);

        Task<bool> AnswerReview(string token, string id, string text);
        Task<bool> AnswerQuestion(string token, string id, string text, bool IsRejected);
        Task<bool> AutoReviewAnswers(string token);
    }
    public class FeedbackRequestsAPI : IFeedbackRequestsAPI
    {
        #region PassedValues
        private IAnswerService _answerSerivce;
        public IAnswerService AnswerSerivce
        {
            get { return _answerSerivce; }
            set { _answerSerivce = value; }
        }
        #endregion
        #region Values
        private readonly string _unansweredReviewsCountityUrl = "https://feedbacks-api.wildberries.ru/api/v1/feedbacks/count-unanswered";
        private readonly string _unansweredQuestionsCountityUrl = "https://feedbacks-api.wildberries.ru/api/v1/questions/count-unanswered";
        private readonly string _unansweredQuestionsListUrl = "https://feedbacks-api.wildberries.ru/api/v1/questions";
        private readonly string _unansweredReviewsListUrl = "https://feedbacks-api.wildberries.ru/api/v1/feedbacks";

        private readonly string _answerQuestionUrl = "https://feedbacks-api.wildberries.ru/api/v1/questions";
        private readonly string _answerReviewUrl = "https://feedbacks-api.wildberries.ru/api/v1/feedbacks/answer";
        #endregion
        #region Methods
        public async Task<UnansweredCountityResponse> GetUnansweredReviewCountity(string token)
        {
            await Task.Delay(1001);
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var response = await client.GetAsync(_unansweredReviewsCountityUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadFromJsonAsync<UnansweredCountityResponse>().Result;
                    }
                    return new UnansweredCountityResponse() { data = { countUnanswered = 0, countUnansweredToday = 0 } };
                }
                catch (Exception ex)
                {
                    return new UnansweredCountityResponse() { data = { countUnanswered = 0, countUnansweredToday = 0 } };
                }
            }
        }
        public async Task<UnansweredCountityResponse> GetUnansweredQuestionsCountity(string token)
        {
            await Task.Delay(1001);
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var response = await client.GetAsync(_unansweredQuestionsCountityUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadFromJsonAsync<UnansweredCountityResponse>().Result;
                    }
                    return new UnansweredCountityResponse() { data = { countUnanswered = 0, countUnansweredToday = 0 } };
                }
                catch (Exception ex)
                {
                    return new UnansweredCountityResponse() { data = { countUnanswered = 0, countUnansweredToday = 0 } };
                }
            }
        }
        public async Task<QuestionListResponse> GetUnansweredQuestionsList(string token, int Page, int CountityOnPage)
        {
            await Task.Delay(1001);
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var response = await client.GetAsync($"{_unansweredQuestionsListUrl}?isAnswered=false&order=dateDesc&take={CountityOnPage}&skip={(Page - 1) * CountityOnPage}");
                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadFromJsonAsync<QuestionListResponse>().Result;
                    }
                    return new();
                }
                catch (Exception ex)
                {
                    return new();
                }
            }
        }
        public async Task<ReviewListResponse> GetUnansweredReviewsList(string token, int Page, int CountityOnPage)
        {
            await Task.Delay(1001);
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var response = await client.GetAsync($"{_unansweredReviewsListUrl}?isAnswered=false&order=dateDesc&take={CountityOnPage}&skip={(Page - 1) * CountityOnPage}");
                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadFromJsonAsync<ReviewListResponse>().Result;
                    }
                    return new();
                }
                catch (Exception ex)
                {
                    return new();
                }
            }
        }
        public async Task<bool> AnswerReview(string token, string id, string text)
        {
            await Task.Delay(1001);
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    StringContent DataBody = new("{" +
                                                        $"\"id\": \"{id}\"," +
                                                        $"\"text\": \"{text}\"" +
                                                 "}",
                    Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var response = await client.PostAsync($"{_answerReviewUrl}", DataBody);
                    return response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        public async Task<bool> AnswerQuestion(string token, string id, string text, bool IsRejected)
        {
            await Task.Delay(1001);
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string state = IsRejected ? "none" : "wbRu";
                    StringContent DataBody = new("{" +
                        $"\"id\": \"{id}\"," +
                        "\"answer\": " +
                            "{" +
                            $"\"text\": \"{text}\"" +
                            "}," +
                        $"\"state\": \"{state}\"" +
                        "}",
                    Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var response = await client.PatchAsync($"{_answerQuestionUrl}", DataBody);
                    return response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        public async Task<bool> AutoReviewAnswers(string token)
        {
            await Task.Delay(1001);
            List<Answer> Answers = await RemoveUnusedAnswers(AnswerSerivce.Answers);
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var AllUnansweredReviews = await GetUnansweredReviewsList(token, 1, 4999);
                    foreach (var item in AllUnansweredReviews.data.feedbacks)
                    {
                        await Task.Delay(1001);
                        var AnswerText = await FindBestAnswer(Answers, item.productValuation);
                        if (AnswerText == null)
                        {
                            continue;
                        }
                        else { 
                            StringContent DataBody = new("{" +
                                                            $"\"id\": \"{item.id}\"," +
                                                            $"\"text\": \"{AnswerText}\"" +
                                                        "}",
                            Encoding.UTF8, "application/json");
                            await client.PatchAsync($"{_answerReviewUrl}", DataBody);
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        private async Task<List<Answer>> RemoveUnusedAnswers(ObservableCollection<Answer> originAnswer)
        {
            return new List<Answer>(originAnswer.Where(i => i.IsUsing == true));
        }

        private async Task<Answer?> FindBestAnswer(List<Answer> answersList, int productVaulation)
        {
            Answer? Result = answersList
                .OrderBy(i => i.Priority)
                .FirstOrDefault(i => ParseTargetRating(i.TargetRating).Contains(productVaulation));
            return Result;
        }

        private HashSet<int> ParseTargetRating(string targetRating)
        {
            HashSet<int> ratings = new HashSet<int>();
            var parts = targetRating.Split(['#'], StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                if (part.Contains("-"))
                {
                    var rangeParts = part.Split('-');
                    if (rangeParts.Length == 2 && int.TryParse(rangeParts[0], out int start) && int.TryParse(rangeParts[1], out int end))
                    {
                        for (int i = start; i <= end; i++)
                        {
                            ratings.Add(i);
                        }
                    }
                }
                else
                {
                    if (int.TryParse(part, out int rating))
                    {
                        ratings.Add(rating);
                    }
                }
            }

            return ratings.OrderBy(i => i).ToHashSet();
        }
        #endregion
        public FeedbackRequestsAPI(IAnswerService answerService)
        {
            AnswerSerivce = answerService;
        }
    }
}
