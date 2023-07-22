using UnityEngine;
using Random = System.Random;

public class BoardController
{
    private int turn;
    private Board board;
    private Player[] _players;
    private GameView view;
    private Field winfield;
    private bool isAnyPlayerWin;
    public BoardController(GameView view,Board board)
    {
        this.winfield = null;
        this.turn = 0;
        this.view = view;
        this.board = board;
    }

    public void AddPlayer(int playerCount)
    {
        _players  = new Player[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            _players[i] = new Player(i.ToString(), 0, 0);
        }
        
    }
    public void RunTurn()
    {
        if (isAnyPlayerWin == false)
        {
            int moveAmount = RollDice();

            MoveForward(moveAmount);

            if (IsThereAnySpecialFieldInPos(_players[turn].X, _players[turn].Y) == true)
            {
                switch (board.GameBoard[_players[turn].X, _players[turn].Y])
                {
                    case Ladder ladder:
                        ClimbingLadder(ladder);
                        break;
                    case Snake snake:
                        BittenBySnake(snake);
                        break;
                }
            }

            if (CheckWinState())
            {
                view.ShowWinnerPlayer(turn.ToString());
            }
            else
            {
                NextTurn();
            }
        }

    }
    
    private int RollDice()
    {
        Random random = new Random();
        int randomNumber = random.Next(1, 7);
        view.ShowRollingDice(randomNumber);
        return randomNumber;
    }

    private void ClimbingLadder(Ladder ladder)
    {
        int sourceX = _players[turn].X;
        int sourceY = _players[turn].Y;
        _players[turn].X = ladder.Next.X;
        _players[turn].Y = ladder.Next.Y;
        view.ShowPlayerClimbingLadder(_players[turn].Color,board.GameBoard[sourceX,sourceY].FieldNumber,board.GameBoard[_players[turn].X,_players[turn].Y].FieldNumber);
        
    }
    
    private void BittenBySnake(Snake snake)
    {
        int sourceX = _players[turn].X;
        int sourceY = _players[turn].Y;
        _players[turn].X = snake.Next.X;
        _players[turn].Y = snake.Next.Y;
        view.ShowPlayerBittenBySnake(_players[turn].Color,board.GameBoard[sourceX,sourceY].FieldNumber,board.GameBoard[_players[turn].X,_players[turn].Y].FieldNumber);
        
    }
    
    private void NextTurn()
    {
        turn = (turn + 1) % _players.Length;
    }

    private void MoveForward(int moveAmount)
    {
        if (IsAbleToMoveForward(moveAmount) == false)
        {
            view.ShowError($"Player {_players[turn].Color} can not move {moveAmount} field(s)");
            return;
        }

        Field nextField=board.GameBoard[_players[turn].X,_players[turn].Y];
        for (int i = 0; i < moveAmount; i++)
        {
            nextField = nextField.Next;
        }

        _players[turn].X = nextField.X;
        _players[turn].Y = nextField.Y;
        view.ShowPlayerMove(_players[turn].Color,nextField.FieldNumber);
    }

    private bool IsAbleToMoveForward(int moveAmount)
    {
        if (board.BoardLength*board.BoardLength < board.GameBoard[_players[turn].X,_players[turn].Y].FieldNumber+moveAmount)
        {
            return false;
        }

        return true;
    }

    private bool IsThereAnySpecialFieldInPos(int x, int y)
    {
        if (board.GameBoard[x,y] is Ladder || board.GameBoard[x,y] is Snake)
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
        int index = board.BoardWidth*board.BoardLength;
        for (int y = board.BoardWidth-1; y >= 0 ; y--)
        {
            for (int x = board.BoardLength-1; x >= 0 ; x--)
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
                preField = board.AddField(new EmptyField(preField,index,diff, y));
                if (winfield==null)
                {
                    winfield = preField;
                }
                index--;
            }
        }
        
        board.AddField(new Ladder(board.GameBoard[2,2],2,1,0));
        board.AddField(new Ladder(board.GameBoard[5,1],5,4,0));
        board.AddField(new Ladder(board.GameBoard[2,4],9,3,1));
        board.AddField(new Ladder(board.GameBoard[4,4],18,5,2));
        board.AddField(new Ladder(board.GameBoard[1,5],25,0,4));
        
        board.AddField(new Snake(board.GameBoard[3,0],17,4,2));
        board.AddField(new Snake(board.GameBoard[5,0],20,4,3));
        board.AddField(new Snake(board.GameBoard[3,2],24,0,3));
        board.AddField(new Snake(board.GameBoard[5,4],32,4,5));
        board.AddField(new Snake(board.GameBoard[0,1],34,2,5));

    }
}
