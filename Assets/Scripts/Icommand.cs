
public interface ICommand
{
    void Execute();
}

public class MovePlayerCommand : ICommand
{
    private Receiver receiver;
    private int playerID;
    private int moveAmount;

    public MovePlayerCommand(int playerID, int moveAmount)
    {
        this.playerID = playerID;
        this.moveAmount = moveAmount;
        receiver = new Receiver();
    }

    public int PlayerID => playerID;

    public void Execute()
    {
        receiver.MovePlayerForward(moveAmount,playerID);
    }

    
}

public class RolledDiceCommand : ICommand
{
    private Receiver receiver;
    private int diceAmount;
    private int playerID;

    public RolledDiceCommand(int diceAmount, int playerID)
    {
        this.diceAmount = diceAmount;
        this.playerID = playerID;
        receiver = new Receiver();
    }
 

    public int DiceAmount => diceAmount;

    public int PlayerID => playerID;

    public void Execute()
    {
        receiver.RolledDice(diceAmount,playerID);
    }
}


public class WaitForPlayerCommand : ICommand
{
    private Receiver receiver;
    private int playerID;

    public WaitForPlayerCommand(int playerID)
    {
        this.playerID = playerID;
        receiver = new Receiver();
    }
    
    public int PlayerID => playerID;

    

    public void Execute()
    {
        receiver.WaitForPlayer(playerID);
    }
}

public class ClimbingLadderCommand : ICommand
{
    private Receiver receiver;

    private int playerID;
    private Ladder ladder;

    public ClimbingLadderCommand(int playerID, Ladder ladder)
    {
        this.playerID = playerID;
        this.ladder = ladder;
        receiver = new Receiver();
    }

    

    public Ladder Ladder => ladder;

    public int PlayerID => playerID;

    public void Execute()
    {
        receiver.ClimbingLadder(ladder,playerID);
    }
}

public class SnakeBiteCommand : ICommand
{
    private Receiver receiver;

    private int playerID;
    private Snake snake;

    

    public SnakeBiteCommand(int playerID, Snake snake)
    {
        this.playerID = playerID;
        this.snake = snake;
        receiver = new Receiver();
    }

    public Snake Snake => snake;

    public int PlayerID => playerID;

    public void Execute()
    {
        receiver.SnakeBite(snake,playerID);
    }
}

public class PlayerWinCommand : ICommand
{
    private Receiver receiver;

    private int playerID;

    public PlayerWinCommand(int playerID)
    {
        this.playerID = playerID;
        receiver = new Receiver();
    }

    public int PlayerID => playerID;
    
    public void Execute()
    {
        receiver.WinPlayer(playerID);
    }
}

public class StartGameCommand : ICommand
{
    private Receiver receiver;

    public StartGameCommand()
    {
        receiver = new Receiver();
    }

    public void Execute()
    {
        receiver.StartGame();
    }
}

public class ChangePlayerTurnCommand : ICommand
{
    private Receiver receiver;
    private int playerID;

    public ChangePlayerTurnCommand(int playerID)
    {
        this.playerID = playerID;
        receiver = new Receiver();
    }

    public void Execute()
    {
        receiver.ChangePlayerTurn(playerID);
    }
}