using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class WorldSpaceCursor : MonoBehaviour
{
    [Header("=== 引用设置 ===")]
    [Tooltip("玩家的 Transform")]
    public Transform player;

    [Header("=== 移动设置 ===")]
    [Tooltip("离玩家的最大距离（米）")]
    public float maxDistance = 5f;

    [Tooltip("跟随移动速度")]
    public float moveSpeed = 10f;

    [Tooltip("面向摄像机的平滑速度")]
    public float rotateSpeed = 10f;

    private Camera _mainCamera;
    private Plane _groundPlane; // 虚拟平面，用于检测鼠标射线

    void Start()
    {
        _mainCamera = Camera.main;
        
        // 创建一个位于玩家位置的虚拟平面，用于作为鼠标射线的“靶子”
        // 平面的法线指向摄像机（这样射线就像打在玩家面前的一堵墙上）
        UpdateCursorPlane();
    }

    void Update()
    {
        if (player == null || _mainCamera == null) return;

        // 1. 更新虚拟平面（跟随玩家移动/旋转）
        UpdateCursorPlane();

        // 2. 发射射线：从摄像机穿过鼠标位置
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        // 3. 计算射线与虚拟平面的交点（这就是鼠标指的世界坐标）
        float enter;
        Vector3 targetPoint;

        if (_groundPlane.Raycast(ray, out enter))
        {
            targetPoint = ray.GetPoint(enter);
        }
        else
        {
            // 如果射线和平面平行（极少见），默认给一个固定偏移
            targetPoint = player.position + player.forward * 2f;
        }

        // 4. === 关键功能：距离限制 ===
        float currentDistance = Vector3.Distance(targetPoint, player.position);

        if (currentDistance > maxDistance)
        {
            // 如果超出最大距离，计算从玩家指向目标的方向
            Vector3 direction = (targetPoint - player.position).normalized;
            // 强制目标点限制在最大距离的圆周上
            targetPoint = player.position + direction * maxDistance;
        }

        // 5. 平滑移动物体到目标点
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, moveSpeed * Time.deltaTime);

        // 6. 【World Space UI 必须】让物体始终面向摄像机（Billboard效果）
        // 这样光标看起来总是平对着玩家，而不是侧面朝向
        Quaternion targetRotation = Quaternion.LookRotation(_mainCamera.transform.forward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    // 更新虚拟平面，使其始终在玩家位置，并面向摄像机
    private void UpdateCursorPlane()
    {
        // 平面原点在玩家位置
        Vector3 planePoint = player.position;
        
        // 平面的法线指向摄像机（反向），这样平面像一堵墙挡在玩家面前
        Vector3 planeNormal = (_mainCamera.transform.position - player.position).normalized;

        _groundPlane = new Plane(planeNormal, planePoint);
    }
    
    // 调试用：在 Scene 视图显示射线和平面
    private void OnDrawGizmos()
    {
        if (player == null || _mainCamera == null) return;
        
        // 画绿色平面
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.position, maxDistance); // 范围
        Gizmos.DrawLine(player.position, player.position + _mainCamera.transform.forward * 2f); // 视线方向
    }
}
