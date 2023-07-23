using UnityEngine;
using Random = System.Random;

public class BoardController
{
    private static BoardController _instance;
    private GameState currentState;
    private Random random = new Random();
    private int turn;
    private int moveAmount;
    private Board board;
    private Player[] _players;
    private GameView view;
    private Field winfield;
    private bool isAnyPlayerWin;

    public BoardController(GameView view, Board board)
    {
        this.winfield = null;
        this.turn = 0;
        this.view = view;
        this.board = board;
        _instance = this;
        currentState = GameState.startGameState;
    }

    public int Turn => turn;

    public static BoardController GetInstance()
    {
        return _instance;
    }

    public void AddPlayer(int playerCount)
    {
        _players = new Player[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            _players[i] = new Player(i.ToString(), 0, 0);
        }
    }

    public void StartGame()
    {
        view.ShowStartGame();
        currentState = GameState.waitForPlayerState;
    }

    public void WaitForPlayer(int playerID)
    {
        view.ShowWaitForPlayer(playerID);
        currentState = GameState.rollingDiceState;
    }

    public void RunGameLoop()
    {
        while (true)
        {
            if (currentState == GameState.winState)
            {
                break;
            }

            switch (currentState)
            {
                case GameState.startGameState:
                    Invoker.GetInstance().ExecuteCommand(new StartGameCommand());
                    break;
                case GameState.waitForPlayerState:
                    Invoker.GetInstance().ExecuteCommand(new WaitForPlayerCommand(turn));
                    break;
                case GameState.rollingDiceState:
                    
                    int randomNumber = random.Next(1, 7);
                    Invoker.GetInstance().ExecuteCommand(new RolledDiceCommand(randomNumber, turn));
                    break;
                case GameState.movePlayerState:
                    Invoker.GetInstance().ExecuteCommand(new MovePlayerCommand(turn, moveAmount));
                    break;
                case GameState.climbingLadderStat:
                    currentState = GameState.changeTurnState;
                    break;
                case GameState.snakeBiteState:
                    currentState = GameState.changeTurnState;
                    break;
                case GameState.changeTurnState:
                    if (moveAmount == 6)
                    {
                        currentState = GameState.waitForPlayerState;
                    }
                    else
                    {
                        Invoker.GetInstance().ExecuteCommand(new ChangePlayerTurnCommand(turn));
                    }

                    break;
                case GameState.winState:
                    break;
            }
        }
    }

    public void RolledDice(int diceAmount, int playerID)
    {
        moveAmount = diceAmount;
        view.ShowRolledDice(diceAmount, playerID);
        currentState = GameState.movePlayerState;
    }

    public void MovePlayerForward(int moveAmount, int playerID)
    {
        MoveForward(moveAmount, playerID);

        if (IsThereAnySpecialFieldInPos(_players[playerID].X, _players[playerID].Y) == true)
        {
            switch (board.GameBoard[_players[playerID].X, _players[playerID].Y])
            {
                case Ladder ladder:
                    currentState = GameState.climbingLadderStat;
                    Invoker.GetInstance().ExecuteCommand(new ClimbingLadderCommand(playerID, ladder));
                    break;
                case Snake snake:
                    currentState = GameState.snakeBiteState;
                    Invoker.GetInstance().ExecuteCommand(new SnakeBiteCommand(playerID, snake));
                    break;
            }
        }
        else if (CheckWinState())
        {
            currentState = GameState.winState;
            Invoker.GetInstance().ExecuteCommand(new PlayerWinCommand(playerID));
        }
        else
        {
            currentState = GameState.changeTurnState;
        }
    }


    public void WinPlayer(int playerID)
    {
        view.ShowWinnerPlayer(playerID.ToString());
    }

