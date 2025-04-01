using System.Reflection;
using MYWFE.Core;
using MYWFE.Utils.Components.Dialog;
using MYWFE.Utils.Components.Dialog.CustomMessageBox;
using MYWFE.Utils.Components.Dialog.CustomModal;
using MYWFE.Utils.Services;

namespace MYWFE.MVVM.ViewModel
{
    class SettingsViewModel : Core.ViewModel
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
            set { _userService = value; onPropertyChanged(nameof(UserService)); }
        }

        private DialogHostViewModel DialogHost { get; }

        private CustomModalViewModel CustomModalViewModel { get; }

        private CustomMessageBoxViewModel CustomMessageBoxViewModel { get; }
        #endregion
        #region Values
        private string _version;
        public string Version
        {
            get { return _version; }
            set { _version = value; }
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
                        if (UserService.User != null)
                        {
                            await Task.Run(() => UserService.UpdateUser(new Utils.Types.User()
                            {
                                FeedbackToken = dialogOutput.DialogResult.FeedbackToken,
                                StatiscticsToken = dialogOutput.DialogResult.StatiscticsToken,
                                Id = UserService.User.Id
                            }));
                        }
                        else
                        {
                            await Task.Run(() => UserService.AddUser(dialogOutput.DialogResult));
                        }
                    }
                }, obj => true);
            }
        }

        private RelayCommand _exitUser;
        public RelayCommand ExitUser
        {
            get
            {
                return _exitUser ??= new RelayCommand(async obj =>
                {
                    var dialogOutput = await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Вы уверены что хотите выйти из аккаунта?"));
                    if (dialogOutput.DialogActionResult == DialogActionResult.Confirm)
                    {
                        UserService.RemoveUser();
                        ConfigurationService.Configuration.AutoAnswerOnReviews = false;
                    }
                }, obj => true);
            }
        }

        private RelayCommand _saveConfig;
        public RelayCommand SaveConfig
        {
            get
            {
                return _saveConfig ??= new RelayCommand(async obj =>
                {
                    ConfigurationService.SaveConfiguration();
                }, obj => true);
            }
        }

        private RelayCommand _resetConfig;
        public RelayCommand ResetConfig
        {
            get
            {
                return _resetConfig ??= new RelayCommand(async obj =>
                {
                    var dialogOutput = await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Вы уверены что хотите сбросить все настройки до стандартных?"));
                    if (dialogOutput.DialogActionResult == DialogActionResult.Confirm)
                    {
                        await Task.Run(ConfigurationService.ResetConfiguration);
                    }
                }, obj => true);
            }
        }
        #endregion
        public SettingsViewModel(IConfigurationService configurationService, IUserService userService, CustomModalViewModel customModal, DialogHostViewModel dialogHost, CustomMessageBoxViewModel customMessageBoxViewModel)
        {
            ConfigurationService = configurationService;
            UserService = userService;

            DialogHost = dialogHost;
            CustomModalViewModel = customModal;
            CustomMessageBoxViewModel = customMessageBoxViewModel;

            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
