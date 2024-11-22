using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner networkRunner;

    [SerializeField]
    private NetworkPrefabRef networkPrefabRef;

    [SerializeField]
    private Button hostImageButton;

    [SerializeField]
    private Button joinButton;

    [SerializeField]
    private TMP_InputField roomCodeInputField;
    
    private Dictionary<PlayerRef, NetworkObject> spawnedCharacters =
        new Dictionary<PlayerRef, NetworkObject>();

    private void Start()
    {
        // Thêm sự kiện cho các nút
        hostImageButton.onClick.AddListener(() => StartGame(GameMode.Host));
        joinButton.onClick.AddListener(JoinGame);
    }

    private string GenerateRandomSessionCode(int length = 6)
    {
        const string chars = "0123456789";
        var random = new System.Random();
        var result = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }

        return result.ToString();
    }

    async void StartGame(GameMode mode)
    {
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        networkRunner.ProvideInput = true;

        var scene = SceneRef.FromIndex(3);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Tạo mã phòng ngẫu nhiên và sử dụng làm tên phiên
        string sessionCode = GenerateRandomSessionCode();

        await networkRunner.StartGame(
            new StartGameArgs()
            {
                GameMode = mode,
                SessionName = sessionCode,
                Scene = scene,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            }
        );

    }

    void JoinGame()
    {
        string enteredCode = roomCodeInputField.text;
        StartCoroutine(ConnectToHostWithCode(enteredCode));
    }

    private IEnumerator ConnectToHostWithCode(string roomCode)
    {
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        networkRunner.ProvideInput = true;

        // Bắt đầu NetworkRunner trong chế độ Client
        yield return networkRunner.StartGame(
            new StartGameArgs
            {
                GameMode = GameMode.Client,
                SessionName = roomCode // Dùng mã phòng người dùng nhập
            }
        );
    }

    public void OnConnectedToServer(NetworkRunner runner) { }

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

        // Handle horizontal movement
        if (Input.GetKey(KeyCode.A)) 
        {
            data.direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D)) 
        {
            data.direction += Vector3.right;
        }

        data.isJumping = Input.GetKey(KeyCode.Space); 

        data.isAttacking = Input.GetKey(KeyCode.K);
        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // Khi người chơi tham gia, chuyển đến scene Player2
        if (networkRunner.IsServer)
        {
            StartCoroutine(WaitForSceneLoadAndSpawn(player));
        }
    }

    private IEnumerator WaitForSceneLoadAndSpawn(PlayerRef player)
    {
        // Wait for a short period to ensure the scene has loaded
        yield return new WaitForSeconds(1);

        // Check if the player has already spawned a character
        if (!spawnedCharacters.ContainsKey(player))
        {
            Vector3 playerPos = new Vector3(
                (player.RawEncoded % networkRunner.Config.Simulation.PlayerCount) * -5f,
                5f
            );
            NetworkObject networkObject = networkRunner.Spawn(
                networkPrefabRef,
                playerPos,
                Quaternion.identity,
                player
            );
            spawnedCharacters.Add(player, networkObject);
        }
        else
        {
            Debug.LogWarning("Player already spawned: " + player);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            spawnedCharacters.Remove(player);
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
        // Khi scene đã tải xong, spawn nhân vật cho player
        foreach (var player in runner.ActivePlayers)
        {
            OnPlayerJoined(runner, player);
        }
    }

    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
}
