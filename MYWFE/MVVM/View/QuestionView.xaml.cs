using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace MYWFE.MVVM.View
{
    public partial class QuestionView : UserControl
    {
        public QuestionView()
        {
            InitializeComponent();
        }

        private void TextBlock_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string _updatedText = "(скопирован)";
            Run _runElement = (sender as Run);
            if (_runElement.Text == string.Empty || _runElement.Text == _updatedText)
                return;
            else
            {
                try
                {
                    Clipboard.SetText(_runElement.Text);
                    _runElement.Text = _updatedText;
                    _runElement.Cursor = Cursors.Arrow;
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }
    }
}
