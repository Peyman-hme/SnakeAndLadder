
using UnityEngine;

public interface ICommand
{
    void Execute();
    string GetName();
}
[System.Serializable]
public class MovePlayerCommand : ICommand
{
    public int playerID;
    public int moveAmount;

    public MovePlayerCommand(int playerID, int moveAmount)
    {
        this.playerID = playerID;
        this.moveAmount = moveAmount;
        
    }
    

    public void Execute()
    {
        Debug.Log("move player");
        new Receiver().MovePlayerForward(moveAmount,playerID);
    }

    public string GetName()
    {
        return CommandName.MovePlayer;
    }
}
[System.Serializable]
public class RolledDiceCommand : ICommand
{
    public int diceAmount;
    public int playerID;

    public RolledDiceCommand(int diceAmount, int playerID)
    {
        this.diceAmount = diceAmount;
        this.playerID = playerID;
        
    }
 

    

    public void Execute()
    {
        Debug.Log("roll dice");

        new Receiver().RolledDice(diceAmount,playerID);
    }

    public string GetName()
    {
        return CommandName.RollDice;
    }
}

[System.Serializable]
public class WaitForPlayerCommand : ICommand
{
    public int playerID;

    public WaitForPlayerCommand(int playerID)
    {
        this.playerID = playerID;
        
    }
    
   

    

    public void Execute()
    {
        new Receiver().WaitForPlayer(playerID);
    }

    public string GetName()
    {
        return CommandName.WaitForPlayer;
    }
}
[System.Serializable]
public class ClimbingLadderCommand : ICommand
{

    public int playerID;
    public int ladderX;
    public int ladderY;

    public ClimbingLadderCommand(int playerID, int ladderX, int ladderY)
    {
        this.playerID = playerID;
        this.ladderX = ladderX;
        this.ladderY = ladderY;
    }


    public void Execute()
    {
        new Receiver().ClimbingLadder(ladderX,ladderY,playerID);
    }

    public string GetName()
    {
        return CommandName.ClimbLadder;
    }
}
[System.Serializable]
public class SnakeBiteCommand : ICommand
{

    public int playerID;
    public int snakeX;
    public int snakeY;


    public SnakeBiteCommand(int playerID, int snakeX, int snakeY)
    {
        this.playerID = playerID;
        this.snakeX = snakeX;
        this.snakeY = snakeY;
    }


    public void Execute()
    {
        new Receiver().SnakeBite(snakeX,snakeY,playerID);
    }

    public string GetName()
    {
        return CommandName.BiteSnake;
    }
}
[System.Serializable]
public class PlayerWinCommand : ICommand
{

    public int playerID;

    public PlayerWinCommand(int playerID)
    {
        this.playerID = playerID;
        
    }

    
    public void Execute()
    {
        new Receiver().WinPlayer(playerID);
    }

    public string GetName()
    {
        return CommandName.WinPlayer;
    }
}
[System.Serializable]
public class StartGameCommand : ICommand
{

    public StartGameCommand()
    {
        
    }

    public void Execute()
    {
        new Receiver().StartGame();
    }

    public string GetName()
    {
        return CommandName.StartGame;
    }
}
[System.Serializable]
public class ChangePlayerTurnCommand : ICommand
{
    public int playerID;

    public ChangePlayerTurnCommand(int playerID)
    {
        this.playerID = playerID;
    }

    public void Execute()
    {
        new Receiver().ChangePlayerTurn(playerID);
    }

    public string GetName()
    {
        return CommandName.ChangeTurn;
    }

}

public class CommandName
{
    public const string RollDice = "RollDice";
    public const string MovePlayer = "MovePlayer";
    public const string ClimbLadder = "ClimbLadder";
    public const string BiteSnake = "BiteSnake";
    public const string WinPlayer = "WinPlayer";
    public const string ChangeTurn = "ChangeTurn";
    public const string StartGame = "StartGame";
    public const string WaitForPlayer = "WaitForPlayer";
}