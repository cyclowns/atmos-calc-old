using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AtmosLibrary;

namespace AtmosCalculator
{
    /// <summary>
    /// Initializes and contains the final gas mixtures used in bomb-related calculations
    /// </summary>
    public static class FinalCalculations
    {
        /// <summary>
        /// Final GasMixture AIR1 used for calculations of temperature, pressure, and bomb strength in <see cref="ResultsPopup"/>
        /// </summary>
        public static GasMixture FinalAIR1 = new GasMixture(null, 0, 1);
        /// <summary>
        /// Final GasMixture AIR2 used for calculations of temperature, pressure, and bomb strength in <see cref="ResultsPopup"/>
        /// </summary>
        public static GasMixture FinalAIR2 = new GasMixture(null, 0, 1);
        /// <summary>
        /// String containing the type of merge to be performed by ResultsPopup.xaml.cs. Either TTV, Canister, or AirPump. Determined by which tab the user is in when entering data.
        /// </summary>
        public static string Type = "TTV"; //ttv by default
        /// <summary>
        /// Double corresponding to the target pressure of canister/airpump, if the user has that option selected
        /// </summary>
        public static double TargetPressure = 0; //0 by default
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        //taken from msdn/SO, ty!
        /// <summary>
        /// Extension method for a FrameworkElement that searches for a child element by type and name.
        /// </summary>
        /// <typeparam name="T">The type of the child element to search for.</typeparam>
        /// <param name="element">The parent framework element.</param>
        /// <param name="sChildName">The name of the child element to search for.</param>
        /// <returns>The matching child element, or null if none found.</returns>
        private T FindElementByName<T>(FrameworkElement element, string sChildName) where T : FrameworkElement
        {
            T childElement = null;
            var nChildCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < nChildCount; i++)
            {
                FrameworkElement child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                if (child == null)
                    continue;

                if (child is T && child.Name.Equals(sChildName))
                {
                    childElement = (T)child;
                    break;
                }

                childElement = FindElementByName<T>(child, sChildName);

                if (childElement != null)
                    break;
            } 
            return childElement;
        }

