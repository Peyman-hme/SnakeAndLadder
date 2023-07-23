using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameView : MonoBehaviour
{
    private BoardController _boardController;
    // Start is called before the first frame update
    void Awake()
    {
        _boardController = new BoardController(this,new Board(6,6));
        _boardController.AssembleBoard();
        _boardController.AddPlayer(2);
    }

    // Update is called once per frame
    void Start()
    {
        BoardController.GetInstance().RunGameLoop();
    }

    public void ShowError(string errorMessage)
    {
        Debug.Log(errorMessage);
    }

    public void ShowPlayerMove(int playerID, int fieldNumber)
    {
        Debug.Log($"Player {playerID} moved to field {fieldNumber}");
    }
    
    public void ShowRolledDice(int diceAmount,int playerID)
    {
        Debug.Log($"Dice Rolled by player {playerID} and number {diceAmount} is shown");
    }
    
    public void ShowPlayerClimbingLadder(string color, int source,int dest)
    {
        Debug.Log($"Player {color} climbed up from {source} to {dest}");
    }
    
    public void ShowPlayerBittenBySnake(string color, int source, int dest)
    {
        Debug.Log($"Player {color} bitten by snake at {source} and sent to {dest}");
    }
    
    public void ShowWinnerPlayer(string color)
    {
        Debug.Log($"Player {color} is winner");
    }
    public void ShowStartGame()
    {
        Debug.Log($"Game has been started");
    }
    
    public void ShowWaitForPlayer(int playerID)
    {
        Debug.Log($"Waiting For Player {playerID}");
    }
    public void ShowChangeTurn(int playerID)
    {
        Debug.Log($"It's Player {playerID} turn");
    }
}
