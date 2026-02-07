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
            
            // 3. 重置旋转（防止重生时头朝下）
            player.transform.rotation = Quaternion.Euler(0, 0, 0);

            // 4. 【核心】重置 CharacterController
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                // 这是一个非常实用的技巧：
                // 瞬间关闭再开启，会清除控制器内部的速度和碰撞缓存
                cc.enabled = false;
                cc.enabled = true;
            }
        }
    }
}
