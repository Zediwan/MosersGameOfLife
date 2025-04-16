using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
                        Fill = Brushes.White, // Default color
                        Stroke = Brushes.Gray, // Optional border
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
                    rectangle.Fill = new SolidColorBrush(cell.GetColor());
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
    }
}
