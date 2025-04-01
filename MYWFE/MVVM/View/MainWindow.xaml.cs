using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MYWFE
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StateChanged += MainWindowStateChangeRaised;
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
        private void MainWindowStateChangeRaised(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MainWindowBorder.BorderThickness = new Thickness(8);
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
                BGIcon.Width = 400;
            }
            else
            {
                MainWindowBorder.BorderThickness = new Thickness(0);
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
                BGIcon.Width = 200;
            }
        }

        private void ResizeToSmallMenu(HashSet<RadioButton> RadioBtns)
        {
            MenuColumnDef.Width = new GridLength(90);
            AccountL.Width = new GridLength(1, GridUnitType.Star);
            AccountR.Width = new GridLength(0);
            AddAccountL.Width = new GridLength(1, GridUnitType.Star);
            AddAccountR.Width = new GridLength(0);

            LogoIcon.Visibility = Visibility.Visible;
            LogoStackPanel.Visibility = Visibility.Collapsed;

            foreach (var Item in RadioBtns)
            {
                Item.Padding = new Thickness(0);
                Grid _TempGrid = Item.Content as Grid;
                _TempGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                _TempGrid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
            }
        }

        private void ResizeToBigMenu(HashSet<RadioButton> RadioBtns)
        {
            MenuColumnDef.Width = new GridLength(290);
            AccountL.Width = new GridLength(0.3, GridUnitType.Star);
            AccountR.Width = new GridLength(1, GridUnitType.Star);
            AddAccountL.Width = new GridLength(0.3, GridUnitType.Star);
            AddAccountR.Width = new GridLength(1, GridUnitType.Star);

            LogoIcon.Visibility = Visibility.Collapsed;
            LogoStackPanel.Visibility = Visibility.Visible;

            foreach (var Item in RadioBtns)
            {
                Item.Padding = new Thickness(15, 0, 0, 0);
                Grid _TempGrid = Item.Content as Grid;
                _TempGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
                _TempGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Root.ActualWidth < 1250)
            {
                ResizeToSmallMenu(new HashSet<RadioButton> { MenuHomeBtn, MenuQuestionsBtn, MenuFeedbacksBtn, MenuSettingsBtn, MenuTemplatesBtn });
            }
            else
            {
                ResizeToBigMenu(new HashSet<RadioButton> { MenuHomeBtn, MenuQuestionsBtn, MenuFeedbacksBtn, MenuSettingsBtn, MenuTemplatesBtn });
            }
        }

    }
}