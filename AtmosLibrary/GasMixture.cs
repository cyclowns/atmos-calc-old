using System;
using System.Linq;
using System.Collections.Generic;

namespace AtmosLibrary
{
    /// <summary>
    /// Definition of GasMixture class, which is used to handle all atmospheric calculations and gases.
    /// </summary>
    public class GasMixture
    { 
        /// <summary>
        /// List of all gases contained in the entire gas mixture.
        /// BGas is the base gas, double is # of moles
        /// </summary>
        public Dictionary<BGas, double> air_contents { get; set; }
        /// <summary>
        /// Total temperature (in kelvin) of entire gas mixture
        /// </summary>
        public double temperature { get; set; }
        /// <summary>
        /// Volume (in liters) of entire gas mixture
        /// </summary>
        public int volume { get; set; }

        /// <summary>
        /// Basic constructor for GasMixture class.
        /// </summary>
        /// <param name="_air_contents">Dictionary of all gases/moles</param>
        /// <param name="_temperature">Temperature of gas mixture (kelvin)</param>
        /// <param name="_volume">Volume (liters) of gas mixture</param>
        public GasMixture(Dictionary<BGas, double> _air_contents, double _temperature, int _volume)
        {
            if(_volume == 0)
            {
                throw new AtmosException.VolumeZeroException("Attempted gas mixture creation with zero volume");
            }
            if(_temperature < 0)
            {
                throw new AtmosException.NegativeException("Temperature in GasMixture creation less than absolute zero");
            }

            air_contents = _air_contents;
            temperature = _temperature;
            volume = _volume;
        }
        /// <summary>
        /// Definition of method that calculates heat capacity for the gas mixture.
        /// Not to be confused with a gases individual specific heat.
        /// </summary>
        /// <returns>Double containing heat capacity of gas mixture</returns>
        public double heat_capacity()
        {
            double heat_capacity = 0;

            foreach(KeyValuePair<BGas, double> g in air_contents)
            {
                heat_capacity += (g.Value * g.Key.specific_heat);
            }

            return heat_capacity;
        }

        /// <summary>
        /// Definition of method that calculates thermal energy for the gas mixture
        /// </summary>
        /// <returns>Thermal energy of the gas mixture</returns>
        public double thermal_energy()
        {
            return temperature * heat_capacity();
        }

        /// <summary>
        /// Definition of method that adds up mole count for each gas in air_contents.
        /// </summary>
        /// <returns>Double containing total mole count of air_contents</returns>
        public double total_moles()
        {
            double mole_count = 0;

            foreach(KeyValuePair<BGas, double> g in air_contents)
            {
                mole_count += g.Value;
            }

            return mole_count;
        }

        /// <summary>
        /// Definition of method that calculates pressure using the Ideal Gas Law.
        /// </summary>
        /// <returns>Double containing pressure count for gas mixture at that instance</returns>
        public double total_pressure()
        {
            return total_moles() * Constants.Num.IdealGas * (temperature / volume);
        }

        /// <summary>
        /// Adds Gas g to air_contents at the end.
        /// Also checks for if the gas with same BGas already exists, in which case it just adds moles.
        /// Removal of gases is not required as the mole count is simply set to 0.
        /// </summary>
        /// <param name="gas">Gas to be added to air_contents</param>
        public void add_gas(KeyValuePair<BGas, double> gas)
        {
            if(!gas.Key || gas.Value < 0)
            {
                throw new ArgumentNullException("Null gas passed to add_gas()");
            }

            if(air_contents.ContainsKey(gas.Key) && air_contents[gas.Key] > 0)
            {
                air_contents[gas.Key] += gas.Value;
            }
            else
            {
                air_contents.Add(gas.Key, gas.Value);
            }
        }

        /// <summary>
        /// Adds gas to air_contents with mole count 0.
        /// </summary>
        /// <param name="gas">BGas to be added.</param>
        public void assert_gas(BGas gas)
        {
            if(!gas)
            {
                throw new ArgumentNullException("Null bgas passed to assert_gas");
            }
            if(!air_contents.ContainsKey(gas))
            {
                KeyValuePair<BGas, double> new_gas = new KeyValuePair<BGas, double>(gas, 0);
                add_gas(new_gas);
            }
        }

