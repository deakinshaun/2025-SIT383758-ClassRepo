using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections;
using System.Collections.Generic;
using System;

public struct InputNetworkData : INetworkInput
{
    public float turnAmount;
    public float moveAmount;
}

public class NetWorkManager : MonoBehaviour, INetworkRunnerCallbacks
{
     public GameObject avatarPrefab;

    private NetworkRunner networkRunner;
    private InputSystem_Actions controls;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controls = new InputSystem_Actions();
        controls.Enable();
    }

     public void startServer () 
    {
     startNetwork (GameMode.Host);
    }
    public void startClient () 
    {
     startNetwork (GameMode.Client);
    }

    private async void startNetwork (GameMode gameMode)
    {
        Debug.Log("Starting network " + gameMode);
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        await networkRunner.StartGame(new StartGameArgs() { GameMode = gameMode, SessionName = "SampleRoom"});

        networkRunner.ProvideInput = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
    {
        Debug.Log("A Player has joined " + player.PlayerId);
        if (networkRunner.IsServer)
        {
            NetworkObject avatar = networkRunner.Spawn(avatarPrefab, Vector3.zero, Quaternion.identity, player);
            networkRunner.SetPlayerObject(player, avatar);
        }
    }

    public void updateNickName (string name)
    {
        Debug.Log("Updating nickname " + name);
        networkRunner.GetPlayerObject(networkRunner.LocalPlayer).gameObject.GetComponent<PlayerName>().setName(name);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) 
    {
         InputNetworkData inputData = new InputNetworkData();
         inputData.turnAmount = controls.Player.Move.ReadValue<Vector2>().x;
         inputData.moveAmount = controls.Player.Move.ReadValue<Vector2>().y;
         input.Set(inputData);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
       // throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
       // throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }
}
