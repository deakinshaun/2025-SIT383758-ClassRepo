using UnityEngine;
using TMPro;
using Fusion;
public class AvatarName : NetworkBehaviour
{
    public TextMeshPro text;
    [Networked, OnChangedRender(nameof(updateName))]
    public  string nickName { get; set; }

    public void updateName()
    {
        text.text = nickName;
    }
    [Rpc(RpcSources.InputAuthority,RpcTargets.StateAuthority)]
    public void RPC_UpdateName(string name)
    {
        nickName = name;
    }
  public void SetName(string name)
  {
        Debug.Log("settin name to " + name);
        text.text = name;
        RPC_UpdateName(name);
  }

}
