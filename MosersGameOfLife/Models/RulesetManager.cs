using System.IO;
using System.Text.Json;

using MosersGameOfLife.Models;

namespace MosersGameOfLife.Models
{
    /// <summary>
    /// Manages rulesets for the Conway's Game of Life application.
    /// Handles loading, saving, and applying rulesets.
    /// </summary>
    public static class RulesetManager
    {
        private static readonly string SaveDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MosersGameOfLife");

        /// <summary>
        /// Full path to the ruleset save file.
        /// </summary>
        private static readonly string SaveFilePath = Path.Combine(SaveDirectory, "rulesets.json");

        /// <summary>
        /// List of all rulesets (predefined and custom).
        /// </summary>
        private static List<Ruleset> _rulesets;

        /// <summary>
        /// List of predefined rulesets (built-in).
        /// </summary>
        private static List<Ruleset> _predefinedRulesets = new List<Ruleset>
        {
            new Ruleset("Conway's Game of Life", new HashSet<int> { 3 }, new HashSet<int> { 2, 3 }),
            new Ruleset("HighLife", new HashSet<int> { 3, 6 }, new HashSet<int> { 2, 3 }),
            new Ruleset("Day & Night", new HashSet<int> { 3, 6, 7, 8 }, new HashSet<int> { 3, 4, 6, 7, 8 }),
            new Ruleset("Replicator", new HashSet<int> { 1, 3, 5, 7 }, new HashSet<int> { 1, 3, 5, 7 }),
            new Ruleset("Seeds", new HashSet<int> { 2 }, new HashSet<int>()),
            new Ruleset("Life without death", new HashSet<int> { 3 }, new HashSet<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 }),
            new Ruleset("2x2", new HashSet<int> { 3, 6 }, new HashSet<int> { 2, 4, 5 }),
            new Ruleset("Assimilation", new HashSet<int> { 3, 4, 5 }, new HashSet<int> { 5 }),
            new Ruleset("Isolated Birth", new HashSet<int> { 1 }, new HashSet<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 }),
            new Ruleset("Maze", new HashSet<int> { 3, 4 }, new HashSet<int> { 3, 4 }),
            new Ruleset("Coagulations", new HashSet<int> { 3, 8 }, new HashSet<int> { 2, 3 }),
            new Ruleset("Diamoeba", new HashSet<int> { 3 }, new HashSet<int> { 0, 1, 2, 3, 4, 5, 6 }),
            new Ruleset("Anneal", new HashSet<int> { 2 }, new HashSet<int> { 3, 4, 5, 6, 7, 8 }),
            new Ruleset("Long Life", new HashSet<int> { 3 }, new HashSet<int> { 1, 2, 3, 4, 5 }),
            new Ruleset("Gnarl", new HashSet<int> { 2, 5 }, new HashSet<int> { 4 }),
            new Ruleset("Stains", new HashSet<int> { 3, 5, 7 }, new HashSet<int> { 1, 3, 5, 8 }),
            new Ruleset("Fill", new HashSet<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, new HashSet<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 })
        };

        /// <summary>
        /// Gets the list of all available rulesets.
        /// </summary>
        public static List<Ruleset> Rulesets => _rulesets;

        /// <summary>
        /// Gets the list of predefined (built-in) rulesets.
        /// </summary>
        public static List<Ruleset> PredefinedRulesets => _predefinedRulesets;

        /// <summary>
        /// Static constructor that initializes the RulesetManager.
        /// </summary>
        static RulesetManager()
        {
            // Initialize the rulesets list with predefined rulesets
            _rulesets = new List<Ruleset>(_predefinedRulesets);

            // Load any saved custom rulesets
            LoadSavedRules();
        }

        /// <summary>
        /// Loads user-saved rulesets from the save file.
        /// </summary>
        public static void LoadSavedRules()
        {
            try
            {
                if (!Directory.Exists(SaveDirectory))
                {
                    Directory.CreateDirectory(SaveDirectory);
                }

                if (File.Exists(SaveFilePath))
                {
                    string json = File.ReadAllText(SaveFilePath);
                    var savedRulesets = JsonSerializer.Deserialize<List<Ruleset>>(json);
                    if (savedRulesets != null)
                    {
                        // Add custom rulesets (don't replace predefined ones)
                        foreach (var ruleset in savedRulesets)
                        {
                            if (!_rulesets.Any(r => r.Name == ruleset.Name))
                            {
                                _rulesets.Add(ruleset);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading rulesets: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves custom rulesets to the save file.
        /// </summary>
        public static void SaveRules()
        {
            try
            {
                if (!Directory.Exists(SaveDirectory))
                {
                    Directory.CreateDirectory(SaveDirectory);
                }

                // Only save custom rulesets (skip predefined ones)
                var customRulesets = _rulesets.Where(r => !_predefinedRulesets.Any(p => p.Name == r.Name)).ToList();
                string json = JsonSerializer.Serialize(customRulesets, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(SaveFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving rulesets: {ex.Message}");
            }
        }

        /// <summary>
        /// Applies a ruleset to a grid.
        /// </summary>
        /// <param name="ruleset">The ruleset to apply</param>
        /// <param name="grid">The grid to apply the ruleset to</param>
        public static void ApplyRuleset(Ruleset ruleset, Grid grid)
        {
            grid.BirthRules = new HashSet<int>(ruleset.BirthRules);
            grid.SurvivalRules = new HashSet<int>(ruleset.SurvivalRules);
        }

        /// <summary>
        /// Adds or updates a ruleset in the collection.
        /// </summary>
        /// <param name="ruleset">The ruleset to add or update</param>
        /// <exception cref="InvalidOperationException">Thrown if trying to modify a predefined ruleset</exception>
        public static void AddRuleset(Ruleset ruleset)
        {
            // Check if this is trying to overwrite a predefined ruleset
            if (_predefinedRulesets.Any(r => r.Name == ruleset.Name))
            {
                throw new InvalidOperationException("Cannot modify predefined rulesets.");
            }

            // Check for existing rulesets with identical rules
            var existingRulePattern = _rulesets.FirstOrDefault(r =>
                r.Name != ruleset.Name && r.HasSameRules(ruleset));

            if (existingRulePattern != null)
            {
                throw new InvalidOperationException(
                    $"A ruleset with these exact rules already exists with name '{existingRulePattern.Name}'. " +
                    $"Rule pattern: {existingRulePattern.GetNotation()}");
            }

            // Check if a ruleset with this name already exists
            int existingIndex = _rulesets.FindIndex(r => r.Name == ruleset.Name);
            if (existingIndex >= 0)
            {
                // Replace existing ruleset
                _rulesets[existingIndex] = ruleset;
            }
            else
            {
                // Add new ruleset
                _rulesets.Add(ruleset);
            }

            // Save changes
            SaveRules();
        }

        /// <summary>
        /// Deletes a ruleset from the collection.
        /// </summary>
        /// <param name="name">Name of the ruleset to delete</param>
        /// <exception cref="InvalidOperationException">Thrown if trying to delete a predefined ruleset</exception>
        public static void DeleteRuleset(string name)
        {
            // Don't allow deletion of predefined rules
            if (_predefinedRulesets.Any(r => r.Name == name))
            {
                throw new InvalidOperationException("Cannot delete predefined rulesets.");
            }

            int index = _rulesets.FindIndex(r => r.Name == name);
            if (index >= 0)
            {
                _rulesets.RemoveAt(index);
                SaveRules();
            }
        }

        /// <summary>
        /// Gets a ruleset representing the current state of a grid.
        /// </summary>
        /// <param name="grid">The grid to create a ruleset from</param>
        /// <returns>A ruleset with the current rules of the grid</returns>
        public static Ruleset GetCurrentRuleset(Grid grid)
        {
            return new Ruleset(
                "Current",
                new HashSet<int>(grid.BirthRules),
                new HashSet<int>(grid.SurvivalRules)
            );
        }
    }
}
