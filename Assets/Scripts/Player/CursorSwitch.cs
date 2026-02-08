using UnityEngine;

public class CursorSwitch : MonoBehaviour, IInteractable
{
    [Header("=== 鼠标初始设置 ===")]
    [Tooltip("游戏开始时光标是否可见")]
    public bool startVisible = false;

    [Header("=== 3D 光标控制（可选） ===")]
    [Tooltip("是否控制场景中的 3D 光标物体（如那个黑点）")]
    public bool isWorldSpaceCursor = false;

    [Tooltip("把场景中的 3D 光标物体拖到这里")]
    public GameObject worldCursorObject;

    // 内部状态
    private bool _isVisible = true;

    void Start()
    {
        _isVisible = startVisible;
        UpdateCursorState();
    }

    // === 核心功能：更新光标和锁定状态 ===
    private void UpdateCursorState()
    {
        if (_isVisible)
        {
            // === 模式 A：UI 模式（菜单/终端模式）===
            // 1. 显示系统光标
            Cursor.visible = true;
            
            // 2. 【关键】解锁光标，允许鼠标在屏幕上自由移动
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // === 模式 B：FPS 游戏模式 ===
            // 1. 隐藏系统光标
            Cursor.visible = false;
            
            // 2. 【关键】锁定光标到屏幕中心，启用第一人称视角控制
            Cursor.lockState = CursorLockMode.Locked;
        }

        // 3. 如果勾选了，同步控制 3D 光标物体的显示
        if (isWorldSpaceCursor && worldCursorObject != null)
        {
            worldCursorObject.SetActive(_isVisible);
        }
    }

    // === 供外部脚本调用的属性 ===
    public bool IsCursorVisible
    {
        get { return _isVisible; }
        set
        {
            if (_isVisible != value)
            {
                _isVisible = value;
                UpdateCursorState();
            }
        }
    }

    // === 交互接口 ===
    public void Interact()
    {
        // 点击这个物体时，切换光标状态
        _isVisible = !_isVisible;
        UpdateCursorState();
    }

    // === 辅助方法 ===
    public void ToggleCursor()
    {
        _isVisible = !_isVisible;
        UpdateCursorState();
    }

    // === 提示文本 ===
    public string GetInteractionText()
    {
        return _isVisible ? "点击：隐藏鼠标 [左键]" : "点击：显示鼠标 [左键]";
    }
}
