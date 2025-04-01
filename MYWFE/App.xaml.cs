using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using MYWFE.MVVM.Model.Database;
using MYWFE.MVVM.ViewModel;
using MYWFE.Services;
using MYWFE.ViewModel;
using NavigationService = MYWFE.Services.NavigationService;
using System.IO;
using MYWFE.MVVM.Model.Database.Requests;
using MYWFE.Utils.Services;
using MYWFE.Utils.Components.Dialog;
using MYWFE.Utils.Components.Dialog.CustomModal;
using MYWFE.Utils.Components.Dialog.CustomMessageBox;
using MYWFE.Utils.Components.SplashScreen;
using MYWFE.Utils.Components.Dialog.CustomImageContainerModal;
using MYWFE.Utils.Components.Dialog.CustomFeedbackAnswerModal;
using MYWFE.MVVM.Model.ApiRequests;

namespace MYWFE
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;
        public App()
        {
            IServiceCollection Services = new ServiceCollection();
            Services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });
            //Core VMs
            Services.AddSingleton<MainViewModel>();
            Services.AddSingleton<HomeViewModel>();
            Services.AddTransient<ReviewViewModel>();
            Services.AddTransient<QuestionViewModel>();
            Services.AddSingleton<TemplatesViewModel>();
            Services.AddSingleton<SettingsViewModel>();

            //Modals VMs
            Services.AddSingleton<DialogHostViewModel>();
            Services.AddTransient<CustomModalViewModel>();
            Services.AddTransient<CustomMessageBoxViewModel>();
            Services.AddTransient<CustomImageContainerModalViewModel>();
            Services.AddTransient<CustomFeedbackAnswerModalViewModel>();

            //API Requests
            Services.AddSingleton<BaseRequestsAPI>();
            Services.AddSingleton<StatisticsRequestsAPI>();
            Services.AddSingleton<FeedbackRequestsAPI>();

            //BD Requests/Context
            Services.AddScoped<IUserRequests, UserRequests>();
            Services.AddScoped<IAnswerRequests, AnswerRequests>();
            Services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "userdata.db")}"));

            //Services
            Services.AddSingleton<INavigationService, NavigationService>();
            Services.AddSingleton<IConfigurationService, ConfigurationService>();
            Services.AddSingleton<IUserService, UserService>();
            Services.AddSingleton<IAnswerService, AnswerService>();

            //Other stuff
            Services.AddTransient<SplashScreenView>();
            Services.AddTransient<SplashScreenViewModel>();

            Services.AddSingleton<Func<Type, Core.ViewModel>>(serviceProvider =>
                viewModelType => (Core.ViewModel)serviceProvider.GetRequiredService(viewModelType));

            _serviceProvider = Services.BuildServiceProvider();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            //Configure switching splashscreen and mainwindow
            using (var scope = _serviceProvider.CreateScope())
            {
                var DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                DbContext.Database.Migrate();
            }

            var MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            var SplashScreen = _serviceProvider.GetRequiredService<SplashScreenView>();
            var SplashScreenVM = _serviceProvider.GetRequiredService<SplashScreenViewModel>();

            SplashScreen.Show();
            await SplashScreenVM.InitializeAsync();
            MainWindow.Loaded += (s, ev) =>
            {
                SplashScreen.Close();
                MainWindow.Activate();
            };

            MainWindow.Show();
            MainWindow.Activate();

            base.OnStartup(e);
        }

    }

}
