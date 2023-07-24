using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class View : MonoBehaviour
{
    public abstract  void ShowError(string errorMessage);

    public abstract  void ShowPlayerMove(int playerID, int fieldNumber);

    public abstract  void ShowRolledDice(int diceAmount, int playerID);

    public abstract void ShowPlayerClimbingLadder(string color, int source, int dest);

    public abstract void ShowPlayerBittenBySnake(string color, int source, int dest);

    public abstract void ShowWinnerPlayer(string color);

    public abstract void ShowStartGame();

    public abstract void ShowWaitForPlayer(int playerID);

    public abstract void ShowChangeTurn(int playerID);
}
