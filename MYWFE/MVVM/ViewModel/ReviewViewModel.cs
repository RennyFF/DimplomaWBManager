using System.Collections.ObjectModel;
using MYWFE.Core;
using MYWFE.Utils.Components.Dialog.CustomMessageBox;
using MYWFE.Utils.Components.Dialog;
using MYWFE.Utils.Components.Dialog.CustomFeedbackAnswerModal;
using MYWFE.Utils.Components.Dialog.CustomImageContainerModal;
using MYWFE.Utils.Services;
using MYWFE.Utils.Components.Dialog.CustomModal;
using MYWFE.MVVM.Model.ApiRequests;
using MYWFE.MVVM.Model.ApiRequests.ResponseTypes;

namespace MYWFE.MVVM.ViewModel
{
    class ReviewViewModel : Core.ViewModel
    {
        #region PassedValues
        private IConfigurationService _configurationService;
        public IConfigurationService ConfigurationService
        {
            get { return _configurationService; }
            set { _configurationService = value; onPropertyChanged(nameof(ConfigurationService)); }
        }

        private IUserService _userService;
        public IUserService UserService
        {
            get => _userService;
            set
            {
                _userService = value;
                onPropertyChanged(nameof(UserService));
            }
        }

        private IAnswerService _answerService;
        public IAnswerService AnswerService
        {
            get => _answerService;
            set
            {
                _answerService = value;
                onPropertyChanged(nameof(AnswerService));
            }
        }

        private DialogHostViewModel DialogHost { get; }
        private CustomFeedbackAnswerModalViewModel CustomFeedbackAnswerModalViewModel { get; }
        private CustomImageContainerModalViewModel CustomImageContainerModalViewModel { get; }
        private CustomMessageBoxViewModel CustomMessageBoxViewModel { get; }
        public CustomModalViewModel CustomModalViewModel { get; }

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

        private bool _isContentLoading;
        public bool IsContentLoading
        {
            get { return _isContentLoading; }
            set { _isContentLoading = value; onPropertyChanged(nameof(IsContentLoading)); }
        }

        private bool _isReloadReady;
        public bool IsReloadReady
        {
            get { return _isReloadReady; }
            set { _isReloadReady = value; onPropertyChanged(nameof(IsReloadReady)); }
        }

