using Fusion;
using TMPro;
using UnityEngine;

public class AvatarName : NetworkBehaviour
{
    [Networked,OnChangedRender(nameof(updateName))]
    public string nickName { get; set; }

    public TMP_Text text;
    public void updateName()
    {
        text.text = nickName;
    }
    
    [Rpc(RpcSources.InputAuthority,RpcTargets.StateAuthority)]
    public void RPC_updateNickname(string name)
    {
        nickName = name;
    }

    public void setName(string name)
    {
        RPC_updateNickname(name);
    }
}
