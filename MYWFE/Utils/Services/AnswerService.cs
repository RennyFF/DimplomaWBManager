using MYWFE.Core;
using MYWFE.MVVM.Model.Database.Requests;
using MYWFE.Utils.Types;
using System.Collections.ObjectModel;

namespace MYWFE.Utils.Services
{
    public interface IAnswerService
    {
        ObservableCollection<Answer>? Answers { get; set; }
        void RemoveAnswer(int Id);
        void AddAnswer(Answer UnsettedAnswer);
        void UpdateAnswer(Answer oldAnswer);
        void UpdateAllAnswers();
    }
    class AnswerService : ObservableObject, IAnswerService
    {
        #region PassedValues
        private readonly IAnswerRequests AnswerContext;
        #endregion
        #region Values
        private ObservableCollection<Answer> _answers = new ObservableCollection<Answer>();

        public ObservableCollection<Answer> Answers
        {
            get { return _answers; }
            set { _answers = value; onPropertyChanged(nameof(Answers)); }
        }
        #endregion
        #region Methods
        private async void SetAnswers()
        {
            var AnswerSet = await AnswerContext.GetListAnswers();
            if (AnswerSet != null)
            {
                await Task.Run(() =>
                {
                    Answers = new ObservableCollection<Answer>(AnswerSet
                    .Select(i => new Answer()
                    {
                        Id = i.Id,
                        IsUsing = i.IsUsing,
                        Priority = i.Priority,
                        TargetRating = i.TargetRating,
                        Text = i.Text,
                        Title = i.Title,
                    }).ToList());
                });
            }
        }
        public async void RemoveAnswer(int id)
        {
            await AnswerContext.RemoveAnswer(id);
            SetAnswers();
        }
        public async void AddAnswer(Answer unsettedAnswer)
        {
            await AnswerContext.AddAnswer(unsettedAnswer);
            SetAnswers();
        }
        public async void UpdateAllAnswers()
        {
            foreach (var Answer in Answers)
            {
                await AnswerContext.UpdateAnswer(Answer);
            }
            SetAnswers();
        }
        public async void UpdateAnswer(Answer oldAnswer)
        {
            await AnswerContext.UpdateAnswer(oldAnswer);
        }
        #endregion
        public AnswerService(IAnswerRequests answerRequests)
        {
            AnswerContext = answerRequests;
            SetAnswers();
        }
    }
}
