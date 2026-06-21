using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float xFollow = 0.5f;   // 0 = locked center, 1 = glued to player's lane
    public float smooth = 8f;
    private Vector3 offset;

    void Start()
    {
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 pos = transform.position;
        pos.z = target.position.z + offset.z;
        float targetX = offset.x + target.position.x * xFollow;
        pos.x = Mathf.Lerp(pos.x, targetX, smooth * Time.deltaTime);
        transform.position = pos;
    }
}