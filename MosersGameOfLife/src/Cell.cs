using System.Windows.Media;

public class Cell
{
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public bool IsAlive { get; set; } // State of the cell

    public Cell(byte r, byte g, byte b, bool isAlive = false)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.IsAlive = isAlive;
    }

    public Color GetColor()
    {
        return IsAlive ? Color.FromRgb(R, G, B) : Colors.White;
    }

    public void SetColor(byte r, byte g, byte b)
    {
        this.R = r;
        this.G = g;
        this.B = b;
    }

    public Cell Copy()
    {
        return new Cell(this.R, this.G, this.B, this.IsAlive);
    }

    public static Cell GetRandomCell()
    {
        Random random = new Random();
        return new Cell(
            (byte)random.Next(256), // Random Red
            (byte)random.Next(256), // Random Green
            (byte)random.Next(256), // Random Blue
            random.NextDouble() < 0.1 // 50% chance to be alive
        );
    }
}
