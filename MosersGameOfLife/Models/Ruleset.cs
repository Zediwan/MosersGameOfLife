using MosersGameOfLife.Models;

namespace MosersGameOfLife.Models
{
    /// <summary>
    /// Represents a set of rules for Conway's Game of Life.
    /// Each ruleset consists of birth rules and survival rules.
    /// </summary>
    public class Ruleset
    {
        /// <summary>
        /// The name of the ruleset.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Set of neighbor counts that cause a dead cell to become alive.
        /// </summary>
        public HashSet<int> BirthRules { get; set; }

        /// <summary>
        /// Set of neighbor counts that allow an alive cell to survive.
        /// </summary>
        public HashSet<int> SurvivalRules { get; set; }

        /// <summary>
        /// Description of the ruleset's behavior and characteristics.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Creates a new empty ruleset with default values.
        /// </summary>
        public Ruleset()
        {
            Name = "New Ruleset";
            BirthRules = new HashSet<int>();
            SurvivalRules = new HashSet<int>();
            Description = "No description available.";
        }

        /// <summary>
        /// Creates a ruleset with the specified properties.
        /// </summary>
        /// <param name="name">Name of the ruleset</param>
        /// <param name="birthRules">Set of birth rules</param>
        /// <param name="survivalRules">Set of survival rules</param>
        /// <param name="description">Description of the ruleset</param>
        public Ruleset(string name, HashSet<int> birthRules, HashSet<int> survivalRules, string description = "")
        {
            Name = name;
            BirthRules = new HashSet<int>(birthRules);
            SurvivalRules = new HashSet<int>(survivalRules);
            Description = description;
        }

        /// <summary>
        /// Returns a string representation of the ruleset.
        /// </summary>
        /// <returns>A string in the format "Name: B[birth rules]/S[survival rules]"</returns>
        public override string ToString()
        {
            return $"{Name}: B{string.Join("", BirthRules)}/S{string.Join("", SurvivalRules)}";
        }

        /// <summary>
        /// Gets the standard notation for this ruleset.
        /// </summary>
        /// <returns>A string in the format "B[birth rules]/S[survival rules]"</returns>
        public string GetNotation()
        {
            return $"B{string.Join("", BirthRules)}/S{string.Join("", SurvivalRules)}";
        }

        /// <summary>
        /// Determines if this ruleset has the same rules as another ruleset.
        /// </summary>
        /// <param name="other">The ruleset to compare with</param>
        /// <returns>True if both rulesets have identical birth and survival rules</returns>
        public bool HasSameRules(Ruleset other)
        {
            if (other == null) return false;

            // Check if the birth and survival rules are the same
            return BirthRules.SetEquals(other.BirthRules) &&
                   SurvivalRules.SetEquals(other.SurvivalRules);
        }
    }
}