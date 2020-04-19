using System;
using System.Linq;
using System.Collections.Generic;

namespace AtmosLibrary
{
    /// <summary>
    /// Contains definitions for all functions called when GasReactions happen
    /// </summary>
    
    // TODO:
    // Add water vapor reaction fully
    // Add pluox creation reaction fully (turf-specific stuff, basically)
    // REsearch point stuff

    static class ReactionFunctions
    {
        /// <summary>
        /// Function that governs Hyper-Noblium formation
        /// </summary>
        /// <param name="air">GasMixture to be reacted</param>
        /// <returns>GasMixture after reaction</returns>
        public static bool NobliumFormation(GasMixture air)
        {
            bool reacted = false;
            double temperature = air.temperature;
            Dictionary<BGas, double> gases = air.air_contents;

            double old_heat_cap = air.heat_capacity();
            double nob_formed = Math.Min(Math.Min((gases[Constants.Gases.Nitrogen] + gases[Constants.Gases.Tritium]) / 100, gases[Constants.Gases.Tritium] / 10), gases[Constants.Gases.Nitrogen] / 20);
            double energy_taken = nob_formed * (Constants.Num.NobliumFormationEnergy / Math.Max(gases[Constants.Gases.BZ], 1));

            if((gases[Constants.Gases.Tritium] - (10 * nob_formed) < 0) || (gases[Constants.Gases.Nitrogen] - (20 * nob_formed) < 0))
            {
                return reacted; //false
            }

            gases[Constants.Gases.HyperNoblium] += nob_formed;
            gases[Constants.Gases.Tritium] -= (10 * nob_formed);
            gases[Constants.Gases.Nitrogen] -= (20 * nob_formed);

            if(nob_formed > 0)
            {
                double new_heat_capacity = air.heat_capacity();
                if(new_heat_capacity > Constants.Num.MinimumHeatCapacity)
                {
                    air.temperature = Math.Max(((air.temperature * old_heat_cap - energy_taken) / new_heat_capacity), Constants.Num.TempMinimum);
                }
            }

            return reacted;
        }
        /// <summary>
        /// Function that governs Stimulum formation
        /// </summary>
        /// <param name="air">GasMixture to be reacted</param>
        /// <returns>GasMixture after reaction</returns>
        public static bool StimulumFormation(GasMixture air)
        {
            bool reacted = false;
            double temperature = air.temperature;
            Dictionary<BGas, double> gases = air.air_contents;

            double old_heat_capacity = air.heat_capacity();
            //fuck whoever coded these reactions
            double heat_scale = Math.Min(Math.Min(Math.Min(air.temperature / Constants.Num.StimulumHeatScale, gases[Constants.Gases.Tritium]), gases[Constants.Gases.Plasma]), gases[Constants.Gases.Nitryl]);
            //fuck my life
            double stim_energy_change = (heat_scale) + (Constants.Num.StimulumFirstRise * Math.Pow(heat_scale, 2)) - (Constants.Num.StimulumFirstDrop * Math.Pow(heat_scale, 3)) + (Constants.Num.StimulumSecondRise * Math.Pow(heat_scale, 4)) - (Constants.Num.StimulumAbsoluteDrop * Math.Pow(heat_scale, 5));

            air.assert_gas(Constants.Gases.Stimulum);

            if ((gases[Constants.Gases.Tritium] - heat_scale < 0) || (gases[Constants.Gases.Plasma] - heat_scale < 0) || (gases[Constants.Gases.Nitryl] - heat_scale < 0))
            {
                return reacted; //false
            }

            gases[Constants.Gases.Stimulum] += heat_scale / 10;
            gases[Constants.Gases.Tritium] -= heat_scale;
            gases[Constants.Gases.Plasma] -= heat_scale;
            gases[Constants.Gases.Nitryl] -= heat_scale;

            if(stim_energy_change > 0)
            {
                double new_heat_cap = air.heat_capacity();
                if(new_heat_cap > Constants.Num.MinimumHeatCapacity)
                {
                    air.temperature = Math.Max(((air.temperature * old_heat_capacity + stim_energy_change) / new_heat_cap), Constants.Num.TempMinimum);
                    reacted = true;
                }
            }

            return reacted;
        }
        /// <summary>
        /// Function that governs BZ formation
        /// </summary>
        /// <param name="air">GasMixture to be reacted</param>
        /// <returns>GasMixture after reaction</returns>
        public static bool BZFormation(GasMixture air)
        {
            bool reacted = false;
            double pressure = air.total_pressure();
            double temperature = air.temperature;
            Dictionary<BGas, double> gases = air.air_contents;

            double old_heat_cap = air.heat_capacity();
            //lord have mercy
            double reaction_efficiency = Math.Min(Math.Min(1 / ((pressure / (0.1 * Constants.Num.OneAtmosphere)) * (Math.Max(gases[Constants.Gases.Plasma] / gases[Constants.Gases.Tritium], 1))), gases[Constants.Gases.Tritium]), gases[Constants.Gases.Plasma] / 2);
            double energy_released = 2 * reaction_efficiency * Constants.Num.CarbonEnergyReleased;

            if(gases.ContainsKey(Constants.Gases.Miasma) && gases[Constants.Gases.Miasma] > 0)
            {
                energy_released /= gases[Constants.Gases.Miasma] * 0.1;
            }
            if(gases.ContainsKey(Constants.Gases.BZ) && gases[Constants.Gases.BZ] > 0)
            {
                energy_released *= gases[Constants.Gases.BZ] * 0.1;
            }

            if((gases[Constants.Gases.Tritium] - reaction_efficiency < 0) || (gases[Constants.Gases.Plasma] - (2 * reaction_efficiency) < 0))
            {
                return reacted; //false
            }
            air.assert_gas(Constants.Gases.BZ);
            gases[Constants.Gases.BZ] += reaction_efficiency;
            if(reaction_efficiency == gases[Constants.Gases.NitrousOxide])
            {
                air.assert_gas(Constants.Gases.Oxygen);
                gases[Constants.Gases.BZ] -= Math.Min(pressure, 1);
                gases[Constants.Gases.Oxygen] += Math.Min(pressure, 1);
            }
            gases[Constants.Gases.NitrousOxide] -= reaction_efficiency;
            gases[Constants.Gases.Plasma] -= (2 * reaction_efficiency);

            if(energy_released > 0)
            {
                double new_heat_cap = air.heat_capacity();
                if (new_heat_cap > Constants.Num.MinimumHeatCapacity)
                {
                    air.temperature = Math.Max(((temperature * old_heat_cap + energy_released) / new_heat_cap), Constants.Num.TempMinimum);
                    reacted = true;
                }
            }

            return reacted;
        }
        /// <summary>
        /// Function that governs Nitryl formation
        /// </summary>
        /// <param name="air">GasMixture to be reacted</param>
        /// <returns>GasMixture after reaction</returns>
        public static bool NitrylFormation(GasMixture air)
        {
            bool reacted = false;
            double temperature = air.temperature;
            double old_heat_capacity = air.heat_capacity();
            Dictionary<BGas, double> gases = air.air_contents;

            double heat_efficiency = Math.Min(Math.Min(temperature / (Constants.Num.FireMinimumTemperature * 100), gases[Constants.Gases.Oxygen]), gases[Constants.Gases.Nitrogen]); //another weird three-way min to conserve matter
            double energy_used = heat_efficiency * Constants.Num.NitrylFormationEnergy;
            air.assert_gas(Constants.Gases.Nitryl);

            if((gases[Constants.Gases.Oxygen] - heat_efficiency < 0) || (gases[Constants.Gases.Nitrogen] - heat_efficiency < 0))
            {
                return reacted; //false
            }
            gases[Constants.Gases.Oxygen] -= heat_efficiency;
            gases[Constants.Gases.Nitrogen] -= heat_efficiency;
            gases[Constants.Gases.Nitryl] += heat_efficiency * 2;

            if(energy_used > 0)
            {
                double new_heat_capacity = air.heat_capacity();
                if(new_heat_capacity > Constants.Num.MinimumHeatCapacity)
                {
                    air.temperature = Math.Max(((temperature * old_heat_capacity - energy_used) / new_heat_capacity), Constants.Num.TempMinimum);
                    reacted = true;
                }
            }

            return reacted;
        }
        /// <summary>
        /// Guess who's back
        /// Back again
        /// Fusion's back
        /// Tell a friend
        /// </summary>
        /// <param name="air">GasMixture to be reacted</param>
        /// <returns>GasMixture after reaction</returns>
        public static bool Fusion(GasMixture air)
        {
            double energy_released = 0;
            double old_heat_capacity = air.heat_capacity();
            double temperature = air.temperature;
            var gases = air.air_contents;

            var initial_plasma = gases[Constants.Gases.Plasma];
            var initial_carbon = gases[Constants.Gases.CarbonDioxide];

            var scale_factor = (air.volume) / (Math.PI);
            var radians = Helpers.ToRadians(Math.Atan((air.volume - Constants.Num.ToroidVolumeBreakeven) / Constants.Num.ToroidVolumeBreakeven));
            var toroidal_size = (2 * Math.PI) + radians;
            double gas_power = 0;

            foreach(var kvp in gases)
            {
                gas_power += (kvp.Key.fusion_power * kvp.Value);
            }

            var instability = (Math.Pow(gas_power * Constants.Num.InstabilityGasPowerFactor, 2)) % toroidal_size;

            var plasma = (initial_plasma - Constants.Num.FusionMoleThreshold) / (scale_factor);
            var carbon = (initial_carbon - Constants.Num.FusionMoleThreshold) / (scale_factor);

            plasma = (plasma - (instability * Math.Sin(Helpers.ToDegrees(carbon)))) % toroidal_size;
            carbon = (carbon - plasma) % toroidal_size;

            gases[Constants.Gases.Plasma] = plasma * scale_factor + Constants.Num.FusionMoleThreshold;
            gases[Constants.Gases.CarbonDioxide] = carbon * scale_factor + Constants.Num.FusionMoleThreshold;
            var delta_plasma = initial_plasma - gases[Constants.Gases.Plasma];

            energy_released += delta_plasma * Constants.Num.PlasmaBindingEnergy;
            if (instability < Constants.Num.FusionInstabilityEndothermality)
                energy_released = Math.Max(energy_released, 0);
            else if (energy_released < 0)
                energy_released *= Math.Pow(instability - Constants.Num.FusionInstabilityEndothermality, 0.5);

            if(air.thermal_energy() + energy_released < 0)
            {
                gases[Constants.Gases.Plasma] = initial_plasma;
                gases[Constants.Gases.CarbonDioxide] = initial_carbon;
                return false;
            }

            gases[Constants.Gases.Tritium] -= Constants.Num.FusionTritiumMolesUsed;

            if(energy_released > 0)
            {
                air.assert_gas(Constants.Gases.Oxygen);
                air.assert_gas(Constants.Gases.NitrousOxide);
                gases[Constants.Gases.Oxygen] += Constants.Num.FusionTritiumMolesUsed * (energy_released * Constants.Num.FusionTritConversionCoeff);
                gases[Constants.Gases.NitrousOxide] += Constants.Num.FusionTritiumMolesUsed * (energy_released * Constants.Num.FusionTritConversionCoeff);
            }
            else
            {
                air.assert_gas(Constants.Gases.BZ);
                air.assert_gas(Constants.Gases.Nitryl);
                gases[Constants.Gases.BZ] += Constants.Num.FusionTritiumMolesUsed * (energy_released * -Constants.Num.FusionTritConversionCoeff);
                gases[Constants.Gases.Nitryl] += Constants.Num.FusionTritiumMolesUsed * (energy_released * -Constants.Num.FusionTritConversionCoeff);
            }

            if(energy_released > 0)
            {
                var new_heatcap = air.heat_capacity();
                if (new_heatcap > Constants.Num.MinimumHeatCapacity)
                    air.temperature = Helpers.Clamp((air.temperature * old_heat_capacity + energy_released) / new_heatcap, Constants.Num.TempMinimum, double.PositiveInfinity);
            }

            return true;
        }

