using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Receiver
{
    public void RolledDice(int diceAmount,int playerID)
    {
        BoardController.GetInstance().RolledDice(diceAmount, playerID);
    }

    public void MovePlayerForward(int diceAmount, int playerID)
    {
        BoardController.GetInstance().MovePlayerForward(diceAmount, playerID);
    }

    public void ClimbingLadder(Ladder ladder, int playerID)
    {
        BoardController.GetInstance().ClimbingLadder(ladder, playerID);
    }

    public void SnakeBite(Snake snake, int playerID)
    {
        BoardController.GetInstance().BittenBySnake(snake, playerID);
    }

    public void WinPlayer(int playerID)
    {
        BoardController.GetInstance().WinPlayer(playerID);
    }

    public void StartGame()
    {
        BoardController.GetInstance().StartGame();
    }
    
    public void WaitForPlayer(int playerID)
    {
        BoardController.GetInstance().WaitForPlayer(playerID);
    }
    
    public void ChangePlayerTurn(int playerID)
    {
        BoardController.GetInstance().NextTurn(playerID);
    }
}