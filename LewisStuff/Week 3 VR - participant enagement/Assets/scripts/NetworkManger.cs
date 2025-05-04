using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;

public class NetworkManger : MonoBehaviour
{
    public GameObject networkControlPanel;
    public GameObject avatarPrefab;

    private NetworkRunner networkManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    public void startServer()
    {
        startNetwork(GameMode.Host);
    }

    public void startClient()
    {
        startNetwork(GameMode.Client);
    }

    public async Task startServerAndClient(GameMode mode)
    {
        networkManager = gameObject.AddComponent<NetworkRunner>();
        await networkManager.StartGame(new StartGameArgs() { GameMode = mode, SessionName = "SampleRoom" });
        networkControlPanel.SetActive(false);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player joined");
        if (networkManager.IsServer)
        {
            NetworkObject participant = networkManager.Spawn(avatarPrefab, Vector3.zero, Quaternion.identity, player);
            networkManager.SetPlayerObject(player, participant);
        }
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
public void OnInput(NetworkRunner runner, NetworkInput input)
    {       
        InputnetworkData ind = new InputnetworkData();
        ind.turnAmount = controls.player.move.redvalue<Vector2>().x;
        ind.forwardAmount = controls.player.move.readvalue<Vector2>().y;
        ind.create = controls.Player.attck.redvalue<float>() >0.0f;
        //debug.Log("got input: " + ind.forwardAmount + " " + ind.turnAmount);
        input.Set(ind);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
 public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
 public void OnConnectedToServer(NetworkRunner runner) { }
 public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
 public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
 public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
 public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
 public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
 public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
 public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
 public void OnSceneLoadDone(NetworkRunner runner) { }
 public void OnSceneLoadStart(NetworkRunner runner) { }
 public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
 public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
 public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
 public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
 public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

 public void updateNickName(string name)
 {
 // Add this only once the AvatarName class has been created.
 networkManager.GetPlayerObject(networkManager.LocalPlayer).gameObject.GetComponent<AvatarName>().setName(name);
 }

}
