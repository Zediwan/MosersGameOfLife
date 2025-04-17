using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace MosersGameOfLife.src
{
    public class Ruleset
    {
        public string Name { get; set; }
        public HashSet<int> BirthRules { get; set; }
        public HashSet<int> SurvivalRules { get; set; }
        public string Description { get; set; }

        public Ruleset()
        {
            BirthRules = new HashSet<int>();
            SurvivalRules = new HashSet<int>();
        }

        public Ruleset(string name, HashSet<int> birthRules, HashSet<int> survivalRules, string description = "")
        {
            Name = name;
            BirthRules = new HashSet<int>(birthRules);
            SurvivalRules = new HashSet<int>(survivalRules);
            Description = description;
        }

        public override string ToString()
        {
            return $"{Name}: B{string.Join("", BirthRules)}/S{string.Join("", SurvivalRules)}";
        }
        
        public string GetNotation()
        {
            return $"B{string.Join("", BirthRules)}/S{string.Join("", SurvivalRules)}";
        }

        public bool HasSameRules(Ruleset other)
        {
            if (other == null) return false;

            // Check if the birth and survival rules are the same
            return BirthRules.SetEquals(other.BirthRules) &&
                   SurvivalRules.SetEquals(other.SurvivalRules);
        }
    }

    public class RulesetManager
    {
        private static readonly string SaveDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MosersGameOfLife");
        
        private static readonly string SaveFilePath = Path.Combine(SaveDirectory, "rulesets.json");
        
        private List<Ruleset> _rulesets;
        private List<Ruleset> _predefinedRulesets;

        public List<Ruleset> Rulesets => _rulesets;
        public List<Ruleset> PredefinedRulesets => _predefinedRulesets;

        public RulesetManager()
        {
            _rulesets = new List<Ruleset>();
            _predefinedRulesets = new List<Ruleset>();
            LoadPredefinedRules();
            LoadSavedRules();
        }

        private void LoadPredefinedRules()
        {
            // Load all the predefined rules
            _predefinedRulesets.Add(new Ruleset(
                "Conway's Game of Life", 
                new HashSet<int> { 3 },
                new HashSet<int> { 2, 3 },
                "The classic cellular automaton"));

            _predefinedRulesets.Add(new Ruleset(
                "HighLife", 
                new HashSet<int> { 3, 6 },
                new HashSet<int> { 2, 3 },
                "Supports replicators"));

            _predefinedRulesets.Add(new Ruleset(
                "Day & Night", 
                new HashSet<int> { 3, 6, 7, 8 },
                new HashSet<int> { 3, 4, 6, 7, 8 },
                "Symmetrical rule"));

            _predefinedRulesets.Add(new Ruleset(
                "Replicator", 
                new HashSet<int> { 1, 3, 5, 7 },
                new HashSet<int> { 1, 3, 5, 7 },
                "Creates replicating patterns"));

            _predefinedRulesets.Add(new Ruleset(
                "Seeds", 
                new HashSet<int> { 2 },
                new HashSet<int>(),
                "Fast growth, chaotic"));

            _predefinedRulesets.Add(new Ruleset(
                "Life without death", 
                new HashSet<int> { 3 },
                new HashSet<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                "Everything lives forever"));

            _predefinedRulesets.Add(new Ruleset(
                "2x2", 
                new HashSet<int> { 3, 6 },
                new HashSet<int> { 2, 4, 5 },
                "Blocks, emulates rule 90"));

            _predefinedRulesets.Add(new Ruleset(
                "Assimilation", 
                new HashSet<int> { 3, 4, 5 },
                new HashSet<int> { 5 },
                "Assimilates patterns"));

            _predefinedRulesets.Add(new Ruleset(
                "Isolated Birth", 
                new HashSet<int> { 1 },
                new HashSet<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                "Give birth when isolated (filling pattern)"));

            _predefinedRulesets.Add(new Ruleset(
                "Maze", 
                new HashSet<int> { 3, 4 },
                new HashSet<int> { 3, 4 },
                "Tends to form stable mazes"));

            _predefinedRulesets.Add(new Ruleset(
                "Coagulations", 
                new HashSet<int> { 3, 8 },
                new HashSet<int> { 2, 3 },
                "Forms growing blobs"));

            _predefinedRulesets.Add(new Ruleset(
                "Diamoeba", 
                new HashSet<int> { 3 },
                new HashSet<int> { 0, 1, 2, 3, 4, 5, 6 },
                "Chaotic amoeba-like growth"));

            _predefinedRulesets.Add(new Ruleset(
                "Anneal", 
                new HashSet<int> { 2 },
                new HashSet<int> { 3, 4, 5, 6, 7, 8 },
                "Melts patterns together"));

            _predefinedRulesets.Add(new Ruleset(
                "Long Life", 
                new HashSet<int> { 3 },
                new HashSet<int> { 1, 2, 3, 4, 5 },
                "Long-living structures"));

            _predefinedRulesets.Add(new Ruleset(
                "Gnarl", 
                new HashSet<int> { 2, 5 },
                new HashSet<int> { 4 },
                "Tree-like growth"));

            _predefinedRulesets.Add(new Ruleset(
                "Stains", 
                new HashSet<int> { 3, 5, 7 },
                new HashSet<int> { 1, 3, 5, 8 },
                "Forms stain-like patterns"));

            _predefinedRulesets.Add(new Ruleset(
                "Fill", 
                new HashSet<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                new HashSet<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                "Everything fills instantly"));
                
            // Add all predefined rulesets to the main ruleset list
            _rulesets.AddRange(_predefinedRulesets);
        }

        public void LoadSavedRules()
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

        public void SaveRules()
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

        public void ApplyRuleset(Ruleset ruleset, Grid grid)
        {
            grid.BirthRules = new HashSet<int>(ruleset.BirthRules);
            grid.SurvivalRules = new HashSet<int>(ruleset.SurvivalRules);
        }

        public void AddRuleset(Ruleset ruleset)
        {
            // Check if this is trying to overwrite a predefined ruleset
            if (_predefinedRulesets.Any(r => r.Name == ruleset.Name))
            {
                throw new InvalidOperationException("Cannot modify predefined rulesets.");
            }

            // Check if a ruleset with this exact rule pattern already exists (different name)
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

        public void DeleteRuleset(string name)
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

        public Ruleset GetCurrentRuleset(Grid grid)
        {
            return new Ruleset(
                "Current",
                new HashSet<int>(grid.BirthRules),
                new HashSet<int>(grid.SurvivalRules)
            );
        }
    }
}