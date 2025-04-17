using System.Windows.Media;

public class Cell
{
    private static readonly double SpawningChance = 0.1;

    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; } // Alpha channel for transparency
    public bool HasTrail => !IsAlive && A > 0;
    public bool IsAlive { get; set; } // State of the cell


    public Cell(byte r, byte g, byte b, bool isAlive = false)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = 0; // Default alpha value
        this.IsAlive = isAlive;
    }

    public Cell(byte r, byte g, byte b, byte a, bool isAlive)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = a;
        this.IsAlive = isAlive;
    }

    public Color GetColor()
    {
        return Color.FromArgb(A, R, G, B);
    }

    public void SetColor(byte r, byte g, byte b, byte a = 255)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = a;
    }

    public Cell Copy()
    {
        return new Cell(this.R, this.G, this.B, this.A, this.IsAlive);
    }

    public static Cell GetRandomCell(bool black = false)
    {
        Random random = new Random();
        if (black)
        {
            return new Cell(0, 0, 0, random.NextDouble() < Cell.SpawningChance);
        }
        return new Cell( (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256), random.NextDouble() < Cell.SpawningChance);
    }

    public void FadeTrail()
    {
        if (A > 0)
        {
            A = (byte)Math.Max(0, A - 10);
        }
    }

    public void Die()
    {
        this.IsAlive = false;
        ActivateTrail();
    }

    public void ComeAlive()
    {
        this.IsAlive = true;
        DeactivateTrail();
    }

    private void ActivateTrail()
    {
        this.A = 200; // Reset alpha to fully opaque
    }

    private void DeactivateTrail()
    {
        this.A = 0; // Set alpha to fully transparent
    }
}
