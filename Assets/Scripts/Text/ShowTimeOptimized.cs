using UnityEngine;
using System;
using UnityEngine.UI;

public class ShowTimeOptimized : MonoBehaviour
{
    public Text timeText;
    
    private string lastTimeStr = "";

    void Update()
    {
        // 获取当前时间
        string currentTimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // 只有当字符串发生变化时（即时间跳变时）才修改 UI 文本
        if (currentTimeStr != lastTimeStr)
        {
            timeText.text = currentTimeStr;
            lastTimeStr = currentTimeStr;
        }
    }
}
