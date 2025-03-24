using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;

public struct InputNetworkData: INetworkInput
{
    public float turnAmount;
    public float moveAmount;
}
public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _networkRunner;
    public NetworkObject avtarPrefab;
    private InputSystem_Actions _controls;
    private float _turnSpeed = 100f;
    private float _moveSpeed = 5f;

    public void StartServer()
    {
        StartNetwork(GameMode.Host);
    }

    private async void StartNetwork(GameMode host)
    {
        _networkRunner = gameObject.AddComponent<NetworkRunner>();
       await _networkRunner.StartGame(new StartGameArgs() { GameMode = host, SessionName = "SampleRoom" });
    }

    public void StartClient()
    {
        StartNetwork(GameMode.Client);
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

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        InputNetworkData inputData = new InputNetworkData();
        float h = _controls.Player.Move.ReadValue<Vector2>().x;
        float v = _controls.Player.Move.ReadValue<Vector2>().y;
        transform.rotation *= Quaternion.AngleAxis(h * _turnSpeed * Time.deltaTime, Vector3.up);
        transform.position += v * _moveSpeed * Time.deltaTime * transform.forward;
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("connected");
        if (_networkRunner.IsServer)
        {
            NetworkObject avtar = _networkRunner.Spawn(avtarPrefab, Vector3.zero, Quaternion.identity, player);
            _networkRunner.SetPlayerObject(player, avtar);
        }
        
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
        throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }

    void Start()
    {
        _controls = new InputSystem_Actions();
        _controls.Enable();
    }

   
    void Update()
    {
        
    }
}
