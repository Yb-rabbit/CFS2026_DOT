using UnityEngine;

public class CinemachinePlayerSync : MonoBehaviour
{
    [Header("=== 组件引用 ===")]
    public Transform playerBody; // 拖拽 Player 物体
    public Camera mainCamera;    // 拖拽 Cinemachine 下的 Main Camera

    void LateUpdate()
    {
        if (mainCamera == null || playerBody == null) return;

        // 1. 获取摄像机的 Y 轴旋转（水平方向）
        // 我们只取 Y 轴，因为 Player 只能左右转，不能上下弯腰
        float camYRotation = mainCamera.transform.eulerAngles.y;

        // 2. 强制让 Player 身体旋转到摄像机的角度
        // Quaternion.Euler 可以创建一个旋转，这里只改变 Y 轴
        playerBody.rotation = Quaternion.Euler(0, camYRotation, 0);
    }
}
