using System.Windows;

namespace AtmosCalculator
{
    /// <summary>
    /// Interaction logic for StartupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window
    {
        public StartupWindow()
        {
            InitializeComponent();
        }

        private void Bomb_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
        }

        private void IdealGas_Click(object sender, RoutedEventArgs e)
        {
            IdealGasCalculator window = new IdealGasCalculator();
            window.Show();
        }

        private void ReactionSimulator_Click(object sender, RoutedEventArgs e)
        {
            ReactionSimulator window = new ReactionSimulator();
            window.Show();
        }

        private void FusionSimulator_Click(object sender, RoutedEventArgs e)
        {
            FusionSimulator window = new FusionSimulator();
            window.Show();
        }
    }
}
