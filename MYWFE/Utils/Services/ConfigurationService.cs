using MYWFE.Core;
using System.Configuration;

namespace MYWFE.Utils.Services
{
    public interface IConfigurationService
    {
        BindableSettings Configuration { get; set; }
        void SaveConfiguration();
        void ResetConfiguration();
    }
    public class ConfigurationService : ObservableObject, IConfigurationService
    {
        #region Values
        private Configuration _appConfiguration;

        public Configuration AppConfiguration
        {
            get { return _appConfiguration; }
            private set { _appConfiguration = value; onPropertyChanged(nameof(AppConfiguration)); }
        }
        private BindableSettings _configuration;

        public BindableSettings Configuration
        {
            get { return _configuration; }
            set { _configuration = value; onPropertyChanged(nameof(Configuration)); }
        }
        #endregion
        #region Methods
        public async void SaveConfiguration()
        {
            await Task.Run(() =>
             {
                 AppConfiguration.Save();
                 ConfigurationManager.RefreshSection("Settings");
             });
        }
        public void ResetConfiguration()
        {
            AppConfiguration.Sections.Remove("Settings");
            SetUpConfig();
            SaveConfiguration();
        }
        private void SetUpConfig()
        {
            if (AppConfiguration.Sections["Settings"] is null)
            {
                AppConfiguration.Sections.Add("Settings", new Settings()
                {
                    DisableNotifications = false,
                    DisableIconOnBG = false,
                    StartUpOnWindows = false,
                    AutoAnswerOnReviews = false
                });
                AppConfiguration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("Settings");
            }
            Configuration = new BindableSettings((Settings)_appConfiguration.GetSection("Settings"));
        }
        #endregion
        public ConfigurationService()
        {
            AppConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            SetUpConfig();
        }
    }
}
