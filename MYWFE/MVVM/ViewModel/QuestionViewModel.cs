using System.Collections.ObjectModel;
using MYWFE.Core;
using MYWFE.Utils.Types;
using MYWFE.Utils.Components.Dialog.CustomMessageBox;
using MYWFE.Utils.Components.Dialog;
using MYWFE.Utils.Services;
using MYWFE.Utils.Components.Dialog.CustomModal;
using MYWFE.MVVM.Model.ApiRequests;

namespace MYWFE.MVVM.ViewModel
{
    class QuestionViewModel : Core.ViewModel
    {
        #region PassedValues
        private IConfigurationService _configurationService;
        public IConfigurationService ConfigurationService
        {
            get { return _configurationService; }
            set { _configurationService = value; }
        }

        private IUserService _userService;
        public IUserService UserService
        {
            get => _userService;
            set
            {
                _userService = value;
                onPropertyChanged();
            }
        }

        private DialogHostViewModel DialogHost { get; }
        public CustomModalViewModel CustomModalViewModel { get; }
        private CustomMessageBoxViewModel CustomMessageBoxViewModel { get; }

        private FeedbackRequestsAPI FeedbackRequestsAPI { get; }
        #endregion
        #region Values
        private readonly int CountityOnPage = 20;

        private HomeViewModel HomeViewModel { get; set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; onPropertyChanged(nameof(IsLoading)); }
        }

        private bool _isReloadReady;
        public bool IsReloadReady
        {
            get { return _isReloadReady; }
            set { _isReloadReady = value; onPropertyChanged(nameof(IsReloadReady)); }
        }

        private bool _isContentLoading;
        public bool IsContentLoading
        {
            get { return _isContentLoading; }
            set { _isContentLoading = value; onPropertyChanged(nameof(IsContentLoading)); }
        }

