﻿namespace MosersGameOfLife.src
{
    public class Grid
    {
        private int Cols { get; set; }
        private int Rows { get; set; }
        public Cell[,] Cells { get; set; }
        private Cell[,] Buffer { get; set; } // Buffer for the next state

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
                    var (aliveNeighbors, avgR, avgG, avgB) = CountAliveNeighbors(i, j);

                    if (!cell.IsAlive && aliveNeighbors == 3)
                    {
                        // Create a new cell with the average color of neighbors
                        Buffer[i, j].IsAlive = true;
                        Buffer[i, j].SetColor(avgR, avgG, avgB);
                    }
                    else if (cell.IsAlive && (aliveNeighbors < 2 || aliveNeighbors > 3))
                    {
                        Buffer[i, j].IsAlive = false;
                    }
                    else
                    {
                        Buffer[i, j] = cell.Copy();
                    }
                }
            });
        }

        private (int aliveCount, byte avgR, byte avgG, byte avgB) CountAliveNeighbors(int x, int y)
        {
            int aliveCount = 0;
            int totalR = 0, totalG = 0, totalB = 0;

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

                    var color = neighborCell.GetColor();
                    totalR += color.R;
                    totalG += color.G;
                    totalB += color.B;
                }
            }

            // Calculate average color
            byte avgR = (byte)(aliveCount > 0 ? totalR / aliveCount : 128); // Default to mid-gray
            byte avgG = (byte)(aliveCount > 0 ? totalG / aliveCount : 128);
            byte avgB = (byte)(aliveCount > 0 ? totalB / aliveCount : 128);

            return (aliveCount, avgR, avgG, avgB);
        }

        public static Grid GetRandomGrid(int cols, int rows)
        {
            var grid = new Grid(cols, rows);

            for (var i = 0; i < cols; i++)
            {
                for (var j = 0; j < rows; j++)
                {
                    Cell cell = Cell.GetRandomCell();
                    grid.Cells[i, j] = cell;
                    grid.Buffer[i, j] = cell;
                }
            }

            return grid;
        }
    }
}