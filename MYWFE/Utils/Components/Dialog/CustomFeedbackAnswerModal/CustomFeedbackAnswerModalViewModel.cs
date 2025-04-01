using MYWFE.Core;
using MYWFE.Utils.Services;
using MYWFE.Utils.Types;
using System.Collections.ObjectModel;

namespace MYWFE.Utils.Components.Dialog.CustomFeedbackAnswerModal
{
    #region DialogSettings
    public class CustomFeedbackAnswerModalViewModelInput : IDialogContentInput
    {
    }

    public class CustomFeedbackAnswerModalViewModelOutput(DialogActionResult dialogActionResult, string manualAnswerText) : IDialogContentOutput
    {
        public DialogActionResult DialogActionResult { get; } = dialogActionResult;
        public string ManualAnswerText { get; } = manualAnswerText;
    }
    #endregion
    public class CustomFeedbackAnswerModalViewModel : Core.ViewModel, IDialogContentViewModel<CustomFeedbackAnswerModalViewModelInput, CustomFeedbackAnswerModalViewModelOutput>
    {
        #region PassedValues
        private IAnswerService _answerService;
        public IAnswerService AnswerService
        {
            get { return _answerService; }
            set { _answerService = value; onPropertyChanged(nameof(AnswerService)); }
        }
        #endregion
        #region Values
        private TaskCompletionSource<CustomFeedbackAnswerModalViewModelOutput>? _tcs;

        private Answer? _selectedAnswer;
        public Answer? SelectedAnswer
        {
            set
            {
                _selectedAnswer = value;
                if (!string.IsNullOrEmpty(CustomText) && SelectedAnswer != null)
                { CustomText = string.Empty; }
                onPropertyChanged(nameof(SelectedAnswer));
            }
            get => _selectedAnswer;
        }

        private string _customText;
        public string CustomText
        {
            get { return _customText; }
            set { _customText = value; 
                onPropertyChanged(nameof(CustomText)); 
                if (SelectedAnswer != null && !string.IsNullOrEmpty(CustomText)) 
                { SelectedAnswer = null; } 
            }
        }

        private ObservableCollection<Answer> _answerList;
        public ObservableCollection<Answer> AnswerList
        {
            get { return _answerList; }
            set { _answerList = value; onPropertyChanged(nameof(AnswerList)); }
        }
        #endregion
        #region Commands
        private RelayCommand _confirmCommand;
        public RelayCommand ConfirmCommand
        {
            get
            {
                return _confirmCommand ??= new RelayCommand(async obj =>
                {
                    await Task.Run(() => _tcs?.SetResult(new(DialogActionResult.Confirm, SelectedAnswer != null ? SelectedAnswer.Text : CustomText)));
                }, obj => SelectedAnswer != null || !string.IsNullOrEmpty(CustomText));
            }
        }

        private RelayCommand _dismissCommand { get; set; }
        public RelayCommand DismissCommand
        {
            get
            {
                return _dismissCommand ??= new RelayCommand(obj =>
                {
                    _tcs?.SetResult(new(DialogActionResult.Dismiss, "Dismiss"));
                }, obj => true);
            }
        }
        #endregion
        public void Initialize(CustomFeedbackAnswerModalViewModelInput parameters, TaskCompletionSource<CustomFeedbackAnswerModalViewModelOutput> tcs)
        {
            _tcs = tcs;
        }
        public CustomFeedbackAnswerModalViewModel(IAnswerService answerService)
        {
            AnswerService = answerService;
        }
    }
}
