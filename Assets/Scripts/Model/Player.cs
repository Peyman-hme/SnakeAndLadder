public class Player
{
    private string color;
    private int x;
    private int y;

    public Player(string color, int x, int y)
    {
        this.color = color;
        this.x = x;
        this.y = y;
    }

    public int X
    {
        get => x;
        set => x = value;
    }

    public int Y
    {
        get => y;
        set => y = value;
    }

    public string Color => color;
}
