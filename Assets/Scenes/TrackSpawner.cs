using System.Collections.Generic;
using UnityEngine;

public class TrackSpawner : MonoBehaviour
{
    public GameObject segmentPrefab;   // the TrackSegment prefab
    public Transform player;           // the Player transform
    public int segmentsOnScreen = 6;   // chunks kept alive at once
    public float segmentLength = 30f;  // must match the prefab's Z length

    private List<GameObject> segments = new List<GameObject>();
    private float nextSpawnZ = 0f;

    void Start()
    {
        for (int i = 0; i < segmentsOnScreen; i++)
            SpawnSegment(i > 0);   // first chunk stays clear
    }

    void Update()
    {
        if (segments.Count == 0) return;
        GameObject oldest = segments[0];
        // once the player is a full chunk past the oldest chunk, recycle it to the front
        if (player.position.z - oldest.transform.position.z > segmentLength)
            RecycleSegment();
    }

    void SpawnSegment(bool withObstacles)
    {
        GameObject seg = Instantiate(segmentPrefab,
            new Vector3(0, 0, nextSpawnZ), Quaternion.identity, transform);
        segments.Add(seg);
        nextSpawnZ += segmentLength;
        if (withObstacles) seg.GetComponent<SegmentObstacles>()?.Populate();
    }

    void RecycleSegment()
    {
        GameObject seg = segments[0];
        segments.RemoveAt(0);
        seg.transform.position = new Vector3(0, 0, nextSpawnZ);
        nextSpawnZ += segmentLength;
        segments.Add(seg);
        seg.GetComponent<SegmentObstacles>()?.Populate();
    }
}