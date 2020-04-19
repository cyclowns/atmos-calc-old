using System;
using System.Collections.Generic;

namespace AtmosLibrary
{
    /// <summary>
    /// Represents a GasReaction object that contains info about a certain reaction - fire, noblium suppression, fusion, etc.
    /// Reaction objects and function delegates are defined in Constants.cs
    /// </summary>
    public class GasReaction
    {
        /// <summary>
        /// Formal name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Informal id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Delegate containing function to be called on reaction
        /// </summary>
        public Func<GasMixture, bool> react { get; set; }
        /// <summary>
        /// Requirements in Dictionary form, keys/values casted to int/double when needed
        /// </summary>
        public Dictionary<string, string> requirements { get; set; }
        /// <summary>
        /// Priority when multiple reactions take place. Higher means it happens first
        /// </summary>
        public int priority { get; set; }

        /// <summary>
        /// Constructor for GasReaction
        /// </summary>
        /// <param name="_name">Formal name</param>
        /// <param name="_id">Informal scientific id</param>
        /// <param name="_react">Delegate containing function to be called on reaction</param>
        /// <param name="_requirements">Requirements in dictionary form</param>
        /// <param name="_priority">Priority when multiple reactions take place</param>
        public GasReaction(string _name, string _id, Func<GasMixture, bool> _react, Dictionary<string, string> _requirements, int _priority)
        {
            name = _name;
            id = _id;
            react = _react;
            requirements = _requirements;
            priority = _priority;
        }

        /// <summary>
        /// Compares priority between two GasReactions. Returns true if host GasReaction has higher priority.
        /// </summary>
        /// <param name="gm">GasReaction to be compared against</param>
        /// <returns>Bool, true if host GasReaction has higher priority</returns>
        public bool ComparePriority(GasReaction gm) => (priority - gm.priority) > 0;
    }
}
