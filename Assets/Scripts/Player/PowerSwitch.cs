using UnityEngine;
using UnityEngine.UI;

public class PowerSwitch : MonoBehaviour, IInteractable
{
    [Header("=== 引用 ===")]
    public Light roomLight;
    public Text statusText;

    public bool IsPowerOn { get { return _isOn; } }

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
        
        // 更新文字状态 (增加空值判断，防止报错)
        if (statusText != null)
        {
            statusText.text = "ONLINE";
            statusText.color = Color.green;
        }
    }

    private void TurnOff()
    {
        // 关灯
        if (roomLight != null) roomLight.enabled = false;
        
        // 更新文字状态 (增加空值判断，防止报错)
        if (statusText != null)
        {
            statusText.text = "OFFLINE";
            statusText.color = Color.red;
        }
        
        // 注意：这里不再手动控制 terminalUI，完全交给 TerminalController 处理
    }
}
