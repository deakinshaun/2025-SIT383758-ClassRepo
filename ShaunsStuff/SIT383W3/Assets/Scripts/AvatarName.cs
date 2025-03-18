using Fusion;
using TMPro;
using UnityEngine;

public class AvatarName : NetworkBehaviour
{
    public TextMeshPro text;

    [Networked, OnChangedRender (nameof (updateName))]
    public string nickName { get; set; }

    public void updateName ()
    {
        // run on all the other clients.
        text.text = nickName;
    }

    [Rpc(RpcSources.InputAuthority,RpcTargets.StateAuthority)]
    public void RPC_UpdateNickname (string name)
    {
        // onto server. Set state of avatar name.
        nickName = name;
    }
    public void setName (string name) // run on the local player i.e. the client.
    {
        Debug.Log("Settng name to " + name);
        //        text.text = name;
        RPC_UpdateNickname(name);
    }
}
