using MYWFE.Core;
using MYWFE.MVVM.Model.ApiRequests;
using MYWFE.MVVM.ViewModel;
using MYWFE.Services;
using MYWFE.Utils.Components.Dialog;
using MYWFE.Utils.Components.Dialog.CustomModal;
using MYWFE.Utils.Services;

namespace MYWFE.ViewModel
{
    class MainViewModel : Core.ViewModel
    {
        #region PassedValues
        private INavigationService _navigation;
        public INavigationService Navigation
        {
            get => _navigation;
            set
            {
                _navigation = value;
                onPropertyChanged(nameof(Navigation));
            }
        }

        private IConfigurationService _configuration;
        public IConfigurationService Configuration
        {
            get => _configuration;
            set
            {
                _configuration = value;
                onPropertyChanged(nameof(Configuration));
            }
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

        public DialogHostViewModel DialogHost { get; }
        public CustomModalViewModel CustomModalViewModel { get; }

        private FeedbackRequestsAPI FeedbackRequestsAPI { get; }
        #endregion
        #region Commands
        private RelayCommand _navigateHomeCommand;
        public RelayCommand NavigateHomeCommand
        {
            get
            {
                return _navigateHomeCommand ??= new RelayCommand(obj =>
                {
                    Navigation.Navigate<HomeViewModel>();
                }, obj => true);
            }
        }

        private RelayCommand _navigateSettingsCommand;
        public RelayCommand NavigateSettingsCommand
        {
            get
            {
                return _navigateSettingsCommand ??= new RelayCommand(obj =>
                {
                    Navigation.Navigate<SettingsViewModel>();
                }, obj => true);
            }
        }

        private RelayCommand _navigateReviewCommand;
        public RelayCommand NavigateReviewCommand
        {
            get
            {
                return _navigateReviewCommand ??= new RelayCommand(obj =>
                {
                    Navigation.Navigate<ReviewViewModel>();
                }, obj => Configuration.Configuration.AutoAnswerOnReviews == false);
            }
        }

        private RelayCommand _navigateQuestionCommand;
        public RelayCommand NavigateQuestionCommand
        {
            get
            {
                return _navigateQuestionCommand ??= new RelayCommand(obj =>
                {
                    Navigation.Navigate<QuestionViewModel>();
                }, obj => true);
            }
        }

        private RelayCommand _navigateTemplatesCommand;
        public RelayCommand NavigateTemplatesCommand
        {
            get
            {
                return _navigateTemplatesCommand ??= new RelayCommand(obj =>
                {
                    Navigation.Navigate<TemplatesViewModel>();
                }, obj => Configuration.Configuration.AutoAnswerOnReviews == false);
            }
        }
        private RelayCommand _openDialogCommand;
        public RelayCommand OpenDialogCommand
        {
            get
            {
                return _openDialogCommand ??= new RelayCommand(async obj =>
                {
                    var dialogOutput = await DialogHost.ShowAsync(CustomModalViewModel, new CustomModalInput(UserService.User));
                    if (dialogOutput.DialogActionResult == DialogActionResult.Confirm && dialogOutput.DialogResult != null)
                    {
                        await Task.Run(() => UserService.AddUser(dialogOutput.DialogResult));
                    }
                }, obj => true);
            }
        }
        #endregion
        #region Methods
        private async Task AutoAnswersOnReviews()
        {
            while (true)
            {
                if (Configuration.Configuration.AutoAnswerOnReviews)
                {
                    //await FeedbackRequestsAPI.AutoReviewAnswers(UserService.User.FeedbackToken);
                    await Task.Delay(3600000);
                }
                else
                {
                    await Task.Delay(20000);
                }
            }
        }
        #endregion
        public MainViewModel(INavigationService navService, IConfigurationService configurationService, IUserService userService, DialogHostViewModel dialogHost, FeedbackRequestsAPI feedbackRequestsAPI, CustomModalViewModel customModal)
        {
            Navigation = navService;
            Configuration = configurationService;
            UserService = userService;

            DialogHost = dialogHost;
            CustomModalViewModel = customModal;

            FeedbackRequestsAPI = feedbackRequestsAPI;

            Task.Run(AutoAnswersOnReviews);
        }
    }
}
