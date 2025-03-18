using UnityEngine;

using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;

public struct InputNetworkData : INetworkInput
{
    public float turnAmount;
    public float moveAmount;
}

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public GameObject avatarPrefab;

    private NetworkRunner networkRunner;
    private InputSystem_Actions controls;

    void Start()
    {
        controls = new InputSystem_Actions();
        controls.Enable();
    }
    public void startServer ()
    {
        startNetwork(GameMode.Host);
    }

    public void startClient ()
    {
        startNetwork(GameMode.Client);
    }

    private async void startNetwork (GameMode gameMode)
    {
        Debug.Log("Starting network as " + gameMode);
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        await networkRunner.StartGame(new StartGameArgs() { GameMode = gameMode, SessionName = "SampleRoom"});

        networkRunner.ProvideInput = true;
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player joined " + player.PlayerId);

        if (networkRunner.IsServer)
        {
            NetworkObject avatar = networkRunner.Spawn(avatarPrefab, Vector3.zero, Quaternion.identity, player);
            networkRunner.SetPlayerObject(player, avatar);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
      //  Debug.Log("OnInput");
        InputNetworkData inputData = new InputNetworkData();
        inputData.turnAmount = controls.Player.Move.ReadValue<Vector2>().x;
        inputData.moveAmount = controls.Player.Move.ReadValue<Vector2>().y;
        input.Set(inputData);
    }

    public void updateNickName (string name)
    {
        Debug.Log("Updating nickname " + name);
        networkRunner.GetPlayerObject(networkRunner.LocalPlayer).gameObject.GetComponent<AvatarName>().setName(name);
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }


    void Update()
    {
        
    }
}