        public int AllPages { get; set; }
        private int _currentPage = 1;
        public int CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; onPropertyChanged(nameof(CurrentPage)); }
        }

        private ObservableCollection<ReviewElement> _reviewList = new();
        public ObservableCollection<ReviewElement> ReviewList
        {
            get { return _reviewList; }
            set { _reviewList = value; }
        }
        #endregion
        #region Commands
        private RelayCommand _openAttachedImages;
        public RelayCommand OpenAttachedImages
        {
            get
            {
                return _openAttachedImages ??= new RelayCommand(async obj =>
                {
                    var dialogOutput = await DialogHost.ShowAsync(CustomImageContainerModalViewModel, new CustomImageContainerModalInput() { ImageList = (obj as ReviewElement).photoLinks});
                }, obj => true);
            }
        }

        private RelayCommand _openManualAnswer;
        public RelayCommand OpenManualAnswer
        {
            get
            {
                return _openManualAnswer ??= new RelayCommand(async obj =>
                {
                    ReviewElement review = obj as ReviewElement;
                    var dialogOutput = await DialogHost.ShowAsync(CustomFeedbackAnswerModalViewModel, new CustomFeedbackAnswerModalViewModelInput());
                    if(dialogOutput.DialogActionResult == DialogActionResult.Confirm)
                    {
                        //var IsSuccess = await FeedbackRequestsAPI.AnswerReview(UserService.User.FeedbackToken, review.id, dialogOutput.ManualAnswerText);
                        var IsSuccess = true;
                        if (IsSuccess) { 
                            if (!ConfigurationService.Configuration.DisableNotifications) {
                                await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Ответ успешно отправлен!"));
                            }
                            ReviewList.Remove(review);
                            await Task.Run(() => IsReloadReady = true);
                        }
                        else { await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Произошла ошибка в отправке ответа!")); }
                    }

                }, obj => true);
            }
        }

        private RelayCommand _startAutoAnswer;
        public RelayCommand StartAutoAnswer
        {
            get
            {
                return _startAutoAnswer ??= new RelayCommand(async obj =>
                {
                    var dialogOutput = await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Запустить автоматическую отправку ответов по шаблонам?"));
                    if (dialogOutput.DialogActionResult == DialogActionResult.Confirm)
                    {
                        //var IsSuccess = await FeedbackRequestsAPI.AutoReviewAnswers(UserService.User.FeedbackToken);
                        var IsSuccess = true;
                        if (IsSuccess)
                        {
                            if (!ConfigurationService.Configuration.DisableNotifications)
                            {
                                await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("На все отзывы ответы успешно отправлены!"));
                            }
                            await Task.Run(() => IsReloadReady = true);
                        }
                        else { await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Произошла ошибка в отправке ответов!")); }
                    }

                }, obj => AnswerService.Answers != null && AnswerService.Answers.Count>0 );
            }
        }

        private RelayCommand _reloadList;
        public RelayCommand ReloadList
        {
            get
            {
                return _reloadList ??= new RelayCommand(obj =>
                {
                    InitReview();
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
                    var changePageTask = LoadReviews();
                    await Task.WhenAll(changePageTask);
                    await Task.Run(() => IsContentLoading = false);
                }, obj => (CurrentPage != 1));
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
                    var changePageTask = LoadReviews();
                    await Task.WhenAll(changePageTask);
                    await Task.Run(() => IsContentLoading = false);
                }, obj => CurrentPage != AllPages);
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
                    var changePageTask = LoadReviews();
                    await Task.WhenAll(changePageTask);
                    await Task.Run(() => IsContentLoading = false);
                }, obj => CurrentPage != 1);
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
                    var changePageTask = LoadReviews();
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
                        InitReview();
                        HomeViewModel.InitHomeView();
                    }
                }, obj => true);
            }
        }
        #endregion
        #region Methods
        private async Task LoadReviews()
        {
            ReviewList.Clear();
            var Result = await FeedbackRequestsAPI.GetUnansweredReviewsList(UserService.User.FeedbackToken, CurrentPage, CountityOnPage);
            await Task.Run(() =>
            {
                ReviewList = new ObservableCollection<ReviewElement>(Result.data.feedbacks.Select(i => new ReviewElement()
                {
                    id = i.id,
                    productDetails = new ReviewElementProductDetails
                    {
                        nmId = i.productDetails.nmId,
                        size = i.productDetails.size.Length == 0 ? "не указан" : i.productDetails.size,
                        productName = i.productDetails.productName
                    },
                    cons = i.cons.Length == 0 ? "—" : i.cons,
                    photoLinks = i.photoLinks,
                    productValuation = i.productValuation,
                    pros = i.pros.Length == 0 ? "—" : i.pros,
                    userName = i.userName.Length == 0 ? "Пользователь" : i.userName,
                    text = i.text.Length == 0 ? "—" : i.text,
                }));
                onPropertyChanged(nameof(ReviewList));
            });
        }

        private async Task LoadPagesCountity()
        {
            var Result = await FeedbackRequestsAPI.GetUnansweredReviewCountity(UserService.User.FeedbackToken);
            await Task.Run(() =>
            {
                if (Result.data.countUnanswered == 0) { AllPages = 1; return; }
                AllPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Result.data.countUnanswered) / Convert.ToDouble(CountityOnPage)));
                onPropertyChanged(nameof(AllPages));
            });
        }

        private async void InitReview()
        {
            await Task.Run(() => IsLoading = true);
            var loadReviews = LoadReviews();
            var loadPagesCountity = LoadPagesCountity();

            await Task.WhenAll(loadPagesCountity, loadReviews);
            await Task.Run(() => IsLoading = false);
        }
        #endregion
        public ReviewViewModel(HomeViewModel homeViewModel, IUserService userService, IAnswerService answerService, IConfigurationService configurationService, FeedbackRequestsAPI feedbackRequestsAPI, DialogHostViewModel dialogHost, CustomModalViewModel customModalViewModel, CustomMessageBoxViewModel customMessageBoxViewModel, CustomFeedbackAnswerModalViewModel customFeedbackAnswerModalViewModel, CustomImageContainerModalViewModel customImageContainerModalViewModel)
        {
            UserService = userService;
            AnswerService = answerService;
            ConfigurationService = configurationService;

            DialogHost = dialogHost;
            CustomFeedbackAnswerModalViewModel = customFeedbackAnswerModalViewModel;
            CustomImageContainerModalViewModel = customImageContainerModalViewModel;
            CustomMessageBoxViewModel = customMessageBoxViewModel;
            CustomModalViewModel = customModalViewModel;
            HomeViewModel = homeViewModel;

            FeedbackRequestsAPI = feedbackRequestsAPI;
            if (UserService.User != null)
            {
                InitReview();
            }
        }
    }
}