        /// <summary>
        /// Removes a number of moles from the GasMixture and returns a GasMixture containing them
        /// </summary>
        /// <param name="moles"># moles to remove</param>
        /// <returns>GasMixture containing the removed quanitty</returns>
        public GasMixture remove(double moles)
        {
            if(moles <= 0)
            {
                throw new AtmosException.NegativeException("Mole count in remove() less than zero");
            }

            moles = Math.Min(moles, total_moles()); //cant take more moles than exist
            GasMixture removed = new GasMixture(new Dictionary<BGas, double>(), temperature, volume);

            //weird iteration stuff with dictionaries, something to do with out of sync errors yaddah yaddah
            //i just copied it from msdn basically
            foreach(BGas g in air_contents.Keys.ToList())
            {
                //clusterfuck
                removed.add_gas(new KeyValuePair<BGas, Double>(g, air_contents[g]));

                removed.air_contents[g] = (air_contents[g] / total_moles()) * moles;
                air_contents[g] -= removed.air_contents[g];
            }

            return removed;
        }

        /// <summary>
        /// Merges this gas mixture and a giver gas mixture
        /// </summary>
        /// <param name="giver">GasMixture to merge with</param>
        public void merge(GasMixture giver)
        {
            if (!giver)
            {
                throw new ArgumentNullException("Null GasMixture passed to merge()");
            }

            if (Math.Abs(temperature - giver.temperature) > Constants.Num.MinimumTempDelta)
            {
                double self_heat_capacity = heat_capacity();
                double giver_heat_capacity = giver.heat_capacity();

                double combined_heat_capacity = self_heat_capacity + giver_heat_capacity;
                if (combined_heat_capacity > 0)
                {
                    temperature = ((giver.temperature * giver_heat_capacity) + (temperature * self_heat_capacity)) / combined_heat_capacity;
                }
            }

            foreach (KeyValuePair<BGas, double> g in giver.air_contents)
            {
                add_gas(g);
            }
        }

        /// <summary>
        /// Performs a round of reactions in the gas mixture and updates values accordingly.
        /// </summary>
        /// <returns>True if reaction succeeded at least once</returns>
        public bool react()
        {
            bool reacted = false;
            foreach (GasReaction gr in Constants.Reactions.ReactionList)
            {
                Dictionary<string, string> reqs = gr.requirements;

                //temp/ener checking
                Console.WriteLine($"Thermal energy: {thermal_energy()}");
                if ((reqs.ContainsKey("TEMP") && temperature < Convert.ToDouble(reqs["TEMP"])) || (reqs.ContainsKey("ENER") && thermal_energy() < Convert.ToDouble(reqs["ENER"])))
                {
                    continue;
                }
                //gas checking
                foreach (KeyValuePair<string, string> kvp in reqs)
                {
                    //key = gas id, value = moles required
                    if (kvp.Key == "TEMP" || kvp.Key == "ENER") continue;
                    BGas found_gas = Constants.Gases.FindBGas(kvp.Key);

                    if(found_gas)
                    {
                        if(air_contents.ContainsKey(found_gas))
                        {
                            if(air_contents[found_gas] >= Convert.ToDouble(kvp.Value))
                            {
                                Console.WriteLine($"Found gas {kvp.Key}");
                                if (gr.id == "nobstop") //if we matched the reqs for noblium suppression
                                {
                                    return false;
                                }
                                continue;
                            }
                        }
                    }
                    goto reaction_loop;
                }

                Console.WriteLine($"Reacted aircontents using reaction {gr.name}");
                reacted = true;
                reacted = gr.react(this);

                reaction_loop:;
            }
            return reacted;
        }

        /// <summary>
        /// A method that casts a GasMixture gm to a bool.
        /// </summary>
        /// <param name="gm">GasMixture to be casted to bool</param>
        public static implicit operator bool(GasMixture gm) => gm.air_contents != null;
    }
}
