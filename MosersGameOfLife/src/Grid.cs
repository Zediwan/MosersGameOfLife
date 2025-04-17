namespace MosersGameOfLife.src
{

    public enum ColorBehaviorMode
    {
        MajorityColor,
        AverageColor,
        BlackAndWhite
    }

    public class Grid
    {
        private int Cols { get; set; }
        private int Rows { get; set; }
        public Cell[,] Cells { get; set; }
        private Cell[,] Buffer { get; set; } // Buffer for the next state
        public static ColorBehaviorMode ColorBehavior { get; set; } = ColorBehaviorMode.BlackAndWhite;

        public Grid(int cols, int rows)
        {
            this.Cols = cols;
            this.Rows = rows;
            this.Cells = new Cell[cols, rows];
            this.Buffer = new Cell[cols, rows]; // Initialize the buffer
        }

        public void Update()
        {
            // Swap the buffers
            (Buffer, Cells) = (Cells, Buffer);

            Parallel.For(0, Cols, i =>
            {
                for (var j = 0; j < Rows; j++)
                {
                    UpdateCell(i, j);
                }
            });
        }

        private void UpdateCell(int i, int j)
        {
            Cell cell = Cells[i, j];

            int aliveNeighbors;
            byte newR = 0, newG = 0, newB = 0;

            // Performance optimization: only calculate colors when needed
            if (ColorBehavior == ColorBehaviorMode.BlackAndWhite)
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

            if (!cell.IsAlive && aliveNeighbors == 3)
            {
                // Birth rule
                Buffer[i, j].ComeAlive();
                ApplyColorByBehavior(Buffer[i, j], newR, newG, newB);
            }
            else if (cell.IsAlive && (aliveNeighbors < 2 || aliveNeighbors > 3))
            {
                // Death rule
                Buffer[i, j].Die();
                // For trail colors in BlackAndWhite mode, we might want a different approach
                if (ColorBehavior == ColorBehaviorMode.BlackAndWhite)
                    Buffer[i, j].SetColor(0, 0, 0, 200); // Black trail
                else
                    Buffer[i, j].SetColor(cell.R, cell.G, cell.B, 200); // Colored trail
            }
            else
            {
                // Survival or remain dead
                Buffer[i, j] = cell.Copy();
                if (Buffer[i, j].HasTrail)
                {
                    Buffer[i, j].FadeTrail();
                }
            }
        }

        private void ApplyColorByBehavior(Cell cell, byte r, byte g, byte b)
        {
            switch (ColorBehavior)
            {
                case ColorBehaviorMode.BlackAndWhite:
                    cell.SetColor(0, 0, 0);
                    break;
                case ColorBehaviorMode.MajorityColor:
                case ColorBehaviorMode.AverageColor:
                    cell.SetColor(r, g, b);
                    break;
            }
        }

        private IEnumerable<(int x, int y)> GetNeighborPositions(int x, int y)
        {
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    int newX = (x + i + Cols) % Cols;
                    int newY = (y + j + Rows) % Rows;
                    yield return (newX, newY);
                }
            }
        }

        private void ProcessCellColor(Cell cell, ref int totalR, ref int totalG, ref int totalB, Dictionary<(byte, byte, byte), int> colorCounts)
        {
            var color = cell.GetColor();

            if (ColorBehavior == ColorBehaviorMode.AverageColor)
            {
                totalR += color.R;
                totalG += color.G;
                totalB += color.B;
            }
            else if (ColorBehavior == ColorBehaviorMode.MajorityColor && colorCounts != null)
            {
                var key = (color.R, color.G, color.B);
                colorCounts[key] = colorCounts.TryGetValue(key, out var count) ? count + 1 : 1;
            }
        }

        private (int aliveCount, byte avgR, byte avgG, byte avgB) CalculateColorResult(
            int aliveCount, int totalR, int totalG, int totalB, Dictionary<(byte, byte, byte), int> colorCounts)
        {
            if (aliveCount == 0)
            {
                return (0, 128, 128, 128); // Default to mid-gray if no neighbors
            }

            if (ColorBehavior == ColorBehaviorMode.AverageColor)
            {
                return (aliveCount,
                    (byte)(totalR / aliveCount),
                    (byte)(totalG / aliveCount),
                    (byte)(totalB / aliveCount));
            }
            else if (ColorBehavior == ColorBehaviorMode.MajorityColor && colorCounts?.Count > 0)
            {
                var majorityColor = colorCounts.OrderByDescending(c => c.Value).First().Key;
                return (aliveCount, majorityColor.Item1, majorityColor.Item2, majorityColor.Item3);
            }

            return (aliveCount, 128, 128, 128);
        }

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

        // Full method when colors are needed
        private (int aliveCount, byte avgR, byte avgG, byte avgB) CountAliveNeighborsAndGetColor(int x, int y)
        {
            // Existing implementation
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

        public static Grid GetRandomGrid(int cols, int rows)
        {
            var grid = new Grid(cols, rows);

            for (var i = 0; i < cols; i++)
            {
                for (var j = 0; j < rows; j++)
                {
                    Cell cell = Cell.GetRandomCell(Grid.ColorBehavior == ColorBehaviorMode.BlackAndWhite);
                    grid.Cells[i, j] = cell;
                    grid.Buffer[i, j] = cell;
                }
            }

            return grid;
        }
    }
}