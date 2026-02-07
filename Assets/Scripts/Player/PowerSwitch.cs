using UnityEngine;
using UnityEngine.UI;

public class PowerSwitch : MonoBehaviour, IInteractable
{
    [Header("=== 引用 ===")]
    public Light roomLight; // 拖拽场景里的灯光
    public GameObject terminalUI; // 拖拽终端 UI 的 Canvas
    public Text statusText; // 开关上的文字（可选）

    private bool _isOn = true;

    // 实现接口方法：定义交互逻辑
    public void Interact()
    {
        _isOn = !_isOn; // 切换开关状态
        
        Debug.Log("开关状态已切换: " + (_isOn ? "开启" : "关闭"));

        if (_isOn)
        {
            TurnOn();
        }
        else
        {
            TurnOff();
        }
    }

    // 实现接口方法：定义提示文字
    public string GetInteractionText()
    {
        return _isOn ? "点击：关闭电源 [左键]" : "点击：重启电源 [左键]";
    }

    private void TurnOn()
    {
        // 开灯
        if (roomLight != null) roomLight.enabled = true;
        
        // 显示终端
        if (terminalUI != null) terminalUI.SetActive(true);
        
        if (statusText != null) statusText.text = "ONLINE";
        statusText.color = Color.green;
    }

    private void TurnOff()
    {
        // 关灯
        if (roomLight != null) roomLight.enabled = false;
        
        // 隐藏终端
        if (terminalUI != null) terminalUI.SetActive(false);
        
        if (statusText != null) statusText.text = "OFFLINE";
        statusText.color = Color.red;
    }
}
