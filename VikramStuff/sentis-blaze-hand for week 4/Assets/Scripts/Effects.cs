using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class Effects : MonoBehaviour
{
    public static Effects effects { get; private set; }

    GameObject sprak;
    public GameObject pinchSpark;
    public AudioClip pinchSprakClip;
    private AudioSource pinchSparkAudio;
    public GameObject FT;
    GameObject FTspawned;

    private bool isOnCooldown = false;
    private bool flameThrower = false;

    Vector3 flamePos;
    private void Awake()
    {
        if (effects != null && effects != this)
        {
            Destroy(gameObject);
            return;
        }

        effects = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        pinchSparkAudio = GetComponent<AudioSource>();
    }

    public void SetFlameThrower(bool flameT, Vector3 pos, Quaternion rot)
    {
        flameThrower = flameT;
        flamePos = pos;
        FlameThrowerFun(pos, rot);
    }

    private void FlameThrowerFun(Vector3 pos, Quaternion rot)
    {
        if (FTspawned == null)
        {
            FTspawned = Instantiate(FT, pos, rot);
            FTspawned.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            FTspawned.transform.localRotation *= Quaternion.Euler(-90f, 0f, 0f);
        }

        var ps = FTspawned.GetComponent<ParticleSystem>();

        
        if (flameThrower)
        {
            FTspawned.transform.position = pos;

            if (!ps.isPlaying)
            {
                ps.Play();
            }
        }
        else
        {
            if (ps.isPlaying)
            {
                ps.Stop();
            }
        }
    }

    private void FixedUpdate()
    {
        if (FTspawned != null)
        {
            FTspawned.transform.position = new Vector3(flamePos.x, flamePos.y, flamePos.z);
        }
    }

    public void MakeSpark(Vector3 position, Quaternion rotation)
    {
        if (isOnCooldown) return; 

        isOnCooldown = true; 
        StartCoroutine(CooldownTimer());

        if (sprak != null)
        {
            sprak.GetComponent<ParticleSystem>().Play();
            sprak.transform.position = position;
        }
        else
        {
            sprak = Instantiate(pinchSpark, position, rotation);
        }

        sprak.transform.localScale = new Vector3(.1f, .1f, .1f);
        if (pinchSparkAudio != null && pinchSprakClip != null)
            pinchSparkAudio.PlayOneShot(pinchSprakClip);

        StartCoroutine(DisableSpark());
    }

    private IEnumerator DisableSpark()
    {
        yield return new WaitForSeconds(0.5f);
        sprak?.GetComponent<ParticleSystem>().Stop();
    }

    private IEnumerator CooldownTimer()
    {
        yield return new WaitForSeconds(.7f); // 1 second cooldown
        isOnCooldown = false;
    }

    
}
