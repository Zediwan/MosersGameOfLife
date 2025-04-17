namespace MosersGameOfLife.Models
{

    /// <summary>
    /// Defines different color behavior modes for cell visualization.
    /// </summary>
    public enum ColorBehaviorMode
    {
        /// <summary>
        /// Colors are determined by the most common color of neighboring cells.
        /// </summary>
        MajorityColor,

        /// <summary>
        /// Colors are determined by averaging the colors of neighboring cells.
        /// </summary>
        AverageColor,

        /// <summary>
        /// Default color behavior (green cells).
        /// </summary>
        Default
    }

    /// <summary>
    /// Represents the grid of cells in Conway's Game of Life.
    /// Handles the logic of cell updates according to the game rules.
    /// </summary>
    public class Grid
    {
        /// <summary>
        /// Number of columns in the grid.
        /// </summary>
        private int Cols { get; set; }

        /// <summary>
        /// Number of rows in the grid.
        /// </summary>
        private int Rows { get; set; }

        /// <summary>
        /// Two-dimensional array representing the current state of all cells.
        /// </summary>
        public Cell[,] Cells { get; set; }

        /// <summary>
        /// Buffer for calculating the next state without modifying the current state.
        /// </summary>
        private Cell[,] Buffer { get; set; }

        /// <summary>
        /// Global color behavior setting for all grids.
        /// </summary>
        public static ColorBehaviorMode ColorBehavior { get; set; } = ColorBehaviorMode.Default;

        /// <summary>
        /// Set of neighbor counts that cause a dead cell to become alive.
        /// Default: B3 (a dead cell becomes alive if it has exactly 3 alive neighbors).
        /// </summary>
        public HashSet<int> BirthRules { get; set; } = new HashSet<int> { 3 };

        /// <summary>
        /// Set of neighbor counts that allow an alive cell to survive.
        /// Default: S23 (an alive cell survives if it has 2 or 3 alive neighbors).
        /// </summary>
        public HashSet<int> SurvivalRules { get; set; } = new HashSet<int> { 2, 3 };

        /// <summary>
        /// Creates a new grid with the specified dimensions.
        /// </summary>
        /// <param name="cols">Number of columns</param>
        /// <param name="rows">Number of rows</param>
        public Grid(int cols, int rows)
        {
            this.Cols = cols;
            this.Rows = rows;
            this.Cells = new Cell[cols, rows];
            this.Buffer = new Cell[cols, rows]; // Initialize the buffer
        }

        /// <summary>
        /// Updates the grid state according to the Game of Life rules.
        /// </summary>
        public void Update()
        {
            // Swap the buffers
            (Buffer, Cells) = (Cells, Buffer);

            // Update all cells in parallel for better performance
            Parallel.For(0, Cols, i =>
            {
                for (var j = 0; j < Rows; j++)
                {
                    UpdateCell(i, j);
                }
            });
        }

        /// <summary>
        /// Updates the state of a single cell based on its neighbors.
        /// </summary>
        /// <param name="i">Column index of the cell</param>
        /// <param name="j">Row index of the cell</param>
        private void UpdateCell(int i, int j)
        {
            Cell cell = Cells[i, j];

            int aliveNeighbors;
            byte newR = 0, newG = 0, newB = 0;

            // Performance optimization: only calculate colors when needed
            if (ColorBehavior == ColorBehaviorMode.Default)
            {
                aliveNeighbors = CountAliveNeighbors(i, j);
            }
            else
            {
                var result = CountAliveNeighborsAndGetColor(i, j);
                aliveNeighbors = result.aliveCount;
                newR = result.avgR;
                newG = result.avgG;
                newB = result.avgB;
            }

            if (!cell.IsAlive && BirthRules.Contains(aliveNeighbors))
            {
                // Birth rule: a dead cell becomes alive if it has the right number of neighbors
                Buffer[i, j].ComeAlive();
                ApplyColorByBehavior(Buffer[i, j], newR, newG, newB);
            }
            else if (cell.IsAlive && !SurvivalRules.Contains(aliveNeighbors))
            {
                // Death rule: an alive cell dies if it doesn't have the right number of neighbors
                Buffer[i, j].Die();
                Buffer[i, j].SetColor(cell.R, cell.G, cell.B, 254); // Colored trail
            }
            else
            {
                // Survival or remain dead: copy the current state
                Buffer[i, j] = cell.Copy();
                if (Buffer[i, j].HasTrail)
                {
                    Buffer[i, j].FadeTrail();
                }
            }
        }

        /// <summary>
        /// Sets the color of a cell based on the current color behavior mode.
        /// </summary>
        /// <param name="cell">The cell to modify</param>
        /// <param name="r">Red component from neighbors</param>
        /// <param name="g">Green component from neighbors</param>
        /// <param name="b">Blue component from neighbors</param>
        private void ApplyColorByBehavior(Cell cell, byte r, byte g, byte b)
        {
            switch (ColorBehavior)
            {
                case ColorBehaviorMode.Default:
                    cell.SetColor(0, 255, 0); // Green
                    break;
                case ColorBehaviorMode.MajorityColor:
                case ColorBehaviorMode.AverageColor:
                    cell.SetColor(r, g, b); // Use calculated color
                    break;
            }
        }

        /// <summary>
        /// Gets the positions of the 8 neighbors of a cell, wrapping around edges.
        /// </summary>
        /// <param name="x">Column index of the cell</param>
        /// <param name="y">Row index of the cell</param>
        /// <returns>Enumerable of neighbor positions</returns>
        private IEnumerable<(int x, int y)> GetNeighborPositions(int x, int y)
        {
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue; // Skip the cell itself

                    // Wrap around the edges
                    int newX = (x + i + Cols) % Cols;
                    int newY = (y + j + Rows) % Rows;
                    yield return (newX, newY);
                }
            }
        }

        /// <summary>
        /// Processes the color of a neighbor cell based on the current color behavior.
        /// </summary>
        /// <param name="cell">The neighbor cell</param>
        /// <param name="totalR">Running total of red component</param>
        /// <param name="totalG">Running total of green component</param>
        /// <param name="totalB">Running total of blue component</param>
        /// <param name="colorCounts">Dictionary tracking color frequencies</param>
        private void ProcessCellColor(Cell cell, ref int totalR, ref int totalG, ref int totalB, Dictionary<(byte, byte, byte), int> colorCounts)
        {
            var color = cell.GetColor();

            if (ColorBehavior == ColorBehaviorMode.AverageColor)
            {
                // Add color components for averaging
                totalR += color.R;
                totalG += color.G;
                totalB += color.B;
            }
            else if (ColorBehavior == ColorBehaviorMode.MajorityColor && colorCounts != null)
            {
                // Count frequency of each color
                var key = (color.R, color.G, color.B);
                colorCounts[key] = colorCounts.TryGetValue(key, out var count) ? count + 1 : 1;
            }
        }

        /// <summary>
        /// Calculates the final color result based on neighbor colors.
        /// </summary>
        private (int aliveCount, byte avgR, byte avgG, byte avgB) CalculateColorResult(
            int aliveCount, int totalR, int totalG, int totalB, Dictionary<(byte, byte, byte), int> colorCounts)
        {
            if (aliveCount == 0)
            {
                return (0, 128, 128, 128); // Default to mid-gray if no neighbors
            }

            if (ColorBehavior == ColorBehaviorMode.AverageColor)
            {
                // Average the color components
                return (aliveCount,
                    (byte)(totalR / aliveCount),
                    (byte)(totalG / aliveCount),
                    (byte)(totalB / aliveCount));
            }
            else if (ColorBehavior == ColorBehaviorMode.MajorityColor && colorCounts?.Count > 0)
            {
                // Find the most frequent color
                var majorityColor = colorCounts.OrderByDescending(c => c.Value).First().Key;
                return (aliveCount, majorityColor.Item1, majorityColor.Item2, majorityColor.Item3);
            }

            // Fallback
            return (aliveCount, 128, 128, 128);
        }

        /// <summary>
        /// Counts the number of alive neighbors of a cell.
        /// </summary>
        /// <param name="x">Column index of the cell</param>
        /// <param name="y">Row index of the cell</param>
        /// <returns>Number of alive neighbors</returns>
        private int CountAliveNeighbors(int x, int y)
        {
            int aliveCount = 0;

            foreach (var neighbor in GetNeighborPositions(x, y))
            {
                if (Cells[neighbor.x, neighbor.y].IsAlive)
                    aliveCount++;
            }

            return aliveCount;
        }

        /// <summary>
        /// Counts alive neighbors and calculates their color information.
        /// </summary>
        /// <param name="x">Column index of the cell</param>
        /// <param name="y">Row index of the cell</param>
        /// <returns>Tuple containing alive count and average color components</returns>
        private (int aliveCount, byte avgR, byte avgG, byte avgB) CountAliveNeighborsAndGetColor(int x, int y)
        {
            int aliveCount = 0;
            int totalR = 0, totalG = 0, totalB = 0;
            var colorCounts = ColorBehavior == ColorBehaviorMode.MajorityColor ? new Dictionary<(byte, byte, byte), int>() : null;

            foreach (var neighbor in GetNeighborPositions(x, y))
            {
                Cell neighborCell = Cells[neighbor.x, neighbor.y];

                if (!neighborCell.IsAlive) continue;

                aliveCount++;
                ProcessCellColor(neighborCell, ref totalR, ref totalG, ref totalB, colorCounts);
            }

            return CalculateColorResult(aliveCount, totalR, totalG, totalB, colorCounts);
        }

        /// <summary>
        /// Creates a new grid with randomly initialized cells.
        /// </summary>
        /// <param name="cols">Number of columns</param>
        /// <param name="rows">Number of rows</param>
        /// <returns>A new Grid with random cell states</returns>
        public static Grid GetRandomGrid(int cols, int rows)
        {
            var grid = new Grid(cols, rows);

            for (var i = 0; i < cols; i++)
            {
                for (var j = 0; j < rows; j++)
                {
                    Cell cell = Cell.GetRandomCell(Grid.ColorBehavior == ColorBehaviorMode.Default);
                    grid.Cells[i, j] = cell;
                    grid.Buffer[i, j] = cell;
                }
            }

            return grid;
        }

        /// <summary>
        /// Sets the state and color of a specific cell.
        /// Used for manual cell painting in the UI.
        /// </summary>
        /// <param name="i">Column index of the cell</param>
        /// <param name="j">Row index of the cell</param>
        /// <param name="isAlive">Whether the cell should be alive</param>
        /// <param name="r">Red component</param>
        /// <param name="g">Green component</param>
        /// <param name="b">Blue component</param>
        /// <param name="a">Alpha component</param>
        public void SetCellState(int i, int j, bool isAlive, byte r, byte g, byte b, byte a = 255)
        {
            // Ensure coordinates are within bounds
            if (i >= 0 && i < Cols && j >= 0 && j < Rows)
            {
                // Update the cell in the current state
                if (isAlive)
                {
                    Cells[i, j].ComeAlive();
                    Cells[i, j].SetColor(r, g, b, a);
                }
                else
                {
                    Cells[i, j].Die();
                    Cells[i, j].SetColor(r, g, b, a);
                }

                // Also update the buffer to ensure the change persists through the next update
                Buffer[i, j] = Cells[i, j].Copy();
            }
        }
    }
}