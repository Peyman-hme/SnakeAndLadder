

public abstract class Field
{
    private Field next;
    private int fieldNumber;
    private int x;
    private int y;

    public Field(Field next, int fieldNumber, int x, int y)
    {
        this.next = next;
        this.fieldNumber = fieldNumber;
        this.x = x;
        this.y = y;
    }

    public int FieldNumber => fieldNumber;

    public Field Next => next;

    public int X => x;

    public int Y => y;
}

public class EmptyField : Field
{
    public EmptyField(Field next, int fieldNumber, int x, int y) : base(next, fieldNumber, x, y)
    {
    }
}

public class Ladder : Field
{
    
    public Ladder(Field next, int fieldNumber, int x, int y) : base(next, fieldNumber, x, y)
    {

    }
    
}

public class Snake : Field
{

    public Snake(Field next, int fieldNumber, int x, int y) : base(next, fieldNumber, x, y)
    {
        
    }


}