        /// <summary>
        /// Function that governs hydrocarbon combustion
        /// </summary>
        /// <param name="air">GasMixture to be reacted</param>
        /// <returns>GasMixture after reaction</returns>
        public static bool Fire(GasMixture air)
        {
            bool reacted = false;
            double energy_released = 0;
            double old_heat_capacity = air.heat_capacity();
            double temperature = air.temperature;
            var gases = air.air_contents;

            //tritium gas burn
            if(gases.ContainsKey(Constants.Gases.Tritium) && gases[Constants.Gases.Tritium] > 0)
            {
                double burned_fuel;

                if(!gases.ContainsKey(Constants.Gases.Oxygen)) //does oxygen exist in the gases?
                {
                    burned_fuel = 0;
                }
                else if(gases[Constants.Gases.Oxygen] < gases[Constants.Gases.Tritium] || air.thermal_energy() < Constants.Num.MinimumTritOxyBurnEnergy) //if it does, is there less of it than tritium?
                {
                    burned_fuel = gases[Constants.Gases.Oxygen] / Constants.Num.TritiumBurnOxyFactor;
                    gases[Constants.Gases.Tritium] -= burned_fuel;
                }
                else //okay, so theres more tritium than oxygen
                {
                    burned_fuel = gases[Constants.Gases.Tritium] * Constants.Num.TritiumBurnTritFactor;
                    gases[Constants.Gases.Tritium] -= gases[Constants.Gases.Tritium] / Constants.Num.TritiumBurnTritFactor;
                    gases[Constants.Gases.Oxygen] -= gases[Constants.Gases.Tritium];
                }

                if(burned_fuel > 0)
                {
                    energy_released += Constants.Num.HydrogenEnergyReleased * burned_fuel;

                    air.assert_gas(Constants.Gases.WaterVapor);
                    gases[Constants.Gases.WaterVapor] += burned_fuel / Constants.Num.TritiumBurnOxyFactor;

                    reacted = true;
                }
            }

            //plasma gas burn
            if(gases.ContainsKey(Constants.Gases.Plasma) && gases[Constants.Gases.Plasma] > Constants.Num.MinimumHeatCapacity)
            {
                double plasma_burn_rate;
                double oxygen_burn_rate;

                double temperature_scale;
                bool super_saturation = false;

                if(temperature > Constants.Num.PlasmaUpperTemperature)
                {
                    temperature_scale = 1;
                }
                else
                {
                    //fun fact
                    //this line of code might be the worst line of code in the game
                    //because of order of operations and how #define works in byond,
                    //UPPER_TEMPERATURE was defined as 1370 + T0C (T0C is 273.15)
                    //no brackets.
                    //this means that it was replacing every mention of it with that exact statement, no brackets
                    //this fucked with the order of operations
                    //causing ALL OF PLASMA FIRE CODE TO HAVE BEEN INCORRECT FOR THE PAST 10 YEARS
                    //IT'S BEEN IN THE GAME FOR 10 YEARS AND I ACCIDENTALLY FOUND OUT ABOUT IT AND GOT IT FIXED
                    //NOW ALL PLASMA BOMBS ARE SMALLER THAN THEY SHOULD BE AND IM FUCKED
                    //THIS IS WHAT IT SHOULD BE: temperature_scale = (temperature - Constants.Num.FireMinimumTemperature) / (Constants.Num.PlasmaUpperTemperature - Constants.Num.FireMinimumTemperature);
                    //AND THIS IS WHAT IT USED TO BE
                    // temperature_scale = (temperature + 173.15) / (Constants.Num.PlasmaUpperTemperature + 173.15);
                    //AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
                    temperature_scale = (temperature - Constants.Num.FireMinimumTemperature) / (Constants.Num.PlasmaUpperTemperature - Constants.Num.FireMinimumTemperature);
                }
                if(temperature_scale > 0)
                {
                    double oxygen_moles = gases.ContainsKey(Constants.Gases.Oxygen) ? gases[Constants.Gases.Oxygen] : 0;
                    oxygen_burn_rate = Constants.Num.OxyBurnRateBase - temperature_scale;

                    if(oxygen_moles / gases[Constants.Gases.Plasma] > Constants.Num.SuperSaturationThreshold)
                    {
                        super_saturation = true;
                    }
                    if (oxygen_moles > gases[Constants.Gases.Plasma] * Constants.Num.PlasmaOxyFullburn) //fun fact: in ss13 code this block was here twice. atmos code is flawless
                    {
                        plasma_burn_rate = (gases[Constants.Gases.Plasma] * temperature_scale) / Constants.Num.PlasmaBurnRateDelta;
                    }
                    else
                    {
                        plasma_burn_rate = (temperature_scale * (oxygen_moles / Constants.Num.PlasmaOxyFullburn)) / Constants.Num.PlasmaBurnRateDelta;
                    }

                    if(plasma_burn_rate > Constants.Num.MinimumHeatCapacity)
                    {
                        plasma_burn_rate = Math.Min(Math.Min(plasma_burn_rate, gases[Constants.Gases.Plasma]), oxygen_moles / oxygen_burn_rate); //weird three-way min in ss13 code

                        gases[Constants.Gases.Plasma] -= plasma_burn_rate;
                        gases[Constants.Gases.Oxygen] -= (plasma_burn_rate * oxygen_burn_rate);

                        if(super_saturation)
                        {
                            air.assert_gas(Constants.Gases.Tritium);
                            gases[Constants.Gases.Tritium] += plasma_burn_rate;
                        }
                        else
                        {
                            air.assert_gas(Constants.Gases.CarbonDioxide);
                            gases[Constants.Gases.CarbonDioxide] += plasma_burn_rate;
                        }

                        energy_released += Constants.Num.PlasmaEnergyReleased * plasma_burn_rate;

                        reacted = true;
                    }
                }
            }

            //change temperature now
            if(energy_released > 0)
            {
                double new_heat_capacity = air.heat_capacity();
                if(new_heat_capacity > Constants.Num.MinimumHeatCapacity)
                {
                    air.temperature = ((temperature * old_heat_capacity) + energy_released) / new_heat_capacity;
                    Console.WriteLine($"New Temp: {air.temperature}");
                }
            }

            return reacted;
        }

        public static bool Sterilization(GasMixture air)
        {
            var gases = air.air_contents;
            if(gases.ContainsKey(Constants.Gases.WaterVapor) && (gases[Constants.Gases.WaterVapor] / air.total_moles()) > 0.1)
            {
                return false;
            }

            var cleaned_air = Math.Min(gases[Constants.Gases.Miasma], 20 + (air.temperature - Constants.Num.FireMinimumTemperature - 70) / 20);
            gases[Constants.Gases.Miasma] -= cleaned_air;
            air.assert_gas(Constants.Gases.Oxygen);
            gases[Constants.Gases.Oxygen] += cleaned_air;

            air.temperature += cleaned_air * 0.002;

            return true;
        }
    }
}
