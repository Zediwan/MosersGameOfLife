public class Cell
{
    private byte State { get; set; } // Current state
    private byte NextState { get; set; } // Next state

    public bool IsAlive() => State == 1;
    public bool IsDead() => State == 0;

    public void SetAlive() => NextState = 1;
    public void SetDead() => NextState = 0;

    public byte GetState() => State;
    public void SetState(byte state) => State = state;

    public System.Windows.Media.Color GetColor() =>
        IsAlive() ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.White;

    public void CommitState() => State = NextState; // Commit the next state to the current state

    public Cell Copy() => new Cell { State = this.State };

    public static Cell GetRandomCell()
    {
        var random = new Random();
        return new Cell { State = (byte)random.Next(0, 2) };
    }
}