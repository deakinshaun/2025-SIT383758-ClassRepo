using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CarController : NetworkBehaviour
{
    [FormerlySerializedAs("speed")] public float maxSpeed = 5f;
    public float accel = 10f;
    [FormerlySerializedAs("brakeSpeed")] public float brake = 20f;
    private float speed;
    public float stopDistance = 2f; // Distance to stop behind another car
    public float raycastLength = 5f; // Max distance to check for cars ahead
    public LayerMask carLayer; // Set this to a "Car" layer in Unity
    [SerializeField] private float minStopTime;
    [SerializeField] private float maxStopTime;

    public bool StopsRandomly { get; set; }
    [Networked] private bool shouldMove { get; set; } 
    private bool isRandomlyStopping;

    private void Start()
    {
        if (HasStateAuthority)
        {
            if (StopsRandomly)
            {
                StartCoroutine(RandomStopRoutine());
            }
        }
    }

    private void Update()
    {
        if (!isRandomlyStopping)
        {
            CheckForCarAhead();
        }

        if (shouldMove)
        {
            speed += accel * Time.deltaTime;
        }
        else
        {
            speed -= brake * Time.deltaTime;
        }
        speed = Mathf.Clamp(speed, 0, maxSpeed);
        
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
    }

    private void CheckForCarAhead()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, raycastLength,
                carLayer))
        {
            float distance = hit.distance;
            shouldMove = distance > stopDistance; // Stop if too close
        }
        else
        {
            shouldMove = true; // No car ahead, keep moving
        }
    }
    
    private IEnumerator RandomStopRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0f, 5f)); // Wait before stopping

            if (!isRandomlyStopping && shouldMove)
            {
                isRandomlyStopping = true;
                shouldMove = false; // Stop the car
                yield return new WaitForSeconds(Random.Range(minStopTime, maxStopTime)); // Stop duration
                shouldMove = true; // Resume movement
                isRandomlyStopping = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Debugging Raycast in Scene View
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * raycastLength);
    }
}