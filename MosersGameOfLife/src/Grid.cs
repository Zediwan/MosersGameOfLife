namespace MosersGameOfLife.src
{
    public class Grid
    {
        private int Cols { get; set; }
        private int Rows { get; set; }
        public Cell[,] Cells { get; set; }

        public Grid(int cols, int rows)
        {
            this.Cols = cols;
            this.Rows = rows;
            this.Cells = new Cell[cols, rows];

            // Initialize the Cells with independent Cell objects
            for (var i = 0; i < cols; i++)
            {
                for (var j = 0; j < rows; j++)
                {
                    this.Cells[i, j] = new Cell();
                }
            }
        }

        public void Update()
        {
            // Calculate the next state for each cell
            for (var i = 0; i < this.Cols; i++)
            {
                for (var j = 0; j < this.Rows; j++)
                {
                    var cell = this.Cells[i, j];
                    var aliveNeighbors = CountAliveNeighbors(i, j);

                    // Apply the rules of the game
                    if (cell.IsDead() && aliveNeighbors == 3)
                    {
                        cell.SetAlive(); // Cell becomes alive
                    }
                    else if (cell.IsAlive() && (aliveNeighbors < 2 || aliveNeighbors > 3))
                    {
                        cell.SetDead(); // Cell dies
                    }
                    else
                    {
                        cell.SetState(cell.GetState()); // Cell remains the same
                    }
                }
            }

            // Commit the next state for all cells
            for (var i = 0; i < this.Cols; i++)
            {
                for (var j = 0; j < this.Rows; j++)
                {
                    this.Cells[i, j].CommitState();
                }
            }
        }

        private int CountAliveNeighbors(int x, int y)
        {
            var aliveCount = 0;

            // Precompute wrapped indices for neighbors
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue; // Skip the cell itself

                    // Calculate wrapped coordinates
                    var newX = (x + i + this.Cols) % this.Cols;
                    var newY = (y + j + this.Rows) % this.Rows;

                    // Count the neighbor
                    if (this.Cells[newX, newY].IsAlive())
                    {
                        aliveCount++;
                    }
                }
            }

            return aliveCount;
        }

        public static Grid GetRandomGrid(int cols, int rows)
        {
            var grid = new Grid(cols, rows);
            for (var i = 0; i < cols; i++)
            {
                for (var j = 0; j < rows; j++)
                {
                    grid.Cells[i, j] = Cell.GetRandomCell();
                }
            }

            return grid;
        }
    }
}
