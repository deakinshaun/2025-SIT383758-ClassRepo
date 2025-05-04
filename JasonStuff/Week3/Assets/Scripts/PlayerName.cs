using UnityEngine;
using Fusion;
using TMPro;

public class PlayerName : NetworkBehaviour
{
    public TextMeshPro text;

    [Networked, OnChangedRender (nameof (updateName))]
    public string nickName { get; set; }
   
   public void updateName()
   {
     Debug.Log("I finally been called");
      text.text = nickName;
   }

     [Rpc(RpcSources.InputAuthority,RpcTargets.StateAuthority)]

    public void RPC_UpdateNickname (string name)
    {
        // onto server. Set state of avatar name.
        nickName = name;
        Debug.Log("RPC nickname has been set to " + nickName);
    }

     
        public void setName (string name) // run on the local player i.e. the client.
    {
        Debug.Log("Settng name to " + name);
             //  text.text = name;
        RPC_UpdateNickname(name);
    }
    
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  

   
    
    
    
   
   

}
