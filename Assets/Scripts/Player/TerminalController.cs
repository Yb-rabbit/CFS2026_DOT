using UnityEngine;

public class TerminalController : MonoBehaviour, IInteractable
{
    [Header("=== 组件引用 ===")]
    public GameObject terminalCanvas;
    public TextSequencer textSequencer;

    [Header("=== 外部依赖 ===")]
    public PowerSwitch powerController; // 拖拽挂载了PowerSwitch的物体

    private bool _isUIActive = false;

    void Start()
    {
        if (terminalCanvas != null) terminalCanvas.SetActive(false);
    }

    void Update()
    {
        // 如果终端是开着的，但是突然断电了，强制关闭终端
        if (_isUIActive && powerController != null && !powerController.IsPowerOn)
        {
            CloseTerminal();
        }
    }

    public void Interact()
    {
        // 如果存在电源控制器，且电源是关的 -> 阻止操作
        if (powerController != null && !powerController.IsPowerOn)
        {
            Debug.Log("没电了，无法打开终端！");
            return; 
        }

        // 只有通电了才能切换开关状态
        _isUIActive = !_isUIActive;

        if (_isUIActive)
        {
            OpenTerminal();
        }
        else
        {
            CloseTerminal();
        }
    }

    private void OpenTerminal()
    {
        if (terminalCanvas != null) terminalCanvas.SetActive(true);
        if (textSequencer != null) textSequencer.StartSequence();
    }

    private void CloseTerminal()
    {
        if (terminalCanvas != null) terminalCanvas.SetActive(false);
    }

    public string GetInteractionText()
    {
        if (powerController != null && !powerController.IsPowerOn)
        {
            return "警告：终端离线";
        }

        return "点击：访问终端 [左键]";
    }
}
