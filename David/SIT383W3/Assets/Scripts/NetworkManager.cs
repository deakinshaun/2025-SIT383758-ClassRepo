using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;
public struct InputNetworkData : INetworkInput
{
   public float turnAmount;
   public float moveAmount;
}

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
   public GameObject avatarPrefab;
public Button clientButton;
public Button serverButton;
public TMP_InputField nicknameInputField;
private Camera mainCamera;
   private NetworkRunner networkRunner;
   private InputSystem_Actions controls;
   private string pendingNickname;

void Start()
{
    controls = new InputSystem_Actions();
    controls.Enable();
    
    // Get reference to the main camera
    mainCamera = Camera.main;
}
   
   public void disableUI()
   {
       clientButton.gameObject.SetActive(false);
       serverButton.gameObject.SetActive(false);
       nicknameInputField.gameObject.SetActive(false);
   }
   public void startServer()
   {
       startNetwork(GameMode.Host);
   }

   public void startClient()
   {
       startNetwork(GameMode.Client);
   }

   private async void startNetwork(GameMode gameMode)
   {
       Debug.Log("Starting network as " + gameMode);
       networkRunner = gameObject.AddComponent<NetworkRunner>();
       await networkRunner.StartGame(new StartGameArgs() { GameMode = gameMode, SessionName = "SampleRoom" });

       networkRunner.ProvideInput = true;
   }
   
public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
{
    Debug.Log("Player joined " + player.PlayerId);

    NetworkObject avatar = null;
    
    if (networkRunner.IsServer)
    {
        avatar = networkRunner.Spawn(avatarPrefab, Vector3.zero, Quaternion.identity, player);
        networkRunner.SetPlayerObject(player, avatar);
    }
    
    // If this is the local player
    if (player == runner.LocalPlayer)
    {
        // Get the player object if we didn't spawn it (client mode)
        if (avatar == null)
        {
            avatar = runner.GetPlayerObject(player);
        }
        
        // Apply pending nickname if exists
        if (!string.IsNullOrEmpty(pendingNickname) && avatar != null)
        {
            try
            {
                Debug.Log("Applying pending nickname: " + pendingNickname);
                avatar.gameObject.GetComponent<AvatarName>().setName(pendingNickname);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to apply nickname: " + e.Message);
            }
        }
        
        // Switch to avatar camera
        SwitchToAvatarCamera(avatar.gameObject);
    }
}
private void SwitchToAvatarCamera(GameObject avatar)
{
    if (avatar != null)
    {
       
        Camera avatarCamera = avatar.GetComponentInChildren<Camera>();
        
        if (avatarCamera != null)
        {
            // Disable main camera
            if (mainCamera != null)
            {
                mainCamera.gameObject.SetActive(false);
            }
            
            // Enable avatar camera
            avatarCamera.gameObject.SetActive(true);
            disableUI();
            Debug.Log("Switched to avatar camera");
        }
        else
        {
            Debug.LogWarning("No camera found on avatar prefab");
        }
    }
}

   public void updateNickName(string name)
   {
       Debug.Log("Updating nickname " + name);
       
       pendingNickname = name;
       
       if (networkRunner != null && networkRunner.IsRunning && networkRunner.LocalPlayer != null)
       {
           try
           {
               networkRunner.GetPlayerObject(networkRunner.LocalPlayer).gameObject.GetComponent<AvatarName>().setName(name);
           }
           catch
           {
               Debug.Log("Network not ready yet, nickname will be applied when connected");
           }
       }
       else
       {
           Debug.Log("Network not ready yet, nickname will be applied when connected");
       }
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
