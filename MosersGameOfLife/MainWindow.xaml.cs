using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using MosersGameOfLife.Models;
using MosersGameOfLife.Views;
using Grid = MosersGameOfLife.Models.Grid;

namespace MosersGameOfLife
{
    /// <summary>
    /// Main window for Conway's Game of Life implementation.
    /// Handles UI interaction, grid visualization, and simulation control.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        // Simulation Core Components
        private Grid _grid;                      // Game grid containing all cells and logic
        private DispatcherTimer _timer;          // Timer for controlling simulation speed
        private Rectangle[] _rectangles;         // UI elements representing cells
        private bool _isPaused = false;          // Tracks simulation pause state

        // Ruleset Management
        private RulesetManager _rulesetManager;  // Handles ruleset storage and retrieval
        private bool _updatingCheckboxes = false; // Flag to prevent recursive checkbox events

        // Display and Rendering Options
        private bool _isTrailEnabled;            // Whether to show cell trails

        // Painting and User Interaction
        private bool _isMouseDown = false;       // Tracks mouse state for painting
        private bool _paintAliveState = true;    // Whether to paint alive or dead cells
        private CheckBox _paintToggleButton;     // Reference to paint toggle UI element

        // Painting Color Management
        private byte _currentPaintR = 0;         // Red component of current paint color
        private byte _currentPaintG = 255;       // Green component of current paint color
        private byte _currentPaintB = 0;         // Blue component of current paint color
        private Random _random = new Random();   // Random number generator for colors

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the main window and starts the Game of Life simulation.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _rulesetManager = new RulesetManager();

            // Initialize the first paint color
            GenerateDefaultPaintColor();

            // Initialize non-nullable fields
            _grid = new Grid(50, 50); // Default grid dimensions
            _timer = new DispatcherTimer();
            _rectangles = Array.Empty<Rectangle>();
            _paintToggleButton = new CheckBox();

            InitializeGame();
            InitializeRulesetUI();
        }

        /// <summary>
        /// Generates a consistent default color for painting cells.
        /// </summary>
        private void GenerateDefaultPaintColor()
        {
            // Set a consistent default color (e.g., green)
            _currentPaintR = 0;
            _currentPaintG = 255;
            _currentPaintB = 0;
        }

        /// <summary>
        /// Initializes the simulation grid and timer.
        /// </summary>
        private void InitializeGame()
        {
            // Initialize the grid
            int cols = 50, rows = 50; // Default grid dimensions
            _grid = Grid.GetRandomGrid(cols, rows);

            // Initialize the UI
            InitializeGridUI();

            // Set up the timer for simulation updates
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100) // Default update speed
            };
            _timer.Tick += (s, e) => UpdateGrid();
            _timer.Start();

            UpdateCurrentRulesetText();
        }

        /// <summary>
        /// Initializes the ruleset selection UI and populates it with available rulesets.
        /// </summary>
        private void InitializeRulesetUI()
        {
            // Populate the ruleset combobox
            RulesetComboBox.Items.Clear();
            foreach (var ruleset in _rulesetManager.Rulesets)
            {
                RulesetComboBox.Items.Add(ruleset.Name);
            }

            // Select Conway's Game of Life by default
            if (RulesetComboBox.Items.Count > 0)
            {
                RulesetComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Initializes the visual grid UI and sets up mouse event handling.
        /// </summary>
        private void InitializeGridUI()
        {
            int cols = _grid.Cells.GetLength(0);
            int rows = _grid.Cells.GetLength(1);

            // Initialize the _rectangles array
            _rectangles = new Rectangle[cols * rows];

            // Clear the UI and set up a uniform grid
            GridDisplay.Children.Clear();
            GridDisplay.Rows = rows;
            GridDisplay.Columns = cols;

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    // Create a new rectangle for each cell
                    var rectangle = new Rectangle
                    {
                        Width = double.NaN, // Size determined by UniformGrid
                        Height = double.NaN,
                        Fill = Brushes.Black, // Default color
                        StrokeThickness = 0.5,
                        Tag = new Point(i, j) // Store the cell coordinates in the Tag property
                    };

                    // Add the event handlers for mouse interaction
                    rectangle.MouseDown += Rectangle_MouseDown;
                    rectangle.MouseEnter += Rectangle_MouseEnter;
                    rectangle.MouseUp += Rectangle_MouseUp;

                    // Add the rectangle to the UI and the array
                    GridDisplay.Children.Add(rectangle);
                    _rectangles[i * rows + j] = rectangle;
                }
            }

            // Add mouse event handlers to the grid display
            GridDisplay.MouseDown += GridDisplay_MouseDown;
            GridDisplay.MouseUp += GridDisplay_MouseUp;
            GridDisplay.MouseLeave += GridDisplay_MouseLeave;
        }

        #endregion

        #region Grid Updates and Rendering

        /// <summary>
        /// Updates the grid state and renders the changes to the UI.
        /// Called periodically by the timer to advance the simulation.
        /// </summary>
        private void UpdateGrid()
        {
            if (_isPaused) return;

            // Update the grid state
            _grid.Update();

            // Update the UI without rebuilding it
            int cols = _grid.Cells.GetLength(0);
            int rows = _grid.Cells.GetLength(1);

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Cell cell = _grid.Cells[i, j];
                    var rectangle = _rectangles[i * rows + j];

                    if (cell.IsAlive)
                    {
                        // Render alive cells
                        rectangle.Fill = new SolidColorBrush(cell.GetColor());
                    }
                    else if (_isTrailEnabled && cell.HasTrail)
                    {
                        // Render trail if enabled
                        rectangle.Fill = new SolidColorBrush(cell.GetColor());
                    }
                    else
                    {
                        // Render dead cells as black
                        rectangle.Fill = Brushes.Black;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the text displaying the current ruleset notation.
        /// </summary>
        private void UpdateCurrentRulesetText()
        {
            var currentRuleset = _rulesetManager.GetCurrentRuleset(_grid);
            CurrentRulesetText.Text = $"Current: {currentRuleset.GetNotation()}";
        }

        /// <summary>
        /// Generates a new random vibrant color for painting cells.
        /// </summary>
        private void GenerateNewPaintColor()
        {
            // Generate vibrant colors by avoiding very dark or very light colors
            _currentPaintR = (byte)(_random.Next(128) + 64);
            _currentPaintG = (byte)(_random.Next(128) + 64);
            _currentPaintB = (byte)(_random.Next(128) + 64);
        }

        /// <summary>
        /// Handles mouse down on a specific cell rectangle.
        /// </summary>
        private void Rectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Generate a new color only when not in the default mode
            if (!_isMouseDown && _paintAliveState && Grid.ColorBehavior != ColorBehaviorMode.Default)
            {
                GenerateNewPaintColor();
            }

            _isMouseDown = true;
            ToggleCellState((Rectangle)sender);
            e.Handled = true;
        }

        /// <summary>
        /// Handles mouse entering a cell rectangle during drag operations.
        /// </summary>
        private void Rectangle_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                ToggleCellState((Rectangle)sender);
            }
        }

        /// <summary>
        /// Handles mouse up events on cell rectangles.
        /// </summary>
        private void Rectangle_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isMouseDown = false;
        }

        /// <summary>
        /// Handles mouse down events on the grid display.
        /// </summary>
        private void GridDisplay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Generate a new color only when not in the default mode
            if (!_isMouseDown && _paintAliveState && Grid.ColorBehavior != ColorBehaviorMode.Default)
            {
                GenerateNewPaintColor();
            }

            _isMouseDown = true;
        }

        /// <summary>
        /// Handles mouse up events on the grid display.
        /// </summary>
        private void GridDisplay_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isMouseDown = false;
        }

        /// <summary>
        /// Handles mouse leaving the grid display area.
        /// </summary>
        private void GridDisplay_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _isMouseDown = false;
        }

        /// <summary>
        /// Toggles the state of a cell based on the current painting mode.
        /// </summary>
        /// <param name="rectangle">The rectangle representing the cell to modify</param>
        private void ToggleCellState(Rectangle rectangle)
        {
            if (rectangle.Tag is Point point)
            {
                int i = (int)point.X;
                int j = (int)point.Y;

                // Set the cell state based on the paint mode
                if (_paintAliveState)
                {
                    // Use the current paint color
                    byte r, g, b;
                    if (Grid.ColorBehavior == ColorBehaviorMode.Default)
                    {
                        // Use the current paint color
                        r = _currentPaintR;
                        g = _currentPaintG;
                        b = _currentPaintB;
                    }
                    else
                    {
                        // For other color modes, still use the current paint color
                        r = _currentPaintR;
                        g = _currentPaintG;
                        b = _currentPaintB;
                    }

                    // Paint alive cell with the current color
                    _grid.SetCellState(i, j, true, r, g, b);

                    // Update the UI
                    rectangle.Fill = new SolidColorBrush(_grid.Cells[i, j].GetColor());
                }
                else
                {
                    // Paint dead cell
                    if (_isTrailEnabled)
                    {
                        // With trail - keep some color but mark as dead
                        _grid.SetCellState(i, j, false, _grid.Cells[i, j].R, _grid.Cells[i, j].G, _grid.Cells[i, j].B, 254);
                    }
                    else
                    {
                        // No trail - completely black
                        _grid.SetCellState(i, j, false, 0, 0, 0, 0);
                    }

                    // Update the UI
                    rectangle.Fill = _isTrailEnabled && _grid.Cells[i, j].HasTrail ?
                        new SolidColorBrush(_grid.Cells[i, j].GetColor()) : Brushes.Black;
                }
            }
        }

        #endregion

        #region Ruleset Management

        /// <summary>
        /// Updates all rule checkboxes to reflect the given ruleset.
        /// </summary>
        /// <param name="ruleset">The ruleset to display</param>
        private void UpdateCheckboxesFromRuleset(Ruleset ruleset)
        {
            _updatingCheckboxes = true;

            try
            {
                // Update birth rule checkboxes
                B0.IsChecked = ruleset.BirthRules.Contains(0);
                B1.IsChecked = ruleset.BirthRules.Contains(1);
                B2.IsChecked = ruleset.BirthRules.Contains(2);
                B3.IsChecked = ruleset.BirthRules.Contains(3);
                B4.IsChecked = ruleset.BirthRules.Contains(4);
                B5.IsChecked = ruleset.BirthRules.Contains(5);
                B6.IsChecked = ruleset.BirthRules.Contains(6);
                B7.IsChecked = ruleset.BirthRules.Contains(7);
                B8.IsChecked = ruleset.BirthRules.Contains(8);

                // Update survival rule checkboxes
                S0.IsChecked = ruleset.SurvivalRules.Contains(0);
                S1.IsChecked = ruleset.SurvivalRules.Contains(1);
                S2.IsChecked = ruleset.SurvivalRules.Contains(2);
                S3.IsChecked = ruleset.SurvivalRules.Contains(3);
                S4.IsChecked = ruleset.SurvivalRules.Contains(4);
                S5.IsChecked = ruleset.SurvivalRules.Contains(5);
                S6.IsChecked = ruleset.SurvivalRules.Contains(6);
                S7.IsChecked = ruleset.SurvivalRules.Contains(7);
                S8.IsChecked = ruleset.SurvivalRules.Contains(8);
            }
            finally
            {
                _updatingCheckboxes = false;
            }
        }

        /// <summary>
        /// Creates a ruleset object based on the current checkbox states.
        /// </summary>
        /// <returns>A ruleset containing the rules from the UI checkboxes</returns>
        private Ruleset GetRulesetFromCheckboxes()
        {
            var birthRules = new HashSet<int>();
            var survivalRules = new HashSet<int>();

            // Get birth rules from checkboxes
            if (B0.IsChecked == true) birthRules.Add(0);
            if (B1.IsChecked == true) birthRules.Add(1);
            if (B2.IsChecked == true) birthRules.Add(2);
            if (B3.IsChecked == true) birthRules.Add(3);
            if (B4.IsChecked == true) birthRules.Add(4);
            if (B5.IsChecked == true) birthRules.Add(5);
            if (B6.IsChecked == true) birthRules.Add(6);
            if (B7.IsChecked == true) birthRules.Add(7);
            if (B8.IsChecked == true) birthRules.Add(8);

            // Get survival rules from checkboxes
            if (S0.IsChecked == true) survivalRules.Add(0);
            if (S1.IsChecked == true) survivalRules.Add(1);
            if (S2.IsChecked == true) survivalRules.Add(2);
            if (S3.IsChecked == true) survivalRules.Add(3);
            if (S4.IsChecked == true) survivalRules.Add(4);
            if (S5.IsChecked == true) survivalRules.Add(5);
            if (S6.IsChecked == true) survivalRules.Add(6);
            if (S7.IsChecked == true) survivalRules.Add(7);
            if (S8.IsChecked == true) survivalRules.Add(8);

            return new Ruleset("", birthRules, survivalRules);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles clicks on the Generate New Grid button.
        /// Creates a new random grid and updates the UI.
        /// </summary>
        private void GenerateNewGrid_Click(object sender, RoutedEventArgs e)
        {
            // Regenerate the grid with the same dimensions
            int cols = _grid.Cells.GetLength(0);
            int rows = _grid.Cells.GetLength(1);
            _grid = Grid.GetRandomGrid(cols, rows);

            // Refresh the UI
            UpdateGrid();
        }

        /// <summary>
        /// Handles selection changes in the color behavior combo box.
        /// </summary>
        private void ColorBehaviorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorBehaviorComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (_grid == null) return;

                var selectedMode = (string)selectedItem.Tag;

                switch (selectedMode)
                {
                    case "MajorityColor":
                        Grid.ColorBehavior = ColorBehaviorMode.MajorityColor;
                        break;

                    case "AverageColor":
                        Grid.ColorBehavior = ColorBehaviorMode.AverageColor;
                        break;

                    case "Default":
                        Grid.ColorBehavior = ColorBehaviorMode.Default;
                        break;
                }
            }
            GenerateNewGrid_Click(sender, e);
        }

        /// <summary>
        /// Handles toggling of the trail effect.
        /// </summary>
        private void TrailCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _isTrailEnabled = true;
        }

        /// <summary>
        /// Handles disabling of the trail effect.
        /// </summary>
        private void TrailCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _isTrailEnabled = false;
        }

        /// <summary>
        /// Handles toggling of the paint mode to paint alive cells.
        /// </summary>
        private void PaintToggle_Checked(object sender, RoutedEventArgs e)
        {
            _paintAliveState = true;
        }

        /// <summary>
        /// Handles toggling of the paint mode to paint dead cells.
        /// </summary>
        private void PaintToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            _paintAliveState = false;
        }

        /// <summary>
        /// Handles clicks on the pause/resume button.
        /// </summary>
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            _isPaused = !_isPaused;

            if (_isPaused)
            {
                _timer.Stop();
                ((Button)sender).Content = "Resume";
            }
            else
            {
                _timer.Start();
                ((Button)sender).Content = "Pause";
            }
        }

        /// <summary>
        /// Handles a rule checkbox being checked.
        /// </summary>
        private void RuleCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (_updatingCheckboxes) return;

            if (sender is CheckBox checkbox)
            {
                if (_grid == null) return;

                int rule = int.Parse(checkbox.Content?.ToString() ?? "0");
                if (checkbox.Name.StartsWith("B"))
                {
                    _grid.BirthRules.Add(rule);
                }
                else if (checkbox.Name.StartsWith("S"))
                {
                    _grid.SurvivalRules.Add(rule);
                }

                UpdateCurrentRulesetText();
            }
        }

        /// <summary>
        /// Handles a rule checkbox being unchecked.
        /// </summary>
        private void RuleCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_updatingCheckboxes) return;

            if (sender is CheckBox checkbox)
            {
                if (_grid == null) return;

                int rule = int.Parse(checkbox.Content?.ToString() ?? "0");
                if (checkbox.Name.StartsWith("B"))
                {
                    _grid.BirthRules.Remove(rule);
                }
                else if (checkbox.Name.StartsWith("S"))
                {
                    _grid.SurvivalRules.Remove(rule);
                }

                UpdateCurrentRulesetText();
            }
        }

        /// <summary>
        /// Handles selection changes in the ruleset combo box.
        /// </summary>
        private void RulesetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RulesetComboBox.SelectedIndex < 0) return;

            string? selectedRulesetName = RulesetComboBox.SelectedItem?.ToString();
            Ruleset? selectedRuleset = _rulesetManager.Rulesets.FirstOrDefault(r => r.Name == selectedRulesetName);

            if (selectedRuleset != null)
            {
                // Display ruleset description if available
                RulesetDescriptionText.Text = string.IsNullOrWhiteSpace(selectedRuleset.Description) ?
                    "No description available." : selectedRuleset.Description;

                // Apply ruleset to grid
                _rulesetManager.ApplyRuleset(selectedRuleset, _grid);

                // Update checkboxes to reflect ruleset
                UpdateCheckboxesFromRuleset(selectedRuleset);

                // Update current ruleset display
                UpdateCurrentRulesetText();

                // Set the name textbox to the selected ruleset name
                RulesetNameTextBox.Text = selectedRuleset.Name;
            }
        }

        /// <summary>
        /// Handles clicks on the Save Ruleset button.
        /// </summary>
        private void SaveRuleset_Click(object sender, RoutedEventArgs e)
        {
            string? rulesetName = RulesetNameTextBox.Text?.Trim();

            if (string.IsNullOrEmpty(rulesetName))
            {
                MessageBox.Show("Please enter a name for the ruleset.", "Name Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Get current ruleset from checkboxes
                Ruleset newRuleset = GetRulesetFromCheckboxes();
                newRuleset.Name = rulesetName;

                // Try to find if a ruleset with these exact rules already exists but has a different name
                var existingRuleset = _rulesetManager.Rulesets.FirstOrDefault(r =>
                    r.Name != rulesetName &&
                    r.BirthRules.SetEquals(newRuleset.BirthRules) &&
                    r.SurvivalRules.SetEquals(newRuleset.SurvivalRules));

                if (existingRuleset != null)
                {
                    // A ruleset with the same rules but different name exists
                    var dialog = new DuplicateRulesetDialog(existingRuleset.Name, existingRuleset.GetNotation());
                    dialog.Owner = this;
                    dialog.ShowDialog();

                    switch (dialog.Result)
                    {
                        case DuplicateRulesetDialog.DialogResult.UseExisting:
                            // Select the existing ruleset
                            int index = RulesetComboBox.Items.IndexOf(existingRuleset.Name);
                            if (index >= 0)
                            {
                                RulesetComboBox.SelectedIndex = index;
                            }
                            return;

                        case DuplicateRulesetDialog.DialogResult.Cancel:
                            // User cancelled, don't save anything
                            return;

                        case DuplicateRulesetDialog.DialogResult.CreateDuplicate:
                            // Continue below to create a duplicate
                            break;
                    }
                }

                // Determine if this is a new ruleset or updating an existing one
                bool isNewRuleset = !_rulesetManager.Rulesets.Any(r => r.Name == rulesetName);
                if (isNewRuleset)
                {
                    newRuleset.Description = "Custom ruleset";
                }
                else
                {
                    var existingNameRuleset = _rulesetManager.Rulesets.FirstOrDefault(r => r.Name == rulesetName);
                    if (existingNameRuleset != null)
                    {
                        newRuleset.Description = existingNameRuleset.Description;
                    }
                }

                // Save ruleset
                _rulesetManager.AddRuleset(newRuleset);

                // Refresh ruleset list
                string currentSelection = rulesetName;
                InitializeRulesetUI();

                // Select the saved ruleset
                int savedIndex = RulesetComboBox.Items.IndexOf(currentSelection);
                if (savedIndex >= 0)
                {
                    RulesetComboBox.SelectedIndex = savedIndex;
                }

                MessageBox.Show($"Ruleset '{rulesetName}' saved successfully.", "Ruleset Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles clicks on the Delete Ruleset button.
        /// </summary>
        private void DeleteRuleset_Click(object sender, RoutedEventArgs e)
        {
            string? rulesetName = RulesetNameTextBox.Text?.Trim();

            if (string.IsNullOrEmpty(rulesetName))
            {
                MessageBox.Show("Please select a ruleset to delete.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_rulesetManager.PredefinedRulesets.Any(r => r.Name == rulesetName))
                {
                    MessageBox.Show("Predefined rulesets cannot be deleted.", "Cannot Delete", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete ruleset '{rulesetName}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _rulesetManager.DeleteRuleset(rulesetName);

                    // Refresh ruleset list and select first item
                    InitializeRulesetUI();
                    if (RulesetComboBox.Items.Count > 0)
                    {
                        RulesetComboBox.SelectedIndex = 0;
                    }

                    MessageBox.Show($"Ruleset '{rulesetName}' deleted successfully.", "Ruleset Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
