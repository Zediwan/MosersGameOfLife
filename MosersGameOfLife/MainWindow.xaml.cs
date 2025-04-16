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

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Initialize the grid
            int cols = 20, rows = 20; // Adjust as needed
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

            // Set UniformGrid rows and columns
            GridDisplay.Rows = rows;
            GridDisplay.Columns = cols;

            // Create the rectangles once and store them in a 1D array
            _rectangles = new Rectangle[cols * rows];
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    var rectangle = new Rectangle
                    {
                        Fill = Brushes.White,
                        Stroke = Brushes.Gray, // Optional: Add a border
                        StrokeThickness = 0.5
                    };
                    _rectangles[i * rows + j] = rectangle;
                    GridDisplay.Children.Add(rectangle);
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
                    var cell = _grid.Cells[i, j];
                    var rectangle = _rectangles[i * rows + j];
                    rectangle.Fill = cell.IsAlive() ? new SolidColorBrush(cell.GetColor()) : Brushes.White; // Dead cells are white
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
