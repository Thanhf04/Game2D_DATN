// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using Fusion;
// using Fusion.Sockets;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;

// public class tets : MonoBehaviour, INetworkRunnerCallbacks
// {
//     private NetworkRunner networkRunner;

//     [SerializeField]
//     private GameObject player1Character; // Nhân vật đầu tiên

//     [SerializeField]
//     private GameObject player2Character; // Nhân vật thứ hai

//     [SerializeField]
//     private Button hostImageButton;

//     [SerializeField]
//     private Button joinButton;

//     [SerializeField]
//     private TMP_InputField roomCodeInputField;

//     private Dictionary<PlayerRef, GameObject> playerCharacterMap =
//         new Dictionary<PlayerRef, GameObject>();

//     private void Start()
//     {
//         hostImageButton.onClick.AddListener(() => StartGame(GameMode.Host));
//         joinButton.onClick.AddListener(JoinGame);
//     }

//     private string GenerateRandomSessionCode(int length = 6)
//     {
//         const string chars = "0123456789";
//         var random = new System.Random();
//         var result = new System.Text.StringBuilder(length);

//         for (int i = 0; i < length; i++)
//         {
//             result.Append(chars[random.Next(chars.Length)]);
//         }

//         return result.ToString();
//     }

//     async void StartGame(GameMode mode)
//     {
//         networkRunner = gameObject.AddComponent<NetworkRunner>();
//         networkRunner.ProvideInput = true;

//         // Tạo mã phòng ngẫu nhiên
//         string sessionCode = GenerateRandomSessionCode();

//         await networkRunner.StartGame(
//             new StartGameArgs
//             {
//                 GameMode = mode,
//                 SessionName = sessionCode,
//                 SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
//             }
//         );

//         Debug.Log("Room created with code: " + sessionCode);
//     }

//     void JoinGame()
//     {
//         string enteredCode = roomCodeInputField.text;
//         StartCoroutine(ConnectToHostWithCode(enteredCode));
//     }

//     private IEnumerator ConnectToHostWithCode(string roomCode)
//     {
//         networkRunner = gameObject.AddComponent<NetworkRunner>();
//         networkRunner.ProvideInput = true;

//         yield return networkRunner.StartGame(
//             new StartGameArgs { GameMode = GameMode.Client, SessionName = roomCode }
//         );

//         if (!networkRunner.IsConnectedToServer)
//         {
//             Debug.LogWarning("Failed to join room with code: " + roomCode);
//         }
//         else
//         {
//             Debug.Log("Successfully joined room with code: " + roomCode);
//         }
//     }

//     public void OnConnectedToServer(NetworkRunner runner) { }

//     public void OnConnectFailed(
//         NetworkRunner runner,
//         NetAddress remoteAddress,
//         NetConnectFailedReason reason
//     ) { }

//     public void OnConnectRequest(
//         NetworkRunner runner,
//         NetworkRunnerCallbackArgs.ConnectRequest request,
//         byte[] token
//     ) { }

//     public void OnCustomAuthenticationResponse(
//         NetworkRunner runner,
//         Dictionary<string, object> data
//     ) { }

//     public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

//     public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

//     public void OnInput(NetworkRunner runner, NetworkInput input) { }

//     public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

//     public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

//     public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

//     public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
//     {
//         /// Chọn nhân vật cho host và client
//         if (runner.ActivePlayers.Count() == 1) 
//         {
//             GameObject selectedCharacter = player1Character;  // Host chọn nhân vật đầu tiên
//             SpawnPlayerCharacter(player, selectedCharacter);
//         }
//         else if (runner.ActivePlayers.Count() == 2)
//         {
//             GameObject selectedCharacter = player2Character;  // Client chọn nhân vật thứ hai
//             SpawnPlayerCharacter(player, selectedCharacter);
//         }
//     }

//     private void SpawnPlayerCharacter(PlayerRef player, GameObject characterPrefab)
//     {
//         Vector3 spawnPosition = new Vector3(0, 0, 0); // Tùy chỉnh vị trí spawn
//         GameObject characterInstance = Instantiate(
//             characterPrefab,
//             spawnPosition,
//             Quaternion.identity
//         );
//         playerCharacterMap.Add(player, characterInstance);

//         // Cung cấp quyền điều khiển cho người chơi
//         characterInstance.GetComponent<Player>().SetPlayerControl(player);
//     }

//     public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
//     {
//         if (playerCharacterMap.TryGetValue(player, out GameObject character))
//         {
//             Destroy(character);
//             playerCharacterMap.Remove(player);
//         }
//     }

//     public void OnReliableDataProgress(
//         NetworkRunner runner,
//         PlayerRef player,
//         ReliableKey key,
//         float progress
//     ) { }

//     public void OnReliableDataReceived(
//         NetworkRunner runner,
//         PlayerRef player,
//         ReliableKey key,
//         ArraySegment<byte> data
//     ) { }

//     public void OnSceneLoadDone(NetworkRunner runner)
//     {
//         foreach (var player in runner.ActivePlayers)
//         {
//             OnPlayerJoined(runner, player);
//         }
//     }

//     public void OnSceneLoadStart(NetworkRunner runner) { }

//     public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

//     public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

//     public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
// }
