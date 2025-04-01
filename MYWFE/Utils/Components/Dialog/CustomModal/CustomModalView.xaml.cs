using System.Diagnostics;
using System.Windows.Controls;

namespace MYWFE.Utils.Components.Dialog.CustomModal
{
    /// <summary>
    /// Логика взаимодействия для CustomModalView.xaml
    /// </summary>
    public partial class CustomModalView : UserControl
    {
        public CustomModalView()
        {
            InitializeComponent();
        }

        private void StackPanel_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string howtoget = "https://seller.wildberries.ru/supplier-settings/access-to-api";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {howtoget}") { CreateNoWindow = true });
        }
    }
}
