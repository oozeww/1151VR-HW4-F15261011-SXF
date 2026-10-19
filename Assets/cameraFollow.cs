
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;

    [Header("摄像机位置")]
    public Vector3 offset = new Vector3(0f, 4f, -10f);

    [Header("跟随平滑度")]
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null)
            return;

        // 摄像机跟随飞机
        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        // 摄像机始终看向飞机
        transform.LookAt(target);
    }
}
