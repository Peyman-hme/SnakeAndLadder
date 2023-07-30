using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;

public class Client
{

    ISocket socket;
    private IClient client;
    ISession session;
    private IMatch currentMatch;
    private IUserPresence localUser;
    private int currentPlayerID;
    private bool isHost;

    public int CurrentPlayerID => currentPlayerID;

    private List<IUserPresence> users = new List<IUserPresence>();

    public IMatch CurrentMatch => currentMatch;

    private string currentMatchmakingTicket;
    // Start is called before the first frame update
    public async void Start()
    {
        client = new Nakama.Client("http", "127.0.0.1", 7350, "defaultkey");
        socket = client.NewSocket();
        var deviceId = PlayerPrefs.GetString("deviceId", SystemInfo.deviceUniqueIdentifier);
        Debug.Log(deviceId);
        Debug.Log(deviceId);
        session = await client.AuthenticateDeviceAsync(deviceId);

        bool appearOnline = true;
        int connectionTimeout = 30;
        Connect(socket,session);

        
    }
    
    private async void Connect(ISocket socket, ISession session)
    {
        try
        {
            if (!socket.IsConnected)
            {
                await socket.ConnectAsync(session);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error connecting socket: " + e.Message);
        }
        socket.ReceivedMatchmakerMatched += OnReceivedMatchmakerMatched;
        socket.ReceivedMatchPresence += OnReceivedMatchPresence;
        // socket.ReceivedMatchState += async m => await OnReceivedMatchState(m);
        socket.ReceivedMatchState += OnReceivedMatchState;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public async Task FindMatch(int minPlayers = 2)
    {
        // Set some matchmaking properties to ensure we only look for games that are using the Unity client.
        // This is not a required when using the Unity Nakama SDK,
        // however in this instance we are using it to differentiate different matchmaking requests across multiple platforms using the same Nakama server.
        var matchmakingProperties = new Dictionary<string, string>
        {
            { "engine", "unity" }
        };

        // Add this client to the matchmaking pool and get a ticket.
        Debug.Log("first");
        var matchmakerTicket = await socket.AddMatchmakerAsync("+properties.engine:unity", minPlayers, minPlayers, matchmakingProperties);
        Debug.Log("end");
        currentMatchmakingTicket = matchmakerTicket.Ticket;
    }

    private async void OnReceivedMatchmakerMatched(IMatchmakerMatched matched)
    {
        // Cache a reference to the local user.
        localUser = matched.Self.Presence;
        // Debug.Log(localUser);
        // Join the match.
        var match = await socket.JoinMatchAsync(matched);

        // ...
        Debug.Log($"matched received");
        // Spawn a player instance for each connected user.
        foreach (var user in match.Presences)
        {
            // SpawnPlayer(match.Id, user);
            Debug.Log($"match: {match.Id} {user}");
        }
        
        foreach (var item in matched.Users)
        {
            users.Add(item.Presence);

            Debug.Log(item.Presence);
        }

        currentPlayerID = users.IndexOf(localUser);
        Debug.Log($"id: {currentPlayerID}");
        
        if (currentPlayerID == 0)
        {
            isHost = true;
            Debug.Log("I'm host yes");
            // BoardController.GetInstance().SendStartGameCommand();

            Debug.Log("I'm host");

        }
        else
        {
            Debug.Log("ajabbb");
        }
        // Cache a reference to the current match.
        currentMatch = match;
    }
    private void OnReceivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        // For each new user that joins, spawn a player for them.
        foreach (var user in matchPresenceEvent.Joins)
        {
            // SpawnPlayer(matchPresenceEvent.MatchId, user);
            Debug.Log($"salam aleikom {user}");
        }

        // For each player that leaves, despawn their player.
        foreach (var user in matchPresenceEvent.Leaves)
        {
            // if (players.ContainsKey(user.SessionId))
            // {
            //     Destroy(players[user.SessionId]);
            //     players.Remove(user.SessionId);
            // }
            Debug.Log("aleikom salam");

        }
    }
    private void OnReceivedMatchState(IMatchState matchState)
{
    // If the incoming data is not related to this remote player, ignore it and return early.
    // if (matchState.UserPresence.SessionId != NetworkData.User.SessionId)
    // {
    //     return;
    // }
    var state = matchState.State.Length > 0 ? System.Text.Encoding.UTF8.GetString(matchState.State).FromJson<Dictionary<string, string>>() : null;
    Debug.Log("state oomad");
    // Decide what to do based on the Operation Code of the incoming state data as defined in OpCodes.
    switch (matchState.OpCode)
    {
        case OpCodes.RollDice:
            Debug.Log("get dice");
            BoardController.GetInstance().SendRollingDiceCommand();
            // Invoker.GetInstance().AddCommand(new RolledDiceCommand(Int32.Parse(state["diceAmount"]), Int32.Parse(state["playerID"])));
            break;
        case OpCodes.GameCommand:
            Debug.Log($"Command is received: {state["commandName"]} {state["command"]}");
            Invoker.GetInstance().AddCommand(ConvertCommandToProperType(state["commandName"],state["command"]));
            break;
        default:
            break;
    }
}
    public async Task SendMatchStateAsync(long opCode, string state)
    {
        await socket.SendMatchStateAsync(currentMatch.Id, opCode, state);
    }

    
    public void SendMatchState(long opCode, string state)
    {
        socket.SendMatchStateAsync(currentMatch.Id, opCode, state);
    }
    
    public void SendMatchStateToHost(long opCode, string state)
    {
        socket.SendMatchStateAsync(currentMatch.Id, opCode, state,new[]{users[0]});
    }
    public void SendMatchStateToUsers(long opCode, string state)
    {
        if (opCode == OpCodes.GameCommand && isHost)
        {
            socket.SendMatchStateAsync(currentMatch.Id, opCode, state, users);
        }
    }


    private ICommand ConvertCommandToProperType(string name,string json)
    {
        switch (name)
        {
            case CommandName.RollDice:
                return JsonUtility.FromJson<RolledDiceCommand>(json);
                break;
            case CommandName.BiteSnake:
                return JsonUtility.FromJson<SnakeBiteCommand>(json);
                break;
            case CommandName.StartGame:
                return JsonUtility.FromJson<StartGameCommand>(json);
                break;
            case CommandName.WinPlayer:
                return JsonUtility.FromJson<PlayerWinCommand>(json);
                break;
            case CommandName.WaitForPlayer:
                return JsonUtility.FromJson<WaitForPlayerCommand>(json);
                break;
            case CommandName.ChangeTurn:
                return JsonUtility.FromJson<ChangePlayerTurnCommand>(json);
                break;
            case CommandName.ClimbLadder:
                return JsonUtility.FromJson<ClimbingLadderCommand>(json);
                break;
            case CommandName.MovePlayer:
                return JsonUtility.FromJson<MovePlayerCommand>(json);
                break;
        }

        return null;
    }
   
}
