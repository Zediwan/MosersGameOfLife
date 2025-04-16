namespace MosersGameOfLife.src
{
    public class Grid
    {
        private int Cols { get; set; }
        private int Rows { get; set; }
        public Cell?[,] Cells { get; set; }
        private Cell?[,] Buffer { get; set; } // Buffer for the next state

        public Grid(int cols, int rows)
        {
            this.Cols = cols;
            this.Rows = rows;
            this.Cells = new Cell[cols, rows];
            this.Buffer = new Cell[cols, rows]; // Initialize the buffer
        }

        public void Update()
        {
            (Buffer, Cells) = (Cells, Buffer);
            Parallel.For(0, Cols, i =>
            {
                for (var j = 0; j < Rows; j++)
                {
                    Cell? cell = Cells[i, j];
                    int aliveNeighbors = CountAliveNeighbors(i, j);

                    if (cell == null && aliveNeighbors == 3)
                    {
                        Buffer[i, j] = CreateCellWithAverageColor(i, j);
                    }
                    else if (cell != null && (aliveNeighbors < 2 || aliveNeighbors > 3))
                    {
                        Buffer[i, j] = null;
                    }
                    else
                    {
                        Buffer[i, j] = cell;
                    }
                }
            });
        }

        private int CountAliveNeighbors(int x, int y)
        {
            int aliveCount = 0;

            // Precompute wrapped indices for neighbors
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue; // Skip the cell itself

                    // Calculate wrapped coordinates
                    int newX = (x + i + this.Cols) % this.Cols;
                    int newY = (y + j + this.Rows) % this.Rows;

                    if (this.Cells[newX, newY] != null)
                    {
                        aliveCount++;
                    }
                }
            }

            return aliveCount;
        }

        private Cell CreateCellWithAverageColor(int x, int y)
        {
            int totalR = 0, totalG = 0, totalB = 0, count = 0;

            // Iterate through neighbors
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue; // Skip the cell itself

                    // Calculate wrapped coordinates
                    var newX = (x + i + this.Cols) % this.Cols;
                    var newY = (y + j + this.Rows) % this.Rows;

                    Cell? neighbor = this.Cells[newX, newY];

                    if (neighbor == null) continue;

                    var color = neighbor.GetColor();
                    totalR += color.R;
                    totalG += color.G;
                    totalB += color.B;
                    count++;
                }
            }

            // Calculate average color
            byte avgR = (byte)(count > 0 ? totalR / count : 0);
            byte avgG = (byte)(count > 0 ? totalG / count : 0);
            byte avgB = (byte)(count > 0 ? totalB / count : 0);

            return new Cell(avgR, avgG, avgB);
        }

        public static Grid GetRandomGrid(int cols, int rows)
        {
            Random random = new Random();
            var grid = new Grid(cols, rows);
            for (var i = 0; i < cols; i++)
            {
                for (var j = 0; j < rows; j++)
                {
                    Cell? cell = null;
                    // Randomly generate a cell or not
                    if (random.NextDouble() < 0.5) // 50% chance to create a cell
                    {
                        cell = Cell.GetRandomCell();
                    }

                    grid.Cells[i, j] = cell;
                    // Initialize the buffer with the same state
                    grid.Buffer[i, j] = cell?.Copy();
                }
            }

            return grid;
        }
    }
}