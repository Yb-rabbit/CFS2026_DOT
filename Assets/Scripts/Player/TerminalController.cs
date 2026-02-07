using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TerminalController : MonoBehaviour, IInteractable
{
    [Header("=== 组件引用 ===")]
    [Tooltip("拖拽终端的 Canvas (UI) 到这里")]
    public GameObject terminalCanvas;

    [Tooltip("拖拽 TextSequencer 组件到这里")]
    public TextSequencer textSequencer;

    [Header("=== 交互设置 ===")]
    [Tooltip("是否在打开终端时隐藏鼠标指针")]
    public bool hideCursorWhenOpen = false;

    [Tooltip("是否在打开终端时锁定玩家移动")]
    public bool lockPlayerMovementWhenOpen = true;

    private bool _isUIActive = false;
    private FirstPersonController _playerController; // 引用下面的玩家脚本

    void Start()
    {
        // 初始隐藏 UI
        if (terminalCanvas != null) terminalCanvas.SetActive(false);
        
        // 获取玩家脚本引用
        _playerController = FindObjectOfType<FirstPersonController>();
    }

    // 实现接口：交互逻辑
    public void Interact()
    {
        _isUIActive = !_isUIActive; // 切换状态

        if (_isUIActive)
        {
            // --- 打开终端 ---
            if (terminalCanvas != null) terminalCanvas.SetActive(true);
            
            // 播放文本序列
            if (textSequencer != null)
            {
                textSequencer.StartSequence();
            }

            // 锁定状态
            if (hideCursorWhenOpen)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None; // 允许点击 UI
            }
            if (lockPlayerMovementWhenOpen && _playerController != null)
            {
                _playerController.enabled = false; // 禁用玩家移动
            }
        }
        else
        {
            // --- 关闭终端 ---
            if (terminalCanvas != null) terminalCanvas.SetActive(false);

            // 恢复状态
            if (hideCursorWhenOpen)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked; // 锁回准星
            }
            if (lockPlayerMovementWhenOpen && _playerController != null)
            {
                _playerController.enabled = true; // 恢复玩家移动
            }
        }
    }

    // 实现接口：提示文字
    public string GetInteractionText()
    {
        return _isUIActive ? "点击：关闭终端 [Esc]" : "点击：访问终端 [E]";
    }
}
