using Microsoft.Win32;
using MYWFE.Core;
using System.Configuration;

namespace MYWFE.Utils
{
    interface ISettings
    {
        bool StartUpOnWindows { get; set; }
        bool DisableIconOnBG { get; set; }
        bool DisableNotifications { get; set; }
        bool AutoAnswerOnReviews { get; set; }
    }
    public class Settings : ConfigurationSection, ISettings
    {
        #region SettignsValues
        [ConfigurationProperty("startUpOnWindows", DefaultValue = false)]
        public bool StartUpOnWindows
        {
            get { return (bool)this["startUpOnWindows"]; }
            set { this["startUpOnWindows"] = value; }
        }

        [ConfigurationProperty("disableIconOnBG", DefaultValue = false)]
        public bool DisableIconOnBG
        {
            get { return (bool)this["disableIconOnBG"]; }
            set { this["disableIconOnBG"] = value; }
        }

        [ConfigurationProperty("disableNotifications", DefaultValue = false)]
        public bool DisableNotifications
        {
            get { return (bool)this["disableNotifications"]; }
            set { this["disableNotifications"] = value; }
        }
        [ConfigurationProperty("autoAnswerOnReviews", DefaultValue = false)]
        public bool AutoAnswerOnReviews
        {
            get { return (bool)this["autoAnswerOnReviews"]; }
            set { this["autoAnswerOnReviews"] = value; }
        }
        #endregion
    }

    public class BindableSettings : ObservableObject, ISettings
    {
        #region BindableSettingsValues
        private const string AppName = "MYWFE";
        private static readonly string AppPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

        private Settings Settings;
        public BindableSettings(Settings settings)
        {
            Settings = settings;
        }

        public bool DisableIconOnBG
        {
            get { return Settings.DisableIconOnBG; }
            set
            {
                if (Settings.DisableIconOnBG != value)
                {
                    Settings.DisableIconOnBG = value;
                    onPropertyChanged();
                }
            }
        }

        public bool DisableNotifications
        {
            get { return Settings.DisableNotifications; }
            set
            {
                if (Settings.DisableNotifications != value)
                {
                    Settings.DisableNotifications = value;
                    onPropertyChanged();
                }
            }
        }

        public bool AutoAnswerOnReviews
        {
            get { return Settings.AutoAnswerOnReviews; }
            set
            {
                if (Settings.AutoAnswerOnReviews != value)
                {
                    Settings.AutoAnswerOnReviews = value;
                    onPropertyChanged();
                }
            }
        }

        public bool StartUpOnWindows
        {
            get { return Settings.StartUpOnWindows; }
            set
            {
                if (Settings.StartUpOnWindows != value)
                {
                    Settings.StartUpOnWindows = value;

                    if (value)
                    {
                        AddToStartup();
                    }
                    else
                    {

                        RemoveFromStartup();
                    }

                    onPropertyChanged();
                }
            }
        }
        #endregion
        #region StartUpMethods
        public static void AddToStartup()
        {
            try
            {
                RegistryKey Key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                if (Key != null)
                {
                    Key.SetValue(AppName, $"\"{AppPath}\"");
                    Key.Close();
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public static void RemoveFromStartup()
        {
            try
            {
                RegistryKey Key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                if (Key != null)
                {
                    Key.DeleteValue(AppName, false);
                    Key.Close();
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
        #endregion
    }
}
