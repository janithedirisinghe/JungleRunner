using System.Collections.Generic;
using UnityEngine;

public class SegmentObstacles : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;   // elephant, tiger, etc.
    public float laneDistance = 2f;        // must match PlayerController
    public int rows = 3;                   // obstacle rows per chunk
    public float firstRowZ = 8f;           // local Z of the first row
    public float rowSpacing = 8f;          // gap between rows
    public float rowChance = 0.7f;         // chance a row has any animals
    public GameObject coinPrefab;
    private List<GameObject> spawned = new List<GameObject>();

    public void Populate()
    {
        Clear();
        for (int r = 0; r < rows; r++)
        {
            if (Random.value > rowChance) continue;       // some rows stay empty

            float z = firstRowZ + r * rowSpacing;
            int clearLane = Random.Range(0, 3);           // this lane is always open
            if (coinPrefab != null) PlaceCoin(clearLane, z);
            for (int lane = 0; lane < 3; lane++)
            {
                if (lane == clearLane) continue;          // guarantee a path
                if (Random.value < 0.5f) continue;        // don't always fill both others
                PlaceObstacle(lane, z);
            }
        }
    }

    void PlaceObstacle(int lane, float localZ)
    {
        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        GameObject obj = Instantiate(prefab, transform);
        obj.transform.localPosition = new Vector3((lane - 1) * laneDistance, 0f, localZ);
        spawned.Add(obj);
    }

    void PlaceCoin(int lane, float localZ)
    {
        GameObject c = Instantiate(coinPrefab, transform);
        c.transform.localPosition = new Vector3((lane - 1) * laneDistance, 1f, localZ);
        spawned.Add(c);   // reuses the cleanup list, so coins clear on recycle too
    }

    void Clear()
    {
        foreach (var o in spawned)
            if (o != null) Destroy(o);
        spawned.Clear();
    }
}