    public void ClimbingLadder(Ladder ladder, int playerID)
    {
        int sourceX = _players[playerID].X;
        int sourceY = _players[playerID].Y;
        _players[playerID].X = ladder.Next.X;
        _players[playerID].Y = ladder.Next.Y;
        view.ShowPlayerClimbingLadder(_players[playerID].Color, board.GameBoard[sourceX, sourceY].FieldNumber,
            board.GameBoard[_players[playerID].X, _players[playerID].Y].FieldNumber);
    }

    public void BittenBySnake(Snake snake, int playerID)
    {
        int sourceX = _players[playerID].X;
        int sourceY = _players[playerID].Y;
        _players[playerID].X = snake.Next.X;
        _players[playerID].Y = snake.Next.Y;
        view.ShowPlayerBittenBySnake(_players[playerID].Color, board.GameBoard[sourceX, sourceY].FieldNumber,
            board.GameBoard[_players[playerID].X, _players[playerID].Y].FieldNumber);
    }

    public void NextTurn(int playerID)
    {
        playerID = (playerID + 1) % _players.Length;
        turn = playerID;
        currentState = GameState.waitForPlayerState;
        view.ShowChangeTurn(playerID);
    }

    private void MoveForward(int moveAmount, int playerID)
    {
        if (IsAbleToMoveForward(moveAmount, playerID) == false)
        {
            view.ShowError($"Player {_players[playerID].Color} can not move {moveAmount} field(s)");
            return;
        }

        Field nextField = board.GameBoard[_players[playerID].X, _players[playerID].Y];
        for (int i = 0; i < moveAmount; i++)
        {
            nextField = nextField.Next;
        }

        _players[playerID].X = nextField.X;
        _players[playerID].Y = nextField.Y;
        view.ShowPlayerMove(turn, nextField.FieldNumber);
    }

    private bool IsAbleToMoveForward(int moveAmount, int playerID)
    {
        if (board.BoardLength * board.BoardLength <
            board.GameBoard[_players[playerID].X, _players[playerID].Y].FieldNumber + moveAmount)
        {
            return false;
        }

        return true;
    }

    private bool IsThereAnySpecialFieldInPos(int x, int y)
    {
        if (board.GameBoard[x, y] is Ladder || board.GameBoard[x, y] is Snake)
        {
            return true;
        }

        return false;
    }

    private bool CheckWinState()
    {
        if (_players[turn].X == winfield.X && _players[turn].Y == winfield.Y)
        {
            isAnyPlayerWin = true;
            return true;
        }

        return false;
    }

    public void AssembleBoard()
    {
        Field preField = null;
        int index = board.BoardWidth * board.BoardLength;
        for (int y = board.BoardWidth - 1; y >= 0; y--)
        {
            for (int x = board.BoardLength - 1; x >= 0; x--)
            {
                int diff;
                if (y % 2 == 0)
                {
                    diff = x;
                }
                else
                {
                    diff = board.BoardLength - x - 1;
                }

                preField = board.AddField(new EmptyField(preField, index, diff, y));
                if (winfield == null)
                {
                    winfield = preField;
                }

                index--;
            }
        }

        board.AddField(new Ladder(board.GameBoard[2, 2], 2, 1, 0));
        board.AddField(new Ladder(board.GameBoard[5, 1], 5, 4, 0));
        board.AddField(new Ladder(board.GameBoard[2, 4], 9, 3, 1));
        board.AddField(new Ladder(board.GameBoard[4, 4], 18, 5, 2));
        board.AddField(new Ladder(board.GameBoard[1, 5], 25, 0, 4));

        board.AddField(new Snake(board.GameBoard[3, 0], 17, 4, 2));
        board.AddField(new Snake(board.GameBoard[5, 0], 20, 4, 3));
        board.AddField(new Snake(board.GameBoard[3, 2], 24, 0, 3));
        board.AddField(new Snake(board.GameBoard[5, 4], 32, 4, 5));
        board.AddField(new Snake(board.GameBoard[0, 1], 34, 2, 5));
    }
}