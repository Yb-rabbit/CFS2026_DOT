using UnityEngine;

// 这是一个接口，任何想要交互的物体都必须继承它
public interface IInteractable
{
    // 当玩家交互时调用此方法
    void Interact();
    
    // 返回交互的提示文字（比如“按 E 关闭电源”）
    string GetInteractionText(); 
}
