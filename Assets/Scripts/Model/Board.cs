public class Board
{
   private int boardLength;
   private int boardWidth;
   private Field[,] gameBoard;

   public Board(int boardLength, int boardWidth)
   {
      this.boardLength = boardLength;
      this.boardWidth = boardWidth;
      this.gameBoard = new Field[boardLength,boardWidth];
   }

   public int BoardLength => boardLength;

   public int BoardWidth => boardWidth;

   public Field[,] GameBoard => gameBoard;

   public Field AddField(Field field)
   {
      gameBoard[field.X, field.Y] = field;
      return field;
   }
    
}

