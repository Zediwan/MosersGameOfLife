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
            (Buffer, Cells) = (Cells, Buffer);
            Parallel.For(0, Cols, i =>
            {
                for (var j = 0; j < Rows; j++)
                {
                    Cell cell = Cells[i, j];
                    var (aliveNeighbors, newR, newG, newB) = CountAliveNeighborsAndGetColor(i, j);

                    if (!cell.IsAlive && aliveNeighbors == 3)
                    {
                        Buffer[i, j].ComeAlive();

                        switch (ColorBehavior)
                        {
                            case ColorBehaviorMode.MajorityColor:
                                Buffer[i, j].SetColor(newR, newG, newB);
                                break;

                            case ColorBehaviorMode.AverageColor:
                                Buffer[i, j].SetColor(newR, newG, newB);
                                break;

                            case ColorBehaviorMode.BlackAndWhite:
                                Buffer[i, j].SetColor(0, 0, 0); // Black for alive cells
                                break;
                        }
                    }
                    else if (cell.IsAlive && (aliveNeighbors < 2 || aliveNeighbors > 3))
                    {
                        Buffer[i, j].Die();
                        Buffer[i, j].SetColor(cell.R, cell.G, cell.B, 200); // Retain color for trail
                    }
                    else
                    {
                        Buffer[i, j] = cell.Copy();
                        if (Buffer[i, j].HasTrail)
                        {
                            Buffer[i, j].FadeTrail(); // Fade the trail
                        }
                    }
                }
            });
        }

        private (int aliveCount, byte avgR, byte avgG, byte avgB) CountAliveNeighborsAndGetColor(int x, int y)
        {
            int aliveCount = 0;
            int totalR = 0, totalG = 0, totalB = 0;
            var colorCounts = ColorBehavior == ColorBehaviorMode.MajorityColor ? new Dictionary<(byte R, byte G, byte B), int>() : null;

            // Precompute wrapped indices for neighbors
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue; // Skip the cell itself

                    // Calculate wrapped coordinates
                    int newX = (x + i + this.Cols) % this.Cols;
                    int newY = (y + j + this.Rows) % this.Rows;

                    Cell neighborCell = this.Cells[newX, newY];

                    if (!neighborCell.IsAlive) continue;

                    aliveCount++;

                    if (ColorBehavior == ColorBehaviorMode.AverageColor)
                    {
                        var color = neighborCell.GetColor();
                        totalR += color.R;
                        totalG += color.G;
                        totalB += color.B;
                    }
                    else if (ColorBehavior == ColorBehaviorMode.MajorityColor)
                    {
                        var color = neighborCell.GetColor();
                        var key = (color.R, color.G, color.B);

                        if (colorCounts.ContainsKey(key))
                            colorCounts[key]++;
                        else
                            colorCounts[key] = 1;
                    }
                }
            }

            if (ColorBehavior == ColorBehaviorMode.AverageColor)
            {
                // Calculate average color
                byte avgR = (byte)(aliveCount > 0 ? totalR / aliveCount : 128); // Default to mid-gray
                byte avgG = (byte)(aliveCount > 0 ? totalG / aliveCount : 128);
                byte avgB = (byte)(aliveCount > 0 ? totalB / aliveCount : 128);

                return (aliveCount, avgR, avgG, avgB);
            }
            else if (ColorBehavior == ColorBehaviorMode.MajorityColor && colorCounts != null && colorCounts.Count > 0)
            {
                var majorityColor = colorCounts.OrderByDescending(c => c.Value).First().Key;
                return (aliveCount, majorityColor.R, majorityColor.G, majorityColor.B);
            }

            // Default to mid-gray if no color calculation is needed
            return (aliveCount, 128, 128, 128);
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