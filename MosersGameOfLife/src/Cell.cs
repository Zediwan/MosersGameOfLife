public class Cell
{
    private byte R { get; set; }
    private byte G { get; set; }
    private byte B { get; set; }

    public System.Windows.Media.Color GetColor() => System.Windows.Media.Color.FromRgb(R, G, B); // Get the color of the cell

    public Cell(byte r, byte g, byte b)
    {
        this.R = r;
        this.G = g;
        this.B = b;
    }

    public void SetColor(byte r, byte g, byte b)
    {
        this.R = r;
        this.G = g;
        this.B = b;
    }


    public Cell Copy()
    {
        return new Cell(this.R, this.G, this.B);
    }

    public static Cell GetRandomCell()
    {
        return new Cell((byte)new Random().Next(0, 256), (byte)new Random().Next(0, 256), (byte)new Random().Next(0, 256));
    }
}