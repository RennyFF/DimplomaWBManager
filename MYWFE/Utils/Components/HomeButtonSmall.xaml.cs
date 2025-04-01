using System.Windows;
using System.Windows.Controls;

namespace MYWFE.Utils.Components
{
    /// <summary>
    /// Логика взаимодействия для HomeSmall.xaml
    /// </summary>
    public partial class HomeButtonSmall : UserControl
    {
        public MahApps.Metro.IconPacks.PackIconPhosphorIconsKind Icon
        {
            get { return (MahApps.Metro.IconPacks.PackIconPhosphorIconsKind)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(MahApps.Metro.IconPacks.PackIconPhosphorIconsKind), typeof(HomeButtonSmall), new PropertyMetadata(MahApps.Metro.IconPacks.PackIconPhosphorIconsKind.WarningCircleFill));

        public string TextInside
        {
            get { return (string)GetValue(TextInsideProperty); }
            set { SetValue(TextInsideProperty, value); }
        }
        public static readonly DependencyProperty TextInsideProperty =
            DependencyProperty.Register("TextInside", typeof(string), typeof(HomeButtonSmall), new PropertyMetadata("Ошибка"));

        public bool UseAnimation
        {
            get { return (bool)GetValue(UseAnimationProperty); }
            set { SetValue(UseAnimationProperty, value); }
        }

        public static readonly DependencyProperty UseAnimationProperty =
        DependencyProperty.Register("UseAnimation", typeof(bool), typeof(HomeButtonSmall), new PropertyMetadata(true));

        public HomeButtonSmall()
        {
            InitializeComponent();
        }
    }
}
