using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using MYWFE.Utils.Services;
using MYWFE.Utils.Components.Dialog.CustomModal;
using MYWFE.Utils.Components.Dialog;
using MYWFE.Core;
using System.Xml;
using MYWFE.MVVM.Model.ApiRequests;
using MYWFE.Utils.Components.Dialog.CustomMessageBox;

namespace MYWFE.MVVM.ViewModel
{
    class HomeViewModel : Core.ViewModel
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
            get { return _userService; }
            set { _userService = value; onPropertyChanged(nameof(_userService)); }
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
        private CustomModalViewModel CustomModalViewModel { get; }
        private CustomMessageBoxViewModel CustomMessageBoxViewModel { get; }

        private StatisticsRequestsAPI StatisticsRequestsAPI { get; }
        private FeedbackRequestsAPI FeedbackRequestsAPI { get; }
        #endregion
        #region Values
        private readonly string _currentMonthDateString = XmlConvert.ToString(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1), XmlDateTimeSerializationMode.Unspecified);
        private readonly DateTime _currentMonthDateDateTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);

        private static readonly SKColor _white = new(255, 255, 255);
        private static readonly SKColor _darkmain = new(29, 40, 64);
        private static readonly SKColor _pink = new(217, 169, 178);

        private ISeries[] _series;
        public ISeries[] Series
        {
            get { return _series; }
            set { _series = value; onPropertyChanged(nameof(Series)); }
        }

        private ICartesianAxis[] _xAxes;
        public ICartesianAxis[] XAxes
        {
            get { return _xAxes; }
            set { _xAxes = value; onPropertyChanged(nameof(XAxes)); }
        }

        private ICartesianAxis[] _yAxes;
        public ICartesianAxis[] YAxes
        {
            get { return _yAxes; }
            set { _yAxes = value; onPropertyChanged(nameof(YAxes)); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; onPropertyChanged(nameof(IsLoading)); }
        }

        private string _reviewsCountity;
        public string ReviewsCountity
        {
            get { return _reviewsCountity; }
            set { _reviewsCountity = value; onPropertyChanged(nameof(ReviewsCountity)); }
        }

        private string _reviewsCountityToday;
        public string ReviewsCountityToday
        {
            get { return _reviewsCountityToday; }
            set { _reviewsCountityToday = value; onPropertyChanged(nameof(ReviewsCountityToday)); }
        }

        private string _questionsCountity;
        public string QuestionsCountity
        {
            get { return _questionsCountity; }
            set { _questionsCountity = value; onPropertyChanged(nameof(QuestionsCountity)); }
        }

        private string _questionsCountityToday;
        public string QuestionsCountityToday
        {
            get { return _questionsCountityToday; }
            set { _questionsCountityToday = value; onPropertyChanged(nameof(QuestionsCountityToday)); }
        }

        private bool _isFailedToAchiveData;
        public bool IsFailedToAchiveData
        {
            get { return _isFailedToAchiveData; }
            set { _isFailedToAchiveData = value; onPropertyChanged(nameof(IsFailedToAchiveData)); }
        }
        #endregion
        #region Commands
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
                        InitHomeView();
                    }
                }, obj => true);
            }
        }
        private RelayCommand _startAnswers;
        public RelayCommand StartAnswers
        {
            get
            {
                return _startAnswers ??= new RelayCommand(async obj =>
                {
                if (AnswerService.Answers != null && AnswerService.Answers.Count > 0)
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
                        }
                        else { await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Произошла ошибка в отправке ответов!")); }
                    }
                    }
                else
                {
                    await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Для запуска функции необходимо заполнить шаблоны!"));
                }
                }, o => true);
            }
        }

        private RelayCommand _setAutoAnswers;
        public RelayCommand SetAutoAnswers
        {
            get
            {
                return _setAutoAnswers ??= new RelayCommand(async obj =>
                {
                    if(AnswerService.Answers != null && AnswerService.Answers.Count > 0)
                    {
                        var dialogOutput = await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Запустить автоматическую отправку ответов по шаблонам каждый час?" +
                            "\nДля этого нужно будет не выходить из приложения." +
                            "\n" +
                            "\nФункция заработает через 30 секунд после ее включения!" +
                            "\n" +
                            "\nПРЕДУПРЕЖДЕНИЕ: при включенной функции нельзя будет отслеживать вкладки «Отзывы» и «Шаблоны»" +
                            "\nв целях предотвращения ошибок."));
                        if (dialogOutput.DialogActionResult == DialogActionResult.Confirm)
                        {
                            ConfigurationService.Configuration.AutoAnswerOnReviews = true;
                        }
                    }
                    else
                    {
                        await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Для запуска функции необходимо заполнить шаблоны!"));
                    }
                }, obj => true);
            }
        }

        private RelayCommand _disableAutoAnswers;
        public RelayCommand DisableAutoAnswers
        {
            get
            {
                return _disableAutoAnswers ??= new RelayCommand(async obj =>
                {
                    ConfigurationService.Configuration.AutoAnswerOnReviews = false;
                    if (!ConfigurationService.Configuration.DisableNotifications) { 
                        await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Вы успешно выключили автоматическую отправку ответов по шаблонам каждый час!"));
                    }
                }, obj => true);
            }
        }
        #endregion
        #region Methods
        private List<string> SetUpDays(DateTime dateTime)
        {
            return Enumerable.Range(1, dateTime.Day).Select(x => x.ToString()).ToList<string>();
        }

        private string SetUpMonth(DateTime dateTime)
        {
            string CurrentMonth = dateTime.ToString("MMMM");
            return char.ToUpper(CurrentMonth[0]) + CurrentMonth.ToLower().Substring(1);
        }

        private async Task<List<int>> GetOrders()
        {
            var Result = await StatisticsRequestsAPI.GetOrdersByMonth(UserService.User.StatiscticsToken, _currentMonthDateString);
            if (Result == null)
            {
                await Task.Run(() => IsFailedToAchiveData = true);
                return new List<int>();
            }
            return Result.Values.ToList<int>();
        }

        private async Task<List<int>> GetSales()
        {
            var Result = await StatisticsRequestsAPI.GetSalesByMonth(UserService.User.StatiscticsToken, _currentMonthDateString);
            if (Result == null)
            {
                await Task.Run(() => IsFailedToAchiveData = true);
                return new List<int>();
            }
            return Result.Values.ToList<int>();
        }

        private async Task LoadReviews()
        {
            await Task.Delay(1010);
            var result = await FeedbackRequestsAPI.GetUnansweredReviewCountity(UserService.User.FeedbackToken);
            ReviewsCountity = $"Нет ответа на отзывы: {result.data.countUnanswered}";
            ReviewsCountityToday = $"Новые отзывы за сегодня: {result.data.countUnansweredToday}";
        }

        private async Task LoadQuestions()
        {
            await Task.Delay(1010);
            var result = await FeedbackRequestsAPI.GetUnansweredQuestionsCountity(UserService.User.FeedbackToken);
            QuestionsCountity = $"Нет ответа на вопросы: {result.data.countUnanswered}";
            QuestionsCountityToday = $"Новые вопросы за сегодня: {result.data.countUnansweredToday}";
        }

        private async Task LoadChart(DateTime CurrentDateTime)
        {
            List<int> Orders = await GetOrders();
            List<int> Sales = await GetSales();
            await Task.Delay(2000);
            await Task.Run(() =>
            {
                Series = [
                new LineSeries<int>
                {
                    Name = "Заказы",
                    LineSmoothness = 1,
                    Values = Orders,
                    Stroke = new SolidColorPaint(_pink, 2),
                    GeometrySize = 10,
                    GeometryFill = new SolidColorPaint(_white, 2),
                    GeometryStroke = new SolidColorPaint(_pink, 2),
                    Fill = null,
                    ScalesYAt = 0
                },
                new LineSeries<int>
                {
                    Name = "Продажи",
                    Values = Sales,
                    Stroke = new SolidColorPaint(_darkmain, 2),
                    GeometrySize = 10,
                    GeometryStroke = new SolidColorPaint(_darkmain, 2),
                    Fill = null,
                    ScalesYAt = 0
                },
            ];
                XAxes = [
                    new Axis
                {
                    Name = SetUpMonth(CurrentDateTime),
                    Labels = SetUpDays(CurrentDateTime),
                    NamePaint = new SolidColorPaint
                    {
                        Color = _white,
                        SKTypeface = SKTypeface.FromFamilyName("Segoe UI", SKFontStyle.Bold),
                    },
                    NameTextSize = 14,
                    LabelsPaint = new SolidColorPaint
                    {
                        Color = _white,
                    },
                    Padding = new LiveChartsCore.Drawing.Padding(10),
                    LabelsRotation = 0
                }
                ];
                YAxes = [
                    new Axis
                {
                    Name = "Количество (шт.)",
                    NameTextSize = 14,
                    MinLimit = 0,
                    NamePaint = new SolidColorPaint
                    {
                        Color = _white,
                        SKTypeface = SKTypeface.FromFamilyName("Segoe UI", SKFontStyle.Bold),
                    },
                    LabelsPaint = new SolidColorPaint
                    {
                        Color = _pink,
                    },
                    SeparatorsPaint = new SolidColorPaint
                    {
                        Color = _white,
                        PathEffect = new DashEffect(new float[] { 15, 10 })
                    },
                    DrawTicksPath = false,
                }
                ];
            });
        }
        public async void InitHomeView()
        {
            await Task.Run(() => IsLoading = true);
            var loadChart = LoadChart(_currentMonthDateDateTime);
            var loadReviews = LoadReviews();
            var loadQuestions = LoadQuestions();

            await Task.WhenAll(loadChart, loadReviews, loadQuestions);
            await Task.Run(() => IsLoading = false);
        }
        #endregion
        public HomeViewModel(IUserService userService, IAnswerService answerService, IConfigurationService configurationService, CustomMessageBoxViewModel customMessageBoxViewModel, DialogHostViewModel dialogHost, CustomModalViewModel customModal, StatisticsRequestsAPI statisticsRequestsAPI, FeedbackRequestsAPI feedbackRequestsAPI)
        {
            UserService = userService;
            AnswerService = answerService;
            ConfigurationService = configurationService;

            DialogHost = dialogHost;
            CustomModalViewModel = customModal;
            CustomMessageBoxViewModel = customMessageBoxViewModel;

            StatisticsRequestsAPI = statisticsRequestsAPI;
            FeedbackRequestsAPI = feedbackRequestsAPI;

            if (UserService.User != null)
            {
                InitHomeView();
            }
        }
    }
}
