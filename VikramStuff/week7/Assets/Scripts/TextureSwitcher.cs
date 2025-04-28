using System.Collections.Generic;
using UnityEngine;

public class TextureSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class RoomTextureSet
    {
        public Texture leftEyeTexture;
        public Texture rightEyeTexture;
    }

    public List<RoomTextureSet> roomTextures = new List<RoomTextureSet>();
    public Material sphereMaterial; 
    private int currentRoomIndex = 0;

    private void Start()
    {
        UpdateTextures();
    }

    public void NextRoom()
    {
        currentRoomIndex = (currentRoomIndex + 1) % roomTextures.Count;
        UpdateTextures();
    }

    public void PreviousRoom()
    {
        currentRoomIndex--;
        if (currentRoomIndex < 0)
            currentRoomIndex = roomTextures.Count - 1;

        UpdateTextures();
    }

    private void UpdateTextures()
    {
        Debug.Log("heyy");
        RoomTextureSet currentSet = roomTextures[currentRoomIndex];
        sphereMaterial.SetTexture("_LeftMainTex", currentSet.leftEyeTexture);
        sphereMaterial.SetTexture("_RightMainTex", currentSet.rightEyeTexture);
    }
}
