using Fusion;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class CarSpawner : NetworkBehaviour
{
    public NetworkPrefabRef carPrefab;

    public int laneCount;
    public float laneSpacing = 5f;
    public int carCountPerLane = 10;
    public float spacing = 3f;
    public Vector3 gizmoSize = new Vector3(2f, 1f, 4f);

    public int seed = 1;

    public override void Spawned()
    {
        if (!Runner.IsServer) return;
        Random.InitState(seed);

        for (int j = 0; j < laneCount; j++)
        {
            float offset = Random.Range(-spacing, spacing);
            for (int i = 0; i < carCountPerLane; i++)
            {
                Vector3 spawnPos = new Vector3(j * laneSpacing, 0, offset + i * spacing);
                var car=  Runner.Spawn(carPrefab, transform.TransformPoint(spawnPos), quaternion.identity,Object.InputAuthority);
                if (i == carCountPerLane - 1)
                {
                    car.GetComponent<CarController>().StopsRandomly = true;
                }
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; // Set the Gizmos color
        Random.InitState(seed);

        for (int j = 0; j < laneCount; j++)
        {
            float offset = Random.Range(-spacing, spacing);
            for (int i = 0; i < carCountPerLane; i++)
            {
                Vector3 spawnPos = new Vector3(j * laneSpacing, 0, offset + i * spacing);
                Vector3 worldPos = transform.TransformPoint(spawnPos);

                Gizmos.DrawWireCube(worldPos, gizmoSize); // Draw a box where cars will spawn
            }
        }
    }
}