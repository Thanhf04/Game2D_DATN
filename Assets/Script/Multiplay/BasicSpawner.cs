using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField]
    public NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters =
        new Dictionary<PlayerRef, NetworkObject>();
    private List<PlayerRef> _pendingPlayers = new List<PlayerRef>(); // List to keep track of players waiting to spawn

    public Button createRoomButton;
    public Button joinRoomButton;
    public TMP_InputField roomCodeInput;

    private NetworkRunner _runner;

    public static string RoomCode { get; private set; }

    private void Start()
    {
        createRoomButton.onClick.AddListener(OnCreateRoomButtonPressed);
        joinRoomButton.onClick.AddListener(() => OnJoinRoomButtonPressed(roomCodeInput.text));
    }

    public async void StartGame(GameMode mode, string sessionName)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await _runner.StartGame(
            new StartGameArgs()
            {
                GameMode = mode,
                SessionName = sessionName,
                Scene = scene,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            }
        );
    }

    public void OnCreateRoomButtonPressed()
    {
        RoomCode = GenerateRoomCode();
        StartGame(GameMode.Host, RoomCode);
        Debug.Log("Created room with code: " + RoomCode);

        // Switch to game scene
        SceneManager.LoadScene("Player2");
    }

    public void OnJoinRoomButtonPressed(string inputRoomCode)
    {
        StartGame(GameMode.Client, inputRoomCode);
        Debug.Log("Attempting to join room with code: " + inputRoomCode);
    }

    private string GenerateRoomCode()
    {
        return UnityEngine.Random.Range(100000, 999999).ToString();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        if (runner.IsServer)
        {
            SceneManager.LoadScene("Player2");
        }
    }

    public void OnConnectFailed(
        NetworkRunner runner,
        NetAddress remoteAddress,
        NetConnectFailedReason reason
    ) { }

    public void OnConnectRequest(
        NetworkRunner runner,
        NetworkRunnerCallbackArgs.ConnectRequest request,
        byte[] token
    ) { }

    public void OnCustomAuthenticationResponse(
        NetworkRunner runner,
        Dictionary<string, object> data
    ) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!_spawnedCharacters.ContainsKey(player))
        {
            // Add player to the pending list for spawning after scene load
            _pendingPlayers.Add(player);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnReliableDataProgress(
        NetworkRunner runner,
        PlayerRef player,
        ReliableKey key,
        float progress
    ) { }

    public void OnReliableDataReceived(
        NetworkRunner runner,
        PlayerRef player,
        ReliableKey key,
        ArraySegment<byte> data
    ) { }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        // Spawn characters for all pending players
        foreach (PlayerRef player in _pendingPlayers)
        {
            if (runner.IsServer)
            {
                Vector3 spawnPosition = new Vector3(
                    (player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3,
                    1,
                    0
                );
                NetworkObject networkPlayerObject = runner.Spawn(
                    _playerPrefab,
                    spawnPosition,
                    Quaternion.identity,
                    player
                );
                _spawnedCharacters[player] = networkPlayerObject;
            }
        }
        _pendingPlayers.Clear(); // Clear the pending list after spawning
    }

    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
}
