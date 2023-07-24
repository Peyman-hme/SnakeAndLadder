using System;
using UnityEngine;
using Random = System.Random;

public class BoardController
{
    private static BoardController _instance;

    // private GameState currentState;
    private Random random = new Random();
    private int turn;
    private int moveAmount;
    private Board board;
    private Player[] _players;
    private View view;
    private Field winfield;
    private bool isAnyPlayerWin;
    private int playerCount;

    public BoardController(View view, Board board)
    {
        this.winfield = null;
        this.turn = 0;
        this.view = view;
        this.board = board;
        _instance = this;
        playerCount = 2;
    }

    public int Turn => turn;

    public static BoardController GetInstance()
    {
        return _instance;
    }

    public void AddPlayer()
    {
        _players = new Player[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            _players[i] = new Player(i.ToString(), 0, 0);
        }

        ((GamePort)view).AddPlayer(playerCount, SendWaitForPlayerCommand);
    }

    public void StartGame()
    {
        view.ShowStartGame();
        AssembleBoard();
    }

    public void WaitForPlayer(int playerID)
    {
        view.ShowWaitForPlayer(playerID);
        // RunGameLoop(GameState.rollingDiceState);
    }

    

    public void SendStartGameCommand()
    {
        Invoker.GetInstance().ExecuteCommand(new StartGameCommand());
    }

    public void SendWaitForPlayerCommand()
    {
        Invoker.GetInstance().ExecuteCommand(new WaitForPlayerCommand(turn));
    }

    public void SendRollingDiceCommand()
    {
        int randomNumber = random.Next(1, 7);
        Invoker.GetInstance().ExecuteCommand(new RolledDiceCommand(randomNumber, turn));
    }

    public void SendMovePlayerCommand()
    {
        Invoker.GetInstance().ExecuteCommand(new MovePlayerCommand(turn, moveAmount));
    }

    public void SendChangePlayerTurnCommand()
    {
        if (moveAmount == 6)
        {
            SendWaitForPlayerCommand();
        }
        else
        {
            Invoker.GetInstance().ExecuteCommand(new ChangePlayerTurnCommand(turn));
        }
    }

    public void SendWinPlayerCommand()
    {
        Invoker.GetInstance().ExecuteCommand(new PlayerWinCommand(turn));
    }


    public void SendClimbingLadderCommand(Field ladder)
    {
        Invoker.GetInstance().ExecuteCommand(new ClimbingLadderCommand(turn, (Ladder)ladder));
    }

    public void SendSnakeBiteCommand(Field snake)
    {
        Invoker.GetInstance().ExecuteCommand(new SnakeBiteCommand(turn, (Snake)snake));
    }

    public Field GetNextField(int x, int y)
    {
        return board.GameBoard[x, y].Next;
    }
    
    public Field GetNextFieldTeleport(int x, int y)
    {
        Debug.Log($"{x} {y}");
        if (board.GameBoard[x, y] is Ladder)
        {
            return board.GameBoard[((Ladder)board.GameBoard[x, y]).FinalDestX,((Ladder)board.GameBoard[x, y]).FinalDestY];
        }
        if (board.GameBoard[x, y] is Snake)
        {
            return board.GameBoard[((Snake)board.GameBoard[x, y]).FinalDestX,((Snake)board.GameBoard[x, y]).FinalDestY];
        }

        return null;
    }

    

    public void RolledDice(int diceAmount, int playerID)
    {
        moveAmount = diceAmount;
        ((GamePort)view).RollDice(diceAmount, SendMovePlayerCommand);
    }

