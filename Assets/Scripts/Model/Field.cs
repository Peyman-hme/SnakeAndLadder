

public class Field
{
    
}

public class EmptyField : Field
{
    
}

public class Ladder : Field
{
    private int finalDestinationX;
    private int finalDestinationY;

    public Ladder(int finalDestinationX, int finalDestinationY)
    {
        this.finalDestinationX = finalDestinationX;
        this.finalDestinationY = finalDestinationY;
    }

    public int FinalDestinationX => finalDestinationX;

    public int FinalDestinationY => finalDestinationY;
}

public class Snake : Field
{
    private int finalDestinationX;
    private int finalDestinationY;

    public Snake(int finalDestinationX, int finalDestinationY)
    {
        this.finalDestinationX = finalDestinationX;
        this.finalDestinationY = finalDestinationY;
    }

    public int FinalDestinationX => finalDestinationX;

    public int FinalDestinationY => finalDestinationY;
}
