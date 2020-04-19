using System;
using System.Windows;
using AtmosLibrary;

namespace AtmosCalculator
{
    /// <summary>
    /// Interaction logic for IdealGasCalculator.xaml
    /// </summary>
    public partial class IdealGasCalculator : Window
    {
        public IdealGasCalculator()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the 'solve' button is clicked for the moles tab.
        /// </summary>
        private void MoleCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double pressure = Convert.ToDouble(M_Pressure.Text);
                double temperature = Convert.ToDouble(M_Temperature.Text);
                double volume = Convert.ToDouble(M_Volume.Text);

                double moles = (pressure * volume) / (Constants.Num.IdealGas * temperature);
                MoleResult.Content = $"{moles} mol";
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Called when the 'solve' button is clicked for the volume tab.
        /// </summary>
        private void VolumeCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double pressure = Convert.ToDouble(V_Pressure.Text);
                double temperature = Convert.ToDouble(V_Temperature.Text);
                double moles = Convert.ToDouble(V_Moles.Text);

                double volume = (moles * Constants.Num.IdealGas * temperature) / pressure;
                VolumeResult.Content = $"{volume} L";
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Called when the 'solve' button is clicked for the temperature tab.
        /// </summary>
        private void TemperatureCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double pressure = Convert.ToDouble(T_Pressure.Text);
                double moles = Convert.ToDouble(T_Moles.Text);
                double volume = Convert.ToDouble(T_Volume.Text);

                double temperature = (pressure * volume) / (moles * Constants.Num.IdealGas);
                TemperatureResult.Content = $"{temperature} K";
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Called when the 'solve' button is clicked for the pressure tab.
        /// </summary>
        private void PressureCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double moles = Convert.ToDouble(P_Moles.Text);
                double temperature = Convert.ToDouble(P_Temperature.Text);
                double volume = Convert.ToDouble(P_Volume.Text);

                double pressure = (moles * Constants.Num.IdealGas * temperature) / volume;
                PressureResult.Content = $"{pressure} kPa";
            }
            catch (Exception)
            { }
        }
    }
}
