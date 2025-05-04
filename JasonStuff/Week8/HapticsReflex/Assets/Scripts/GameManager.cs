using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;


public class GameManager : MonoBehaviour
{
    public List<VRButton> buttons;
    public float gameDuration = 60f;
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 3f;

    private float gameTimer;
    private int score = 0;
    private float reactionStartTime;

    void Start()
    {
        gameTimer = gameDuration;
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (gameTimer > 0)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
            int index = Random.Range(0, buttons.Count);
            reactionStartTime = Time.time;

            buttons[index].Activate(() =>
            {
                float reactionTime = Time.time - reactionStartTime;

                if (reactionTime < 1.0f)
                {
                    score++;
                    TriggerHaptics(true);
                    
                }
                else
                {
                    TriggerHaptics(false);
                    
                }
            });

            gameTimer -= Time.deltaTime;
        }

        
    }

    void TriggerHaptics(bool strong)
    {
        float frequency = strong ? 1.0f : 0.2f;
        float amplitude = strong ? 1.0f : 0.2f;
        float duration = strong ? 0.3f : 0.1f;

       
        OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);

        StartCoroutine(StopHapticsAfter(duration));
    }

    IEnumerator StopHapticsAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }
}
