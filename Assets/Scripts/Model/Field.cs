
using System;

[Serializable]
public abstract class Field
{
    public Field Next;
    public int FieldNumber;
    public int X;
    public int Y;

    public Field(Field next, int fieldNumber, int x, int y)
    {
        this.Next = next;
        this.FieldNumber = fieldNumber;
        this.X = x;
        this.Y = y;
    }

   
}

public class EmptyField : Field
{
    public EmptyField(Field next, int fieldNumber, int x, int y) : base(next, fieldNumber, x, y)
    {
    }
}

public class Ladder : Field
{
    public int FinalDestX;
    public int FinalDestY;

    public Ladder(Field next, int fieldNumber, int x, int y, int finalDestX, int finalDestY) : base(next, fieldNumber, x, y)
    {
        this.FinalDestX = finalDestX;
        this.FinalDestY = finalDestY;
    }

     
}

public class Snake : Field
{
    public int FinalDestX;
    public int FinalDestY;

    public Snake(Field next, int fieldNumber, int x, int y, int finalDestX, int finalDestY) : base(next, fieldNumber, x, y)
    {
        this.FinalDestX = finalDestX;
        this.FinalDestY = finalDestY;
    }
 
}
