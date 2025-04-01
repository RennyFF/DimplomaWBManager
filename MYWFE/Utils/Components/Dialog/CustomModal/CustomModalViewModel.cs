using FluentValidation.Internal;
using FluentValidation;
using MYWFE.Core;
using MYWFE.Utils.Types;
using System.Collections;
using System.ComponentModel;
using MYWFE.MVVM.Model.ApiRequests;
using MYWFE.MVVM.Model.ApiRequests.ResponseTypes;

namespace MYWFE.Utils.Components.Dialog.CustomModal
{
    #region DialogSettings
    public class CustomModalInput : User, IDialogContentInput
    {
        public CustomModalInput(User? user)
        {
            if (user != null)
            {
                StatiscticsToken = user.StatiscticsToken;
                FeedbackToken = user.FeedbackToken;
            }
        }
    }
    public class CustomModalOutput(DialogActionResult dialogActionResult, User? dialogResult) : IDialogContentOutput
    {
        public DialogActionResult DialogActionResult { get; } = dialogActionResult;
        public User? DialogResult { get; } = dialogResult;
    }
    #endregion
    public class CustomModalViewModel : Core.ViewModel, IUser, INotifyDataErrorInfo, IDialogContentViewModel<CustomModalInput, CustomModalOutput>
    {
        #region PassedValues
        private BaseRequestsAPI RequestsAPI { get; }
        private readonly Utils.ModalValidation Validator;
        #endregion
        #region Values
        private TaskCompletionSource<CustomModalOutput>? _tcs;

        private bool _isPingingStatistics;
        public bool IsPingingStatistics
        {
            get { return _isPingingStatistics; }
            set { _isPingingStatistics = value; onPropertyChanged(nameof(IsPingingStatistics)); }
        }

        private string _statiscticsToken;
        public string StatiscticsToken
        {
            get { return _statiscticsToken; }
            set { _statiscticsToken = value; onPropertyChanged(nameof(StatiscticsToken)); ValidatePropertyAsync(nameof(StatiscticsToken)); }
        }

        private bool? _statisticsApproved = null;
        public bool? StatisticsApproved
        {
            get { return _statisticsApproved; }
            set { _statisticsApproved = value; onPropertyChanged(nameof(StatisticsApproved)); }
        }

        private bool _isPingingFeedbacks;
        public bool IsPingingFeedbacks
        {
            get { return _isPingingFeedbacks; }
            set { _isPingingFeedbacks = value; onPropertyChanged(nameof(IsPingingFeedbacks)); }
        }

        private string _feedbackToken;
        public string FeedbackToken
        {
            get { return _feedbackToken; }
            set { _feedbackToken = value; onPropertyChanged(nameof(FeedbackToken)); ValidatePropertyAsync(nameof(FeedbackToken)); }
        }

        private bool? _feedbackApproved = null;
        public bool? FeedbackApproved
        {
            get { return _feedbackApproved; }
            set { _feedbackApproved = value; onPropertyChanged(nameof(FeedbackApproved)); }
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
                    ValidateAllFields();
                    if (!HasErrors)
                    {
                        await Task.Run(() =>
                        {
                            IsPingingStatistics = true;
                            IsPingingFeedbacks = true;
                        });
                        StatisticsApproved = null;
                        FeedbackApproved = null;

                        await Task.Delay(10001);
                        StatisticsApproved = await RequestsAPI.PingToken(StatiscticsToken, PingCategories.Statistics);
                        await Task.Run(() => IsPingingStatistics = false);
                        await Task.Delay(10001);
                        FeedbackApproved = await RequestsAPI.PingToken(FeedbackToken, PingCategories.Feedback);
                        await Task.Run(() => IsPingingFeedbacks = false);
                        if ((bool)StatisticsApproved && (bool)FeedbackApproved)
                        {
                            AccountInfoResponse accountInfo = await RequestsAPI.GetAccountInformation(FeedbackToken);
                            await Task.Run(() => _tcs?.SetResult(new(DialogActionResult.Confirm, new User()
                            {
                                FeedbackToken = FeedbackToken,
                                StatiscticsToken = StatiscticsToken,
                                UserName = accountInfo.name,
                                TradeMark = accountInfo.tradeMark
                            }
                            )));
                        }
                    }
                }, obj => (
                !IsPingingStatistics &&
                !IsPingingFeedbacks
                ));
            }
        }

        private RelayCommand _dismissCommand { get; set; }
        public RelayCommand DismissCommand
        {
            get
            {
                return _dismissCommand ??= new RelayCommand(obj =>
                {
                    _tcs?.SetResult(new(DialogActionResult.Dismiss, null));
                }, obj => true);
            }
        }
        #endregion
        public void Initialize(CustomModalInput parameters, TaskCompletionSource<CustomModalOutput> tcs)
        {
            FeedbackToken = parameters.FeedbackToken;
            StatiscticsToken = parameters.StatiscticsToken;
            _tcs = tcs;
        }
        #region Validation
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        private async void ValidateAllFields()
        {
            ValidatePropertyAsync(nameof(FeedbackToken));
            ValidatePropertyAsync(nameof(StatiscticsToken));
        }
        private Dictionary<string, List<string>> _errors = new();

        public IEnumerable GetErrors(string propertyName)
        {
            return _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;
        }

        public bool HasErrors => _errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChange;

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChange?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        private async void ValidatePropertyAsync(string propertyName)
        {
            var context = new ValidationContext<CustomModalViewModel>(this, new PropertyChain(), new MemberNameValidatorSelector(new[] { propertyName }));
            var result = await Validator.ValidateAsync(context);
            _errors.Remove(propertyName);
            foreach (var error in result.Errors)
            {
                if (error.PropertyName == propertyName)
                {
                    if (!_errors.ContainsKey(propertyName))
                    {
                        _errors[propertyName] = new List<string>();
                    }
                    _errors[propertyName].Add(error.ErrorMessage);
                }
            }
            OnErrorsChanged(propertyName);
        }
        #endregion
        public CustomModalViewModel(BaseRequestsAPI baseRequestsAPI)
        {
            Validator = new Utils.ModalValidation();
            RequestsAPI = baseRequestsAPI;
        }
    }
}
