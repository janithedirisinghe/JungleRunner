using UnityEngine;

public class SegmentTransition : MonoBehaviour
{
    public GameObject startWall;
    public GameObject endWall;

    public void SetTransition(bool showStart, bool showEnd = false)
    {
        if (startWall != null) startWall.SetActive(showStart);
        if (endWall != null) endWall.SetActive(showEnd);
    }
}