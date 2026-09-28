using System.Collections.Generic;
using UnityEngine;

public class TrackSpawner : MonoBehaviour
{
    [Header("Segment Prefabs")]
    public GameObject normalSegmentPrefab;
    public GameObject pondSegmentPrefab;

    [Header("Track Settings")]
    public Transform player;
    public int segmentsOnScreen = 6;
    public float segmentLength = 30f;

    [Header("Pond Mixing")]
    [Range(0f, 1f)]
    public float pondChance = 0.3f;
    public int minRunLength = 2;
    public int maxRunLength = 5;
    public int safeSegmentsAtStart = 4;

    private List<GameObject> segments = new List<GameObject>();
    private float nextSpawnZ = 0f;

    // mixing state
    private bool currentlyInPond = false;
    private int currentRunLength = 0;
    private int currentRunTarget = 3;
    private int safeSegmentsRemaining;

   
    void Start()
    {
        safeSegmentsRemaining = safeSegmentsAtStart;
        currentRunTarget = Random.Range(minRunLength, maxRunLength + 1);

        for (int i = 0; i < segmentsOnScreen; i++)
            SpawnSegment(i >= 2);
    }

    void Update()
    {
        if (segments.Count == 0) return;
        GameObject oldest = segments[0];
        if (player.position.z - oldest.transform.position.z > segmentLength)
            RecycleSegment();
    }

    GameObject PickNextPrefab()
    {
        // force normal segments at the very start
        if (safeSegmentsRemaining > 0)
        {
            safeSegmentsRemaining--;
            return normalSegmentPrefab;
        }

        currentRunLength++;

        // time to switch?
        if (currentRunLength >= currentRunTarget)
        {
            currentRunLength = 0;
            currentRunTarget = Random.Range(minRunLength, maxRunLength + 1);

            if (currentlyInPond)
            {
                // always return to normal after pond
                currentlyInPond = false;
            }
            else
            {
                // roll for pond
                currentlyInPond = (Random.value < pondChance);
            }
        }

        return currentlyInPond ? pondSegmentPrefab : normalSegmentPrefab;
    }

           private bool lastSegmentWasPond = false;

        void SpawnSegment(bool withObstacles)
    {
        GameObject prefab = PickNextPrefab();
        bool thisIsPond = (prefab == pondSegmentPrefab);
        bool typeChanged = (thisIsPond != lastSegmentWasPond);

        GameObject seg = Instantiate(prefab,
            new Vector3(0, 0, nextSpawnZ), Quaternion.identity, transform);
        segments.Add(seg);
        nextSpawnZ += segmentLength;

        // start wall: show when entering a new type
        // end wall: show when leaving pond (on the last pond segment)
        // We show end wall on previous pond when this is now normal
        bool showStartWall = typeChanged;
        bool showEndWall = false;   // handled on recycle

        seg.GetComponent<SegmentTransition>()
            ?.SetTransition(showStartWall, showEndWall);

        if (withObstacles)
            seg.GetComponent<SegmentObstacles>()?.Populate();
        seg.GetComponent<SegmentScenery>()?.Randomize();
        seg.GetComponent<SegmentFog>()?.ApplyFog();

        lastSegmentWasPond = thisIsPond;
    }

    void RecycleSegment()
    {
        GameObject seg = segments[0];
        segments.RemoveAt(0);

        GameObject prefab = PickNextPrefab();
        bool thisIsPond = (prefab == pondSegmentPrefab);
        bool typeChanged = (thisIsPond != lastSegmentWasPond);

        if (seg.name.Replace("(Clone)", "").Trim() !=
            prefab.name.Replace("(Clone)", "").Trim())
        {
            // show end wall on the outgoing segment before destroying
            seg.GetComponent<SegmentTransition>()
                ?.SetTransition(
                    seg.GetComponent<SegmentTransition>()?.startWall?.activeSelf ?? false,
                    true);

            Destroy(seg);
            seg = Instantiate(prefab,
                new Vector3(0, 0, nextSpawnZ), Quaternion.identity, transform);
        }
        else
        {
            seg.transform.position = new Vector3(0, 0, nextSpawnZ);
        }

        nextSpawnZ += segmentLength;
        segments.Add(seg);

        seg.GetComponent<SegmentTransition>()?.SetTransition(typeChanged);
        seg.GetComponent<SegmentObstacles>()?.Populate();
        seg.GetComponent<SegmentScenery>()?.Randomize();
        seg.GetComponent<SegmentFog>()?.ApplyFog();

        lastSegmentWasPond = thisIsPond;
    }
}