        /// <summary>
        /// Loops through a <see cref="StackPanel"/> corresponding to tank_number and checks to see if it has the correct gas list format.
        /// If it does, LoopThroughGasStackPanel will convert it to a <see cref="Gas"/> list and return it.
        /// </summary>
        /// <param name="given_stack">StackPanel to loopthrough</param>
        /// <param name="pressure_textbox">TextBox to check for pressure calculations</param>
        /// <param name="temperature_textbox">TextBox to check for temperature calculations</param>
        /// <param name="volume">Volume of the tank/mixture</param>
        /// <returns>List<Gas> containing converted contents of StackPanel</returns>
        private Dictionary<BGas, double> GetGasList(StackPanel stack_panel, TextBox temperature_textbox, TextBox pressure_textbox, int volume)
        {
            Dictionary<BGas, double> returned_list = new Dictionary<BGas, double>();
            //used later
            double percent_final = 0;

            foreach(ContentControl content_control in stack_panel.Children)
            {
                ComboBox combo_box = FindElementByName<ComboBox>(content_control, "GasListItemComboBox");
                string GasName = combo_box.Text;

                BGas found = Constants.Gases.GasList.Find(x => x.name == GasName);

                if (found)
                {
                    TextBox percent_textbox = FindElementByName<TextBox>(content_control, "GasListPercentTextBox");

                    try
                    {
                        double percent = Math.Max(Convert.ToDouble(percent_textbox.Text), 0); //causes dividebyzero errors if its lower than 0, which is caught
                        double temperature = Convert.ToDouble(temperature_textbox.Text);
                        double pressure = Math.Max(Convert.ToDouble(pressure_textbox.Text), 0);
                        //calculate what mole count should be based on percentage
                        double mole_count = ((pressure * volume) / (temperature * Constants.Num.IdealGas)) / (100 / percent);

                        returned_list.Add(found, mole_count);

                        percent_final += percent;
                    }
                    catch(Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            if(Math.Round(percent_final, 1) != 100) //account for the fact that ss13 often shows inaccurate percentage, usually 0.01 off
            {
                throw new AtmosException.GasPercentException("Gas percentages do not add up to 100%");
            }
            return returned_list;
        }
        /// <summary>
        /// Fires when the "Add Tank" button is clicked in the UI for Tank 1.
        /// </summary>
        private void TANK1AddTank()
        {
            try
            {
                double temperature = Convert.ToDouble(TANK1TemperatureTextBox.Text);
                Dictionary<BGas, double> air_contents = GetGasList(TANK1GasStackPanel, TANK1TemperatureTextBox, TANK1PressureTextBox, Constants.Num.TankVolume);
                //add to tank one
                FinalCalculations.FinalAIR1.temperature = temperature;
                FinalCalculations.FinalAIR1.air_contents = air_contents;
                FinalCalculations.FinalAIR1.volume = Constants.Num.TankVolume;
                //never threw
                OutputText.Content = "[AddTank1]: Successful!";
            }
            catch(Exception ex)
            {
                OutputText.Content = $"[AddTank1]: {ex.Message} ({ex.GetType().Name})";
            }
        }
        /// <summary>
        /// Fires when the "Add Tank" button is clicked in the UI for Tank 2.
        /// </summary>
        private void TANK2AddTank()
        {
            try
            {
                double temperature = Math.Max(Convert.ToDouble(TANK2TemperatureTextBox.Text), 0);
                Dictionary<BGas, double> air_contents = GetGasList(TANK2GasStackPanel, TANK2TemperatureTextBox, TANK2PressureTextBox, Constants.Num.TankVolume);
                //add to tank two
                FinalCalculations.FinalAIR2.temperature = temperature;
                FinalCalculations.FinalAIR2.air_contents = air_contents;
                FinalCalculations.FinalAIR2.volume = Constants.Num.TankVolume;
                //never threw
                OutputText.Content = "[AddTank2]: Successful!";
            }
            catch (Exception ex)
            {
                OutputText.Content = $"[AddTank2]: {ex.Message} ({ex.GetType().Name})";
            }
        }

        /// <summary>
        /// Fires when the "Add Gas" button is clicked in the UI for Tank 1. Creates a new ContentControl with gas info.
        /// </summary>
        private void TANK1AddGasButton_Click(object sender, RoutedEventArgs e)
        {
            if (TANK1GasStackPanel.Children.Count < 12) //number of gases, zero-initalized
            {
                ContentControl gas_item = new ContentControl
                {
                    Template = this.Resources["GasItem"] as ControlTemplate,
                    Name = $"TANK1GasListItem{TANK1GasStackPanel.Children.Count + 2}" //Count is zero-initialized
                };
                TANK1GasStackPanel.Children.Add(gas_item);
            }
        }

        /// <summary>
        /// Fires when the "Add Gas" button is clicked in the UI for Tank 2. Creates a new ContentControl with gas info.
        /// </summary>
        private void TANK2AddGasButton_Click(object sender, RoutedEventArgs e)
        {
            if(TANK2GasStackPanel.Children.Count < 12) //number of gases, zero-initalized
            {
                ContentControl gas_item = new ContentControl
                {
                    Template = this.Resources["GasItem"] as ControlTemplate,
                    Name = $"TANK2GasListItem{TANK2GasStackPanel.Children.Count + 2}" //Count is zero-initialized
                };
                TANK2GasStackPanel.Children.Add(gas_item);
            }
        }

        /// <summary>
        /// Fires when the Calculate Button is clicked
        /// </summary>
        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            //change type
            FinalCalculations.Type = "TTV";
            //call addtank
            TANK1AddTank();
            TANK2AddTank();
            //calculations are done in ResultsPopup.xaml.cs
            ResultsPopup popup = new ResultsPopup();
            try //this fails if the popup closes before its ever shown (i.e, the calculation dun fucked up and it closed itself)
            {
                popup.Show();
            }
            catch(Exception ex)
            {
                OutputText.Content = "[Calculate] Calculation failed, did you add your tanks correctly?";
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// yes
        /// </summary>
        private void CreditsButton_Click(object sender, RoutedEventArgs e)
        {
            CreditsWindow window = new CreditsWindow();
            window.Show();
        }

        /// <summary>
        /// Called when the 'Calculate' button for an air pump is clicked and is executed.
        /// </summary>
        private void AirPumpAddTank()
        {
            try
            {
                double temperature = Math.Max(Convert.ToDouble(AirPumpTemperatureTextBox.Text), 0);
                Dictionary<BGas, double> air_contents = GetGasList(AirPumpGasStackPanel, AirPumpTemperatureTextBox, AirPumpPressureTextBox, Constants.Num.AirPumpVolume);
                //add to tank
                FinalCalculations.FinalAIR1.temperature = temperature;
                FinalCalculations.FinalAIR1.air_contents = air_contents;
                FinalCalculations.FinalAIR1.volume = Constants.Num.AirPumpVolume;
                //never threw
                AirPumpOutputText.Content = "[AddPump]: Successful!";
            }
            catch (Exception ex)
            {
                AirPumpOutputText.Content = $"[AddPump]: {ex.Message} ({ex.GetType().Name})";
            }
        }

        /// <summary>
        /// Called when the 'Calculate' button for an air pump is clicked and is executed.
        /// </summary>
        private void AirPumpTANKAddTank()
        {
            try
            {
                double temperature = Math.Max(Convert.ToDouble(AirPumpTANKTemperatureTextBox.Text), 0);
                Dictionary<BGas, double> air_contents = GetGasList(AirPumpTANKGasStackPanel, AirPumpTANKTemperatureTextBox, AirPumpTANKPressureTextBox, Constants.Num.TankVolume);
                //add to tank two
                FinalCalculations.FinalAIR2.temperature = temperature;
                FinalCalculations.FinalAIR2.air_contents = air_contents;
                FinalCalculations.FinalAIR2.volume = Constants.Num.TankVolume;
                //target pressure
                FinalCalculations.TargetPressure = Convert.ToDouble(AirPumpTargetPressureTextBox.Text);
                Console.WriteLine($"Target pressure is {FinalCalculations.TargetPressure}");
                //never threw
                AirPumpOutputText.Content = "[AddPumpTank]: Successful!";
            }
            catch (Exception ex)
            {
                AirPumpOutputText.Content = $"[AddPumpTank]: {ex.Message} ({ex.GetType().Name})";
            }
        }

        /// <summary>
        /// Called when the 'Add Gas' button is clicked for an Air Pump.
        /// </summary>
        private void AirPumpAddGasButton_Click(object sender, RoutedEventArgs e)
        {
            if (AirPumpGasStackPanel.Children.Count < 12) //number of gases, zero-initalized
            {
                ContentControl gas_item = new ContentControl
                {
                    Template = this.Resources["GasItem"] as ControlTemplate,
                    Name = $"AirPumpGasListItem{AirPumpGasStackPanel.Children.Count + 2}" //Count is zero-initialized
                };
                AirPumpGasStackPanel.Children.Add(gas_item);
            }
        }

        /// <summary>
        /// Called when the 'Add Gas' button is clicked for an air pump's holding tank.
        /// </summary>
        private void AirPumpTANKAddGasButton_Click(object sender, RoutedEventArgs e)
        {
            if (AirPumpTANKGasStackPanel.Children.Count < 12) //number of gases, zero-initalized
            {
                ContentControl gas_item = new ContentControl
                {
                    Template = this.Resources["GasItem"] as ControlTemplate,
                    Name = $"AirPumpTANKGasListItem{AirPumpTANKGasStackPanel.Children.Count + 2}" //Count is zero-initialized
                };
                AirPumpTANKGasStackPanel.Children.Add(gas_item);
            }
        }

        /// <summary>
        /// Called when the 'Calculate' button is clicked for an air pump.
        /// </summary>
        private void AirPumpCalculateButton_Click(object sender, RoutedEventArgs e)
        {
            FinalCalculations.Type = "AIRPUMP";
            AirPumpAddTank();
            AirPumpTANKAddTank();

            //calculations are done in ResultsPopup.xaml.cs
            ResultsPopup popup = new ResultsPopup();
            try //this fails if the popup closes before its ever shown (i.e, the calculation dun fucked up and it closed itself)
            {
                popup.Show();
            }
            catch (Exception ex)
            {
                AirPumpOutputText.Content = "[AirPumpCalculate] Calculation failed, did you add your tanks correctly?";
                Console.WriteLine(ex.Message);
            }
        }
    }
}
