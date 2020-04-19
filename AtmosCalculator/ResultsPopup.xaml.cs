using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AtmosLibrary;

namespace AtmosCalculator
{
    /// <summary>
    /// Interaction logic for ResultsPopup.xaml
    /// </summary>
    public partial class ResultsPopup : Window
    {
        public ResultsPopup()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Calculates the results of a bomb and adds it to the window.
        /// </summary>
        private void Window_Initialized(object sender, EventArgs e)
        {
            //Do pressure/temperature calculations
            try
            {
                //set some advanced info first
                Air1MolesReadout.Content = Math.Round(FinalCalculations.FinalAIR1.total_moles(), 3);
                Air2MolesReadout.Content = Math.Round(FinalCalculations.FinalAIR2.total_moles(), 3);
                //hacky shit
                bool TTV = true;
                switch (FinalCalculations.Type)
                {
                    case "TTV":
                        ContainerSpecificMerge.TTV_Merge(FinalCalculations.FinalAIR1, FinalCalculations.FinalAIR2);
                        TTV = true; //hack
                        break;
                    case "AIRPUMP":
                        ContainerSpecificMerge.AirPump_Merge(FinalCalculations.FinalAIR1, FinalCalculations.FinalAIR2, FinalCalculations.TargetPressure);
                        TTV = false;
                        break;
                    default:
                        throw new AtmosException.CalculateResultsException();
                }
                //shorthand
                GasMixture final_gm = TTV ? FinalCalculations.FinalAIR1 : FinalCalculations.FinalAIR2;
                bool verdict;
                PressureBeforeExplosionReadout.Content = Math.Round(final_gm.total_pressure(), 3);
                TempBeforeExplosionReadout.Content = Math.Round(final_gm.temperature, 3);

                double NumReacts = 0;
                if(final_gm.total_pressure() < Constants.Num.TankFragmentPresssure)
                {
                    while(final_gm.react() && final_gm.total_pressure() < Constants.Num.TankFragmentPresssure) //give it a couple tries to get above fragment pressure
                    {
                        NumReacts++;
                    }
                }
                NumReactsReadout.Content = NumReacts;
                //Now do reactions, three times
                if(final_gm.total_pressure() > Constants.Num.TankFragmentPresssure)
                {
                    Console.WriteLine("Reacting");
                    final_gm.react();
                    final_gm.react();
                    final_gm.react();
                    verdict = true;
                }
                else
                {
                    verdict = false;
                }

                //Temperature
                KelvinReadout.Content = Math.Round(final_gm.temperature, 2);
                CelsiusReadout.Content = Math.Round(final_gm.temperature - Constants.Num.ZeroCelsius, 2);
         
                //Pressure
                PressureResult.Content = Math.Round(final_gm.total_pressure(), 2);

                Dictionary<String, double> gas_percent_dict = new Dictionary<String, double>();
                foreach (KeyValuePair<BGas, double> g in final_gm.air_contents)
                {
                    if (gas_percent_dict.ContainsKey(g.Key.name))
                    {
                        gas_percent_dict[g.Key.name] += g.Value / final_gm.total_moles();
                        continue;
                    }

                    gas_percent_dict.Add(g.Key.name, g.Value / final_gm.total_moles());
                }
                //add to stackpanel n such
                foreach (KeyValuePair<String, double> item in gas_percent_dict)
                {
                    Label gas_percent_label = new Label();

                    gas_percent_label.Content = $"{Math.Round(item.Value * 100, 4)}% {item.Key}";
                    gas_percent_label.FontFamily = new FontFamily("Roboto");

                    PressureStackPanel.Children.Add(gas_percent_label);
                }
                //actual bomb stuff..
                if (verdict)
                {
                    double pressure = final_gm.total_pressure();
                    double bomb_range = (pressure - Constants.Num.TankFragmentPresssure) / Constants.Num.TankFragmentScale;
                    
                    string theoretical_range = $"{Math.Round(bomb_range / 4, 2)}/{Math.Round(bomb_range / 2, 2)}/{Math.Round(bomb_range, 2)}";
                    string real_range = bomb_range > 20 ? "5/10/20" : theoretical_range; //maxcap

                    TheoreticalRange.Content = theoretical_range;
                    RealRange.Content = real_range;
                    BombVerdict.Content = "Success!";

                    //visualization
                    //width/height mult. by 2 for the heavy, /1 for medium and /2 for light - this is because 1 tile is 2 pixels
                    //light
                    //make sure it doesnt overflow
                    if (bomb_range * 2 < MetaStationImage.Height + 192.5)
                    {
                        LightRadius.Width = bomb_range * 2;
                        LightRadius.Height = bomb_range * 2;
                        Canvas.SetLeft(LightRadius, 227.5 - (LightRadius.Width / 2));
                        Canvas.SetTop(LightRadius, 192.5 - (LightRadius.Height / 2));
                    }
                    else
                    {
                        LightRadiusRect.Width = MetaStationImage.Width;
                        LightRadiusRect.Height = MetaStationImage.Height;
                    }

                    //medium
                    if (bomb_range < MetaStationImage.Height + 192.5)
                    {
                        MediumRadius.Width = bomb_range;
                        MediumRadius.Height = bomb_range;
                        Canvas.SetLeft(MediumRadius, 227.5 - (MediumRadius.Width / 2));
                        Canvas.SetTop(MediumRadius, 192.5 - (MediumRadius.Height / 2));
                    }
                    else
                    {
                        MediumRadiusRect.Width = MetaStationImage.Width;
                        MediumRadiusRect.Height = MetaStationImage.Height;
                    }

                    //heavy
                    if (bomb_range / 2 < MetaStationImage.Height + 192.5)
                    {
                        HeavyRadius.Width = bomb_range / 2;
                        HeavyRadius.Height = bomb_range / 2;
                        Canvas.SetLeft(HeavyRadius, 227.5 - (HeavyRadius.Width / 2));
                        Canvas.SetTop(HeavyRadius, 192.5 - (HeavyRadius.Height / 2));
                    }
                    else
                    {
                        HeavyRadiusRect.Width = MetaStationImage.Width;
                        HeavyRadiusRect.Height = MetaStationImage.Height;
                    }
                }
                else
                {
                    TheoreticalRange.Content = "0/0/0";
                    RealRange.Content = "0/0/0";
                    BombVerdict.Content = "Dud...";
                }

            }
            catch(Exception egg)
            {
                Console.WriteLine(egg);
            }
        }
    }
}
