using UnityEngine;

public class SaveEnd : MonoBehaviour
{
    public static void ScarePlayer()
    {
        // 你想打开的任意整蛊页面
        string scareURL = "https://www.bilibili.com/video/BV1GJ411x7h7";
        Application.OpenURL(scareURL);
        
        Debug.Log("👻 玩家已被吓一跳，页面已打开：" + scareURL);
    }
}