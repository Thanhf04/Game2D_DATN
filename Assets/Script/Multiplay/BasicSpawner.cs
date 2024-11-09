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
    private NetworkRunner networkRunner;

    [SerializeField]
    private NetworkPrefabRef networkPrefabRef;
    private Dictionary<PlayerRef, NetworkObject> spawnedCharacters =
        new Dictionary<PlayerRef, NetworkObject>();

    async void StartGame(GameMode mode)
    {
        // Tạo mới NetworkRunner
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        networkRunner.ProvideInput = true;

        var scene = SceneRef.FromIndex(4);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Đợi scene "Player2" tải xong trước khi bắt đầu game
        await networkRunner.StartGame(
            new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "MySession",
                Scene = scene,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            }
        );
    }

    private void OnGUI()
    {
        if (networkRunner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
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
        if (Input.GetKey(KeyCode.A)) // Move left
        {
            data.direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D)) // Move right
        {
            data.direction += Vector3.right;
        }

        // Handle jump input (e.g., Space key)
        data.isJumping = Input.GetKey(KeyCode.Space); // Check if space key is pressed for jumping
        
        data.isAttacking = Input.GetKey(KeyCode.K);
        if(data.isAttacking = Input.GetKey(KeyCode.K)){
            Debug.Log("danhadjdsayyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy");
        }
        data.isRolling = Input.GetKey(KeyCode.F);

        // Set the input data to be sent over the network
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
                (player.RawEncoded % networkRunner.Config.Simulation.PlayerCount) * 3.1f,
                0f
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
