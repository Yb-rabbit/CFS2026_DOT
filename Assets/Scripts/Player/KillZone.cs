using UnityEngine;

public class KillZone : MonoBehaviour
{
    public Vector3 respawnPoint = new Vector3(0, 1f, 0);

    private void OnTriggerEnter(Collider other)
    {
        // 1. 向上查找玩家脚本
        FirstPersonController player = other.GetComponentInParent<FirstPersonController>();

        if (player != null)
        {
            Debug.Log("重生中...");

            // 2. 移动位置
            player.transform.position = respawnPoint;
            
            // 3. 重置旋转
            player.transform.rotation = Quaternion.Euler(0, 0, 0);

            // 4. 重置 CharacterController
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                cc.enabled = true;
            }

            // 5. 重置鼠标状态
            // 我们不直接改 Cursor.lockState，而是找 CursorSwitch 脚本来改
            // 这样能保证 _isVisible 内部变量和 3D 光标状态同步更新
            ResetCursorState();
        }
    }

    void ResetCursorState()
    {
        // 尝试在场景中找到 CursorSwitch 脚本
        CursorSwitch cursorSwitch = FindObjectOfType<CursorSwitch>();

        if (cursorSwitch != null)
        {
            // IsCursorVisible = false 代表：隐藏光标、锁定鼠标（FPS 游戏模式）
            cursorSwitch.IsCursorVisible = false; 
        }
        else
        {
            // 如果场景里没有 CursorSwitch 脚本，为了保险起见，强制锁定一下系统鼠标
            Debug.LogWarning("未找到 CursorSwitch，强制锁定系统鼠标。");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
