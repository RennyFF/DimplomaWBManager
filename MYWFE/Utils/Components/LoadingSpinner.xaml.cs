using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MYWFE.Utils.Components
{
    /// <summary>
    /// Логика взаимодействия для LoadingSpinner.xaml
    /// </summary>
    public partial class LoadingSpinner : UserControl
    {
        public double Diameter
        {
            get { return (double)GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }
        public static readonly DependencyProperty DiameterProperty = 
            DependencyProperty.Register("Diameter", typeof(double), typeof(LoadingSpinner), new PropertyMetadata(100.0));

        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(LoadingSpinner), new PropertyMetadata(1.0));

        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(LoadingSpinner), new PropertyMetadata(Brushes.Black));

        public PenLineCap Cap
        {
            get { return (PenLineCap)GetValue(CapProperty); }
            set { SetValue(CapProperty, value); }
        }
        public static readonly DependencyProperty CapProperty =
            DependencyProperty.Register("Cap", typeof(PenLineCap), typeof(LoadingSpinner), new PropertyMetadata(PenLineCap.Flat));

        public LoadingSpinner()
        {
            InitializeComponent();
        }
    }
}
