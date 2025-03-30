using Fusion;
using TMPro;
using UnityEngine;

public class AvatarName : NetworkBehaviour
{
    public TextMeshPro text;

    [Networked, OnChangedRender(nameof(updateName))]
    public string nickName { get; set; }

    public void updateName()
    {
        text.text = nickName;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_UpdateNickname(string name)
    {
        nickName = name;
    }
    public void setName(string name) 
    {
        Debug.Log("Setting name to " + name);
        RPC_UpdateNickname(name);
    }
}