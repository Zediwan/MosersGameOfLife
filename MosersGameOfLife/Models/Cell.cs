using System.Windows.Media;

namespace MosersGameOfLife.Models;

/// <summary>
/// Represents an individual cell in Conway's Game of Life.
/// Each cell has a state (alive or dead) and a color.
/// </summary>
public class Cell
{
    /// <summary>
    /// Probability of a randomly generated cell being alive.
    /// </summary>
    private static readonly double SpawningChance = 0.1;

    /// <summary>
    /// Red component of the cell's color.
    /// </summary>
    public byte R { get; set; }

    /// <summary>
    /// Green component of the cell's color.
    /// </summary>
    public byte G { get; set; }

    /// <summary>
    /// Blue component of the cell's color.
    /// </summary>
    public byte B { get; set; }

    /// <summary>
    /// Alpha (transparency) component of the cell's color.
    /// </summary>
    public byte A { get; set; }

    /// <summary>
    /// Determines if the cell has a visible trail effect.
    /// A cell has a trail if it's dead but still has some opacity.
    /// </summary>
    public bool HasTrail => !IsAlive && A > 0;

    /// <summary>
    /// Whether the cell is currently alive.
    /// </summary>
    public bool IsAlive { get; set; }

    /// <summary>
    /// Creates a cell with the specified RGB color.
    /// </summary>
    /// <param name="r">Red component (0-255)</param>
    /// <param name="g">Green component (0-255)</param>
    /// <param name="b">Blue component (0-255)</param>
    /// <param name="isAlive">Whether the cell is initially alive</param>
    public Cell(byte r, byte g, byte b, bool isAlive = false)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = 0; // Default alpha value (transparent)
        this.IsAlive = isAlive;
    }

    /// <summary>
    /// Creates a cell with the specified RGBA color.
    /// </summary>
    /// <param name="r">Red component (0-255)</param>
    /// <param name="g">Green component (0-255)</param>
    /// <param name="b">Blue component (0-255)</param>
    /// <param name="a">Alpha component (0-255)</param>
    /// <param name="isAlive">Whether the cell is initially alive</param>
    public Cell(byte r, byte g, byte b, byte a, bool isAlive)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = a;
        this.IsAlive = isAlive;
    }

    /// <summary>
    /// Gets the color of the cell as a WPF Color object.
    /// </summary>
    /// <returns>The color of the cell</returns>
    public Color GetColor()
    {
        return Color.FromArgb(A, R, G, B);
    }

    /// <summary>
    /// Sets the color of the cell.
    /// </summary>
    /// <param name="r">Red component (0-255)</param>
    /// <param name="g">Green component (0-255)</param>
    /// <param name="b">Blue component (0-255)</param>
    /// <param name="a">Alpha component (0-255)</param>
    public void SetColor(byte r, byte g, byte b, byte a = 255)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = a;
    }

    /// <summary>
    /// Creates a copy of this cell with identical properties.
    /// </summary>
    /// <returns>A new Cell instance with the same properties</returns>
    public Cell Copy()
    {
        return new Cell(this.R, this.G, this.B, this.A, this.IsAlive);
    }

    /// <summary>
    /// Creates a random cell for initial grid generation.
    /// </summary>
    /// <param name="black">If true, creates a black cell. Otherwise, creates a random colored cell.</param>
    /// <returns>A randomly generated Cell</returns>
    public static Cell GetRandomCell(bool black = false)
    {
        Random random = new Random();
        if (black)
        {
            return new Cell(0, 0, 0, random.NextDouble() < Cell.SpawningChance);
        }
        return new Cell(
            (byte)random.Next(256),
            (byte)random.Next(256),
            (byte)random.Next(256),
            random.NextDouble() < Cell.SpawningChance);
    }

    /// <summary>
    /// Fades the trail effect of the cell over time.
    /// Gradually reduces opacity and color brightness.
    /// </summary>
    public void FadeTrail()
    {
        if (A > 0)
        {
            A = (byte)Math.Max(0, A - 10);  // Reduce alpha
            R = (byte)Math.Max(0, R - 5);   // Gradually reduce red
            G = (byte)Math.Max(0, G - 5);   // Gradually reduce green
            B = (byte)Math.Max(0, B - 5);   // Gradually reduce blue
        }
    }

    /// <summary>
    /// Marks the cell as dead and activates its trail effect.
    /// </summary>
    public void Die()
    {
        this.IsAlive = false;
        ActivateTrail();
    }

    /// <summary>
    /// Marks the cell as alive and deactivates any trail effect.
    /// </summary>
    public void ComeAlive()
    {
        this.IsAlive = true;
        DeactivateTrail();
    }

    /// <summary>
    /// Activates the trail effect by setting the alpha value.
    /// </summary>
    private void ActivateTrail()
    {
        this.A = 254; // Reset alpha to nearly opaque
    }

    /// <summary>
    /// Deactivates the trail effect by setting alpha to transparent.
    /// </summary>
    private void DeactivateTrail()
    {
        this.A = 0; // Set alpha to fully transparent
    }
}
