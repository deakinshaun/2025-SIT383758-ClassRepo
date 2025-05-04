using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private GameObject avatarPrefab;
    public GameObject networkControlPanel;
    public GameObject carSpawner;
    public Transform carSpawnerParent;
    public Transform[] playerSpawnPoints;
    private NetworkRunner _networkRunner;

    private InputSystem_Actions controls;

    private void Start()
    {
        controls = new();
        controls.Enable();
    }

    public void StartServer()
    {
        StartNetwork(GameMode.Host);
    }


    public void StartClient()
    {
        StartNetwork(GameMode.Client);
    }

    private async void StartNetwork(GameMode mode)
    {
        _networkRunner = gameObject.AddComponent<NetworkRunner>();
        await _networkRunner.StartGame(new StartGameArgs() { GameMode = mode, SessionName = "SampleRoom" });

        _networkRunner.ProvideInput = true;
        networkControlPanel.SetActive(false);

        if (mode == GameMode.Host)
        {
            _networkRunner.Spawn(carSpawner, carSpawnerParent.position, carSpawnerParent.rotation);
        }
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (_networkRunner.IsServer)
        {
            var playerSpawnTf = playerSpawnPoints[player.AsIndex % playerSpawnPoints.Length];
            NetworkObject participant =
                _networkRunner.Spawn(avatarPrefab, playerSpawnTf.position, Quaternion.identity, player);
            _networkRunner.SetPlayerObject(player, participant);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var ind = new InputNetworkData();
        ind.turnAmount = controls.Player.Move.ReadValue<Vector2>().x;
        ind.forwardAmount = controls.Player.Move.ReadValue<Vector2>().y;
        ind.honk = controls.Player.Attack.IsPressed();
        input.Set(ind);
    }

    public void UpdateNickName(string name)
    {
        _networkRunner.GetPlayerObject(_networkRunner.LocalPlayer).gameObject.GetComponent<AvatarName>().setName(name);
    }


    #region UnusedCallbacks

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    #endregion
}