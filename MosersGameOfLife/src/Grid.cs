namespace MosersGameOfLife.src
{
    public class Grid
    {
        private int Cols { get; set; }
        private int Rows { get; set; }
        public byte[,] Cells { get; set; }
        private byte[,] Buffer { get; set; } // Reusable buffer for the new state

        public Grid(int cols, int rows)
        {
            this.Cols = cols;
            this.Rows = rows;
            this.Cells = new byte[cols, rows];
            this.Buffer = new byte[cols, rows]; // Initialize the buffer
        }

        public void Update()
        {
            // Iterate through each cell
            for (var i = 0; i < this.Cols; i++)
            {
                for (var j = 0; j < this.Rows; j++)
                {
                    // Count the number of alive neighbors
                    var state = this.Cells[i, j];
                    var aliveNeighbors = CountAliveNeighbors(i, j);

                    // Apply the rules of the game
                    if (state == 0 && aliveNeighbors == 3)
                    {
                        Buffer[i, j] = 1; // Cell becomes alive
                    }
                    else if (state == 1 && (aliveNeighbors < 2 || aliveNeighbors > 3))
                    {
                        Buffer[i, j] = 0; // Cell dies
                    }
                    else
                    {
                        Buffer[i, j] = state; // Cell remains the same
                    }
                }
            }

            // Swap the buffers (reuse memory)
            (this.Buffer, this.Cells) = (this.Cells, this.Buffer);
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
                    aliveCount += this.Cells[newX, newY];
                }
            }

            return aliveCount;
        }

        public static Grid GetRandomGrid(int cols, int rows)
        {
            var grid = new Grid(cols, rows);
            var random = new Random();
            for (var i = 0; i < cols; i++)
            {
                for (var j = 0; j < rows; j++)
                {
                    grid.Cells[i, j] = (byte)random.Next(0, 2);
                }
            }
            return grid;
        }
    }
}
