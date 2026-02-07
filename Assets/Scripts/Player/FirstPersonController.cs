using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("=== 移动设置 ===")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("=== 组件引用 ===")]
    public CharacterController controller; // 必须手动拖拽
    public Transform cameraTransform;      // 必须手动拖拽 Main Camera (或者 Cinemachine下的 Camera)

    [Header("=== 身体旋转 (可选) ===")]
    [Tooltip("勾选后，人物模型也会转向移动的方向")]
    public bool rotateBodyToFaceCamera = false;

    private Vector3 _moveDirection;
    private float _verticalSpeed;

    void Start()
    {
        // 自动查找组件（防止手动拖拽遗漏）
        if (controller == null) controller = GetComponent<CharacterController>();
        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        // 鼠标锁定
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. 获取输入
        float horizontal = Input.GetAxis("Horizontal"); // A / D
        float vertical = Input.GetAxis("Vertical");       // W / S

        // 2. 【核心修复】基于摄像机朝向计算移动向量
        // 获取摄像机的“前”和“右”方向
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        // 【关键】将 Y 轴设为 0，确保玩家永远只在水平面上移动
        // 如果不做这一步，当你抬头看天按 W，你会飞到天上去；低头看地按 W，你会钻到地底
        camForward.y = 0;
        camRight.y = 0;

        // 归一化向量，防止斜着走速度变快（勾股定理）
        camForward.Normalize();
        camRight.Normalize();

        // 组合方向：(前*W) + (右*A/D)
        Vector3 move = (camForward * vertical) + (camRight * horizontal);

        // 3. 处理奔跑
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // 4. 移动处理 (包含重力)
        // 如果在地面
        if (controller.isGrounded)
        {
            _verticalSpeed = -2f; // 轻微向下压，确保贴地
        }
        else
        {
            // 空中时应用重力
            _verticalSpeed += Physics.gravity.y * Time.deltaTime;
        }

        // 组合最终的移动向量 (水平移动 + 垂直重力)
        Vector3 finalMove = (move * currentSpeed) + (Vector3.up * _verticalSpeed);

        // 应用移动
        controller.Move(finalMove * Time.deltaTime);

        // 5. 【可选】让身体模型旋转面向摄像机方向
        // 如果你想让人物脚下的身体也跟着转头，取消下面注释并勾选 rotateBodyToFaceCamera
        if (rotateBodyToFaceCamera && move != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(move);
        }

        // 6. ESC 释放鼠标
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