        public int AllPages { get; set; }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; onPropertyChanged(nameof(CurrentPage)); }
        }

        private ObservableCollection<Question> _questionList = new();
        public ObservableCollection<Question> QuestionList
        {
            get { return _questionList; }
            set { _questionList = value; }
        }
        #endregion
        #region Commands
        private RelayCommand _sendAnswer;
        public RelayCommand SendAnswer
        {
            get
            {
                return _sendAnswer ??= new RelayCommand(async obj =>
                {
                    Question question = obj as Question;
                    var dialogOutput = await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Вы уверены, что хотите отправить ответ на отзыв?"));
                    if (dialogOutput.DialogActionResult == DialogActionResult.Dismiss)
                    {
                        return;
                    }
                    var IsSuccess = await FeedbackRequestsAPI.AnswerQuestion(UserService.User.FeedbackToken, question.id, question.AnswerText, false);
                    //var IsSuccess = true;
                    if (IsSuccess) { 
                        if (!ConfigurationService.Configuration.DisableNotifications)
                        {
                            await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Ответ успешно отправлен!"));
                        }
                        QuestionList.Remove(question);
                        await Task.Run(() => IsReloadReady = true);
                    }
                    else { await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Произошла ошибка в отправке ответа!")); }
                }, obj => true);
            }
        }

        private RelayCommand _rejectQuestion;
        public RelayCommand RejectQuestion
        {
            get
            {
                return _rejectQuestion ??= new RelayCommand(async obj =>
                {
                    Question question = obj as Question;
                    var dialogOutput = await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Вы уверены, что хотите отклонить отзыв?"));
                    if (dialogOutput.DialogActionResult == DialogActionResult.Dismiss)
                    {
                        return;
                    }
                    var IsSuccess = await FeedbackRequestsAPI.AnswerQuestion(UserService.User.FeedbackToken, question.id, question.AnswerText, true);
                    //var IsSuccess = true;
                    if (IsSuccess)
                    {
                        if (!ConfigurationService.Configuration.DisableNotifications)
                        {
                            await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Вопрос отклонён продавцом!"));
                        }
                        QuestionList.Remove(question);
                        await Task.Run(() => IsReloadReady = true);
                    }
                    else { await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Произошла ошибка в отправке ответа!")); }
                }, obj => true);
            }
        }

        private RelayCommand _reloadList;
        public RelayCommand ReloadList
        {
            get
            {
                return _reloadList ??= new RelayCommand(obj =>
                {
                    InitQuestion();
                }, obj => true);
            }
        }

        private RelayCommand _firstPage;
        public RelayCommand FirstPage
        {
            get
            {
                return _firstPage ??= new RelayCommand(async obj =>
                {
                    CurrentPage = 1;
                    await Task.Run(() => IsContentLoading = true);
                    var changePageTask = LoadQuestions();
                    await Task.WhenAll(changePageTask);
                    await Task.Run(() => IsContentLoading = false);
                }, obj => (CurrentPage != 1));
            }
        }

        private RelayCommand _previousPage;
        public RelayCommand PreviousPage
        {
            get
            {
                return _previousPage ??= new RelayCommand(async obj =>
                {
                    CurrentPage -= 1;
                    await Task.Run(() => IsContentLoading = true);
                    var changePageTask = LoadQuestions();
                    await Task.WhenAll(changePageTask);
                    await Task.Run(() => IsContentLoading = false);
                }, obj => CurrentPage != 1);
            }
        }

        private RelayCommand _nextPage;
        public RelayCommand NextPage
        {
            get
            {
                return _nextPage ??= new RelayCommand(async obj =>
                {
                    CurrentPage += 1;
                    await Task.Run(() => IsContentLoading = true);
                    var changePageTask = LoadQuestions();
                    await Task.WhenAll(changePageTask);
                    await Task.Run(() => IsContentLoading = false);
                }, obj => CurrentPage != AllPages);
            }
        }

        private RelayCommand _lastPage;
        public RelayCommand LastPage
        {
            get
            {
                return _lastPage ??= new RelayCommand(async obj =>
                {
                    CurrentPage = AllPages;
                    await Task.Run(() => IsContentLoading = true);
                    var changePageTask = LoadQuestions();
                    await Task.WhenAll(changePageTask);
                    await Task.Run(() => IsContentLoading = false);
                }, obj => CurrentPage != AllPages);
            }
        }

        private RelayCommand _openModalCommand;
        public RelayCommand OpenModalCommand
        {
            get
            {
                return _openModalCommand ??= new RelayCommand(async obj =>
                {
                    var dialogOutput = await DialogHost.ShowAsync(CustomModalViewModel, new CustomModalInput(UserService.User));
                    if (dialogOutput.DialogActionResult == DialogActionResult.Confirm && dialogOutput.DialogResult != null)
                    {
                        await Task.Run(() => UserService.AddUser(dialogOutput.DialogResult));
                        InitQuestion();
                        HomeViewModel.InitHomeView();
                    }
                }, obj => true);
            }
        }
        #endregion
        #region Methods
        private async Task LoadQuestions()
        {
            QuestionList.Clear();
            var Result = await FeedbackRequestsAPI.GetUnansweredQuestionsList(UserService.User.FeedbackToken, CurrentPage, CountityOnPage);
            if (Result != null)
            {
                await Task.Run(() =>
                {
                    QuestionList = new ObservableCollection<Question>(Result.data.questions.Select(i => new Question()
                    {
                        id = i.id,
                        productDetails = i.productDetails,
                        text = i.text,
                    }));
                    onPropertyChanged(nameof(QuestionList));
                });
            }
        }

        private async Task LoadPagesCountity()
        {
            var Result = await FeedbackRequestsAPI.GetUnansweredQuestionsCountity(UserService.User.FeedbackToken);
            await Task.Run(() =>
            {
                if (Result.data.countUnanswered == 0) { AllPages = 1; return; }
                AllPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Result.data.countUnanswered) / Convert.ToDouble(CountityOnPage)));
                onPropertyChanged(nameof(AllPages));
            });
        }

        private async void InitQuestion()
        {
            await Task.Run(() => IsLoading = true);
            await Task.Run(() => IsReloadReady = false);
            var loadPagesCountity = LoadPagesCountity();
            var loadQuestions = LoadQuestions();

            await Task.WhenAll(loadPagesCountity, loadQuestions);
            await Task.Run(() => IsLoading = false);
        }
        #endregion
        public QuestionViewModel(HomeViewModel homeViewModel, IUserService userService, IConfigurationService configurationService, DialogHostViewModel dialogHost, CustomModalViewModel customModalViewModel, CustomMessageBoxViewModel customMessageBoxViewModel, FeedbackRequestsAPI feedbackRequestsAPI)
        {
            DialogHost = dialogHost;
            CustomMessageBoxViewModel = customMessageBoxViewModel;
            ConfigurationService = configurationService;
            CustomModalViewModel = customModalViewModel;
            UserService = userService;
            FeedbackRequestsAPI = feedbackRequestsAPI;
            HomeViewModel = homeViewModel;
            if (UserService.User != null)
            {
                InitQuestion();
            }
        }
    }
}