    public void MovePlayerForward(int moveAmount, int playerID)
    {
        int preX = _players[turn].X;
        int preY = _players[turn].Y;
        bool isMoved = MoveForwardIfPossible(moveAmount, playerID);
        

        if (isMoved)
        {
            switch (board.GameBoard[_players[playerID].X, _players[playerID].Y])
            {
                case Ladder ladder:
                    ((GamePort)view).MoveForwardPlayer(playerID, moveAmount, preX, preY, SendClimbingLadderCommand,
                        ladder);
                    
                    break;
                case Snake snake:
                    ((GamePort)view).MoveForwardPlayer(playerID, moveAmount, preX, preY, SendSnakeBiteCommand, snake);

                    break;
                case Field field:
                    ((GamePort)view).MoveForwardPlayer(playerID, moveAmount, preX, preY, SendChangePlayerTurnCommand,
                        field);

                    break;
            }
            if (CheckWinState())
            {
                SendWinPlayerCommand();
            }
            
        }
        else
        {
            view.ShowError($"Player {_players[playerID].Color} can not move {moveAmount} field(s)");
            SendChangePlayerTurnCommand();
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
        Debug.Log($"{sourceX},{sourceY}");
        _players[playerID].X = ladder.FinalDestX;
        _players[playerID].Y = ladder.FinalDestY;
        ((GamePort)view).PlayerClimbingLadder(playerID,sourceX,sourceY,SendChangePlayerTurnCommand);

    }

    public void BittenBySnake(Snake snake, int playerID)
    {
        int sourceX = _players[playerID].X;
        int sourceY = _players[playerID].Y;
        _players[playerID].X = snake.FinalDestX;
        _players[playerID].Y = snake.FinalDestY;
        ((GamePort)view).PlayerBittenBySnake(playerID,sourceX,sourceY,SendChangePlayerTurnCommand);
    }

    public void NextTurn(int playerID)
    {
        // playerID = (playerID + 1) % _players.Length;
        // turn = playerID;
        Debug.Log("Turn Changed");
        turn = (turn + 1) % _players.Length;
        ((GamePort)view).ShowChangePlayer(playerID,SendWaitForPlayerCommand);
    }

    private bool MoveForwardIfPossible(int moveAmount, int playerID)
    {
        if (IsAbleToMoveForward(moveAmount, playerID) == false)
        {
            return false;
        }

        Field nextField = board.GameBoard[_players[playerID].X, _players[playerID].Y];
        for (int i = 0; i < moveAmount; i++)
        {
            nextField = nextField.Next;
        }

        _players[playerID].X = nextField.X;
        _players[playerID].Y = nextField.Y;
        return true;
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

        board.AddField(new Ladder(board.GameBoard[1, 0].Next, board.GameBoard[1, 0].FieldNumber, 1, 0 ,2, 2));
        board.AddField(new Ladder(board.GameBoard[4, 0].Next, board.GameBoard[4, 0].FieldNumber,4, 0 ,5, 1));
        board.AddField(new Ladder(board.GameBoard[3, 1].Next, board.GameBoard[3, 1].FieldNumber,3, 1 ,2, 4));
        board.AddField(new Ladder(board.GameBoard[5, 2].Next, board.GameBoard[5, 2].FieldNumber, 5, 2,4, 4));
        board.AddField(new Ladder(board.GameBoard[0, 4].Next, board.GameBoard[0, 4].FieldNumber, 0, 4,1, 5));

        board.AddField(new Snake(board.GameBoard[4, 2].Next, board.GameBoard[4, 2].FieldNumber, 4, 2,3, 0));
        board.AddField(new Snake(board.GameBoard[4, 3].Next, board.GameBoard[4, 3].FieldNumber, 4, 3,5, 0));
        board.AddField(new Snake(board.GameBoard[0, 3].Next, board.GameBoard[0, 3].FieldNumber, 0, 3,3, 2));
        board.AddField(new Snake(board.GameBoard[4, 5].Next, board.GameBoard[4, 5].FieldNumber, 4, 5,5, 4));
        board.AddField(new Snake(board.GameBoard[2, 5].Next, board.GameBoard[2, 5].FieldNumber, 2, 5,0, 1));

        ((GamePort)view).SetBoard(board, AddPlayer);
    }

    private void Foo()
    {
    }
}