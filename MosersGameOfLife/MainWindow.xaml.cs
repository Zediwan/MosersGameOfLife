using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using MosersGameOfLife.src;
using Grid = MosersGameOfLife.src.Grid;

namespace MosersGameOfLife
{
    public partial class MainWindow : Window
    {
        private Grid _grid;
        private DispatcherTimer _timer;
        private Rectangle[] _rectangles; // 1D array to store UI elements
        private bool _isTrailEnabled;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Initialize the grid
            int cols = 50, rows = 50; // Adjust as needed
            _grid = Grid.GetRandomGrid(cols, rows);

            // Initialize the UI
            InitializeGridUI();

            // Set up the timer
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100) // Adjust update interval as needed
            };
            _timer.Tick += (s, e) => UpdateGrid();
            _timer.Start();
        }

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
                        Width = double.NaN, // Adjust size as needed
                        Height = double.NaN,
                        Fill = Brushes.Black, // Default color
                        StrokeThickness = 0.5
                    };

                    // Add the rectangle to the UI and the array
                    GridDisplay.Children.Add(rectangle);
                    _rectangles[i * rows + j] = rectangle;
                }
            }
        }

        private void UpdateGrid()
        {
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
                        // Render dead cells as white
                        rectangle.Fill = Brushes.Black;
                    }
                }
            }
        }

        private void GenerateNewGrid_Click(object sender, RoutedEventArgs e)
        {
            // Regenerate the grid with the same dimensions
            int cols = _grid.Cells.GetLength(0);
            int rows = _grid.Cells.GetLength(1);
            _grid = Grid.GetRandomGrid(cols, rows);

            // Refresh the UI
            UpdateGrid();
        }

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
        private void TrailCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _isTrailEnabled = true;
        }

        private void TrailCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _isTrailEnabled = false;
        }

        private void RuleCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkbox)
            {
                if (_grid == null) return;

                int rule = int.Parse(checkbox.Content.ToString());
                if (checkbox.Name.StartsWith("B"))
                {
                    _grid.BirthRules.Add(rule);
                }
                else if (checkbox.Name.StartsWith("S"))
                {
                    _grid.SurvivalRules.Add(rule);
                }
            }
        }

        private void RuleCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkbox)
            {
                if (_grid == null) return;

                int rule = int.Parse(checkbox.Content.ToString());
                if (checkbox.Name.StartsWith("B"))
                {
                    _grid.BirthRules.Remove(rule);
                }
                else if (checkbox.Name.StartsWith("S"))
                {
                    _grid.SurvivalRules.Remove(rule);
                }
            }
        }
    }
}
