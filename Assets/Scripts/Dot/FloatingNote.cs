using UnityEngine;
using System.Collections;

public class FloatingNote : MonoBehaviour
{
    [Header("=== 飘动设置 ===")]
    [Tooltip("飘动速度")]
    public float speed = 5f;

    [Tooltip("飘动方向（默认向前，即Z轴正方向）")]
    public Vector3 moveDirection = Vector3.forward;

    [Tooltip("飘动多远后销毁（距离）")]
    public float maxDistance = 50f;

    private Vector3 _startPosition;

    void Start()
    {
        // 记录生成时的初始位置
        _startPosition = transform.position;
        
        // 随机化一点旋转，让它飘起来更自然
        transform.rotation = Random.rotation;
    }

    void Update()
    {
        // 核心逻辑：每一帧都向指定方向移动
        transform.Translate(moveDirection * speed * Time.deltaTime);

        // 检查距离：如果飘得太远了，就销毁自己
        if (Vector3.Distance(_startPosition, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }
}
