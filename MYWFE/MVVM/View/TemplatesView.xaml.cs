using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace MYWFE.MVVM.View
{
    public partial class TemplatesView : UserControl
    {
        public TemplatesView()
        {
            InitializeComponent();
        }
        private void TextBoxWatcher(object sender, EventArgs e)
        {
            TextBox Textbox = sender as TextBox;
            if (Textbox != null)
            {
                string pattern = @"^([1-5]([-#][1-5])*)?$";

                if (!Regex.IsMatch(Textbox.Text, pattern))
                {
                    Textbox.Text = Regex.Replace(Textbox.Text, @"[^1-5#-]", "");
                    string[] parts = Textbox.Text.Split(['#', '-'], StringSplitOptions.RemoveEmptyEntries);
                    foreach (string part in parts)
                    {
                        if (!Regex.IsMatch(part, "^[1-5]$"))
                        {
                            Textbox.Text = "";
                            break;
                        }
                    }
                    Textbox.SelectionStart = Textbox.Text.Length;
                }
            }
        }
    }
}
