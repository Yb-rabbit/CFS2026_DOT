using UnityEngine;

public class SmartPatrol : MonoBehaviour
{
    [Header("=== 引用设置 ===")]
    [Tooltip("将玩家（通常是FirstPersonController挂载的物体）拖到这里")]
    public Transform playerTarget;

    [Header("=== 行为参数 ===")]
    [Tooltip("停止移动的安全距离（米）")]
    public float safeDistance = 3f;

    [Tooltip("追踪时的移动速度")]
    public float moveSpeed = 3f;

    [Tooltip("停下时的原地旋转速度（度/秒）")]
    public float rotateSpeed = 45f;

    void Start()
    {
        // 如果没有手动拖拽玩家，尝试自动寻找
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTarget = player.transform;
        }
    }

    void Update()
    {
        // 1. 计算与玩家的距离
        float distance = Vector3.Distance(transform.position, playerTarget.position);

        // 2. 判断逻辑
        if (distance < safeDistance)
        {
            // 【警戒模式】距离过近 -> 停止移动 -> 原地旋转
            // 绕 Y 轴旋转 (0, y, 0) = 左右水平旋转
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        }
        else
        {
            // 【追踪模式】距离较远 -> 停止旋转 -> 追踪玩家
            Vector3 direction = (playerTarget.position - transform.position).normalized;

            // 使用 World Space 移动，确保物体始终平移，不会发生倾斜
            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
            
            // 可选：追踪时让物体面朝玩家（如果不想要朝向，可以注释掉下面这行）
            // transform.LookAt(playerTarget);
        }
    }
    
    // 辅助调试：在Scene视图中画出安全距离范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, safeDistance);
    }
}
