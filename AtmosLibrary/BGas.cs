using System;
using System.Collections.Generic;
using System.Text;

namespace AtmosLibrary
{
    /// <summary>
    /// Definition of immutable read-only BGas class, which is used to instantiate all base gases such as oxygen or plasma.
    /// Instantiation of all base gases is done in Constants.cs.
    /// </summary>
    public class BGas
    {
        /// <summary>
        /// Short identifier for base gas.
        /// </summary>
        public string id { get; }
        /// <summary>
        /// More formal name for base gas.
        /// </summary>
        public string name { get; }
        /// <summary>
        /// Integer "specific heat". Pertains to how much energy it takes to heat the gas up.
        /// </summary>
        public double specific_heat { get; }
        /// <summary>
        /// Pertains to how much a certain gas accelerates a plasma/co2 fusion reaction
        /// </summary>
        public double fusion_power { get; }

        public BGas(string _id, string _name, double _specific_heat, double _fusion_power)
        {
            id = _id;
            name = _name;
            specific_heat = _specific_heat;
            fusion_power = _fusion_power;
        }
        /// <summary>
        /// A method that casts a BGas bg to a bool.
        /// </summary>
        /// <param name="bg">BGas to be casted to bool</param>
        public static implicit operator bool(BGas bg) => bg != null;
    }
}
