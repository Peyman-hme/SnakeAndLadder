public class Board
{
   private Field[,] gameBoard = new Field[10,10];

   public void AddLadder(Ladder ladder, int x, int y)
   {
      gameBoard[x, y] = ladder;
   }
   
   public void AddSnake(Snake snake, int x, int y)
   {
      gameBoard[x, y] = snake;
   }
}

