using UnityEngine;

public class SegmentScenery : MonoBehaviour
{
    public Transform[] sceneryItems;        // drag your rocks/trees here
    [Range(0f, 1f)]
    public float keepChance = 0.85f;        // chance each item shows per pass

    public void Randomize()
    {
        foreach (Transform item in sceneryItems)
        {
            if (item == null) continue;
            bool keep = Random.value < keepChance;
            item.gameObject.SetActive(keep);
            if (!keep) continue;
            item.localRotation = Quaternion.Euler(
                0f, Random.Range(0f, 360f), 0f);
        }
    }
}