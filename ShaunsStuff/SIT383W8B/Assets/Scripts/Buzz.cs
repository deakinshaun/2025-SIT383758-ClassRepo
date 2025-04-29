using Oculus.Haptics;
using UnityEngine;

public class Buzz : MonoBehaviour
{
    public HapticClip hapclip;

    private HapticClipPlayer player;

    void Start()
    {
        player = new HapticClipPlayer(hapclip);
 //       player.Play(Controller.Left);
        Debug.Log("Played haptics");
    }

    void Update()
    {
        
    }

    float timeRemaining = 0.0f;
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Collision");
        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0.0f)
        {
            player.Play(Controller.Left);
            timeRemaining = player.clipDuration;
        }
    }
}
