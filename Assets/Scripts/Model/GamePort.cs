
using System;

public interface GamePort
{
   void SetBoard(Board board,Action OnComplete);
   void AddPlayer(int playerCount,Action OnComplete);
   void RollDice(int diceAmount,Action OnComplete);
   void MoveForwardPlayer(int playerID,int moveAmount,int sourceX, int sourceY, Action<Field> OnComplete,Field field);
   void MoveForwardPlayer(int playerID,int moveAmount,int sourceX, int sourceY, Action OnComplete,Field field);
   void PlayerBittenBySnake(int playerID,int sourceX, int sourceY, Action OnComplete);
   void PlayerClimbingLadder(int playerID,int sourceX, int sourceY, Action OnComplete);
   void ShowChangePlayer(int playerID, Action OnComplete);
}
