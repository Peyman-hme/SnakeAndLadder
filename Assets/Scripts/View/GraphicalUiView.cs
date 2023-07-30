using System;
using System.Collections;
using System.Collections.Generic;
using Nakama.TinyJson;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class GraphicalUiView : View, GamePort
{
    private BoardController _boardController;

    private GameObject[,] gameBoard;
    private List<GameObject> players = new List<GameObject>();
    private Color[] playerColors = new[] { Color.blue, Color.red, Color.yellow, Color.green };
    public GameObject singleField;
    public GameObject playerPrefab;
    public GameObject boardObj;
    public GameObject snakesObj;
    public GameObject laddersObj;
    public GameObject[] playersInfo;
    public Button rollDiceButton;
    public Text DiceNumber;
    public GameObject winPopup;

    void Awake()
    {
        _boardController = new BoardController(this, new Board(6, 6));
        gameBoard = new GameObject[6, 6];
    }

    // Update is called once per frame
    void Start()
    {
        Debug.Log(JsonUtility.ToJson(new MovePlayerCommand(5, 6)));
        Invoker.GetInstance().ExecuteCommand(new StartGameCommand());
    }

    void Update()
    {
        Invoker.GetInstance().Update();
    }

    public override void ShowError(string errorMessage)
    {
        Debug.Log(errorMessage);
    }

    public override void ShowPlayerMove(int playerID, int fieldNumber)
    {
        Debug.Log($"Player {playerID} moved to field {fieldNumber}");
    }

    public override void ShowRolledDice(int diceAmount, int playerID)
    {
        Debug.Log($"Dice Rolled by player {playerID} and number {diceAmount} is shown");
    }

    public override void ShowPlayerClimbingLadder(string color, int source, int dest)
    {
        Debug.Log($"Player {color} climbed up from {source} to {dest}");
    }

    public override void ShowPlayerBittenBySnake(string color, int source, int dest)
    {
        Debug.Log($"Player {color} bitten by snake at {source} and sent to {dest}");
    }

    public override void ShowWinnerPlayer(int playerID)
    {
        winPopup.SetActive(true);
        winPopup.transform.Find("MessageText").GetComponent<Text>().text = $"Player {playerID} wins";
        rollDiceButton.interactable = false;
    }

    public override void ShowStartGame()
    {
        Debug.Log($"Game has been started");
    }

    private void ShowWaiting(GameObject playerInfo)
    {
        playerInfo.transform.Find("Avatar").GetComponent<Image>().color = new Color32(150, 150, 150, 255);
        playerInfo.transform.Find("Name").GetComponent<Text>().color = new Color32(150, 150, 150, 255);
    }

    private void ShowPlayerTurn(GameObject playerInfo)
    {
        playerInfo.transform.Find("Avatar").GetComponent<Image>().color = Color.white;
        playerInfo.transform.Find("Name").GetComponent<Text>().color = Color.white;
    }

    public void ShowChangePlayer(int playerID, Action OnComplete)
    {
        Debug.Log("showing turn change");
        for (var i = 0; i < playersInfo.Length; i++)
        {
            if (i != playerID)
            {
                ShowWaiting(playersInfo[i]);
            }
            else
            {
                ShowPlayerTurn(playersInfo[i]);
            }
        }

        OnComplete();
    }

    public override void ShowWaitForPlayer(int playerID, int currentPlayerID)
    {
        if (playerID == currentPlayerID)
        {
            rollDiceButton.interactable = true;
        }
    }

    public override void ShowChangeTurn(int playerID)
    {
        Debug.Log($"It's Player {playerID} turn");
    }


    public void SetBoard(Board board, Action OnComplete)
    {
        int index = board.BoardWidth * board.BoardLength;
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
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


                gameBoard[diff, y] =
                    Instantiate(singleField, new Vector3(diff, y, 0), Quaternion.identity) as GameObject;
                SetFieldAttributes(gameBoard[diff, y], index, boardObj);
                index--;
            }
        }

        snakesObj.SetActive(true);
        laddersObj.SetActive(true);
        OnComplete();
    }

    public void AddPlayer(int playerCount, Action OnComplete)
    {
        for (int i = 0; i < playerCount; i++)
        {
            GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, -5), quaternion.identity) as GameObject;
            player.GetComponent<SpriteRenderer>().color = playerColors[i];
            players.Add(player);
        }

        OnComplete();
    }

    public void RollDice(int diceAmount, Action OnComplete)
    {
        Debug.Log($"showing dice number {diceAmount}");
        DiceNumber.text = diceAmount.ToString();
        OnComplete();
    }

    public void MoveForwardPlayer(int playerID, int moveAmount, int sourceX, int sourceY, Action<Field> OnComplete,
        Field field)
    {
        StartCoroutine(DoMove(playerID, moveAmount, sourceX, sourceY, OnComplete,
            field));
    }

    private IEnumerator DoMove(int playerID, int moveAmount, int sourceX, int sourceY, Action<Field> OnComplete,
        Field field)
    {
        yield return StartCoroutine(MoveSmoothly(playerID, moveAmount, sourceX, sourceY));
        OnComplete(field);
    }

    private IEnumerator DoMove(int playerID, int moveAmount, int sourceX, int sourceY, Action OnComplete)
    {
        yield return StartCoroutine(MoveSmoothly(playerID, moveAmount, sourceX, sourceY));
        OnComplete();
    }

    public void PlayerBittenBySnake(int playerID, int sourceX, int sourceY, Action OnComplete)
    {
        StartCoroutine(DoMoveSmoothlyTeleport(playerID, sourceX, sourceY, OnComplete));
    }


    private IEnumerator DoMoveSmoothlyTeleport(int playerID, int sourceX, int sourceY, Action OnComplete)
    {
        yield return StartCoroutine(MoveSmoothlyTeleport(playerID, sourceX, sourceY));
        OnComplete();
    }

    public void PlayerClimbingLadder(int playerID, int sourceX, int sourceY, Action OnComplete)
    {
        StartCoroutine(DoMoveSmoothlyTeleport(playerID, sourceX, sourceY, OnComplete));
    }


    private IEnumerator MoveSmoothly(int playerID, int moveAmount, int sourceX, int sourceY)
    {
        Vector3 prePos = players[playerID].transform.position;
        Vector3 destPos;
        for (int step = 0; step < moveAmount; step++)
        {
            Field next = _boardController.GetNextField(sourceX, sourceY);
            destPos = gameBoard[next.X, next.Y].transform.position;
            destPos.z = -5;
            for (float t = 0; t < 0.5; t += Time.deltaTime)
            {
                players[playerID].transform.position = Vector3.Lerp(prePos, destPos, t / 0.5f);
                yield return 0;
            }

            yield return new WaitForSeconds(0.5f);
            sourceX = next.X;
            sourceY = next.Y;
            prePos = destPos;
        }
    }

    private IEnumerator MoveSmoothlyTeleport(int playerID, int sourceX, int sourceY)
    {
        Vector3 prePos = players[playerID].transform.position;
        Vector3 destPos;

        Field next = _boardController.GetNextFieldTeleport(sourceX, sourceY);
        Debug.Log(next.Y);
        destPos = gameBoard[next.X, next.Y].transform.position;
        destPos.z = -5;
        for (float t = 0; t < 0.5; t += Time.deltaTime)
        {
            players[playerID].transform.position = Vector3.Lerp(prePos, destPos, t / 0.5f);
            yield return 0;
        }

        yield return new WaitForSeconds(0.5f);
        sourceX = (int)destPos.x;
        sourceY = (int)destPos.y;
        prePos = destPos;
    }

    public void MoveForwardPlayer(int playerID, int moveAmount, int sourceX, int sourceY, Action OnComplete,
        Field field)
    {
        StartCoroutine(DoMove(playerID, moveAmount, sourceX, sourceY, OnComplete));
    }

    private void SetFieldAttributes(GameObject field, int number, GameObject parent)
    {
        field.name = "Field " + number.ToString();
        field.transform.parent = parent.transform;
        field.transform.Find("Number").GetComponent<TextMesh>().text = number.ToString();
    }

    public void OnClickRollDiceButton()
    {
        DiceNumber.text = "Rolling";
        rollDiceButton.interactable = false;
        Invoke("SendRollDiceRequest", 1);
    }

    void SendRollDiceRequest()
    {
        _boardController.SendRequestToHost(OpCodes.RollDice, "");
    }

    public void TryFindMatch()
    {
        _boardController.TryFindMatch();
    }
}