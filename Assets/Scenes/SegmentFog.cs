using UnityEngine;

public class SegmentFog : MonoBehaviour
{
    public Color fogColor = new Color(0.7f, 0.8f, 0.85f);
    public float fogStart = 20f;
    public float fogEnd = 60f;

    public void ApplyFog()
    {
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogStartDistance = fogStart;
        RenderSettings.fogEndDistance = fogEnd;
    }
}