// DOTLogDisplayManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DOTLogDisplayManager : MonoBehaviour
{
    [Header("日志数据源")]
    public List<DOTLogEntry> allLogs = new List<DOTLogEntry>();
    
    [Header("UI组件绑定")]
    public Text logDisplayText;      // 显示日志的Text组件
    public ScrollRect scrollRect;    // 滚动区域（可选）
    
    [Header("显示设置")]
    public bool autoScrollToBottom = true;  // 新日志自动滚动到底部
    public bool showTimeline = true;        // 显示时间线标记
    public string logSeparator = "\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";
    
    void Start()
    {
        // 按displayOrder排序日志
        allLogs.Sort((a, b) => a.displayOrder.CompareTo(b.displayOrder));
        
        // 初始显示所有日志
        DisplayAllLogs();
    }
    
    // 核心方法：显示所有日志
    public void DisplayAllLogs()
    {
        string displayText = "【D.O.T. 系统日志 - 完整记录】\n";
        displayText += "═══════════════════════════════════════\n\n";
        
        foreach (var log in allLogs)
        {
            displayText += FormatSingleLog(log) + logSeparator;
        }
        
        logDisplayText.text = displayText;
        
        // 滚动到底部
        if (autoScrollToBottom && scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
    
    // 格式化单条日志
    string FormatSingleLog(DOTLogEntry log)
{
    string prefix = log.isCriticalLog ? "⚠ " : "▶ ";
    return $"{prefix}{log.displayOrder:000} {log.timestamp.Substring(11)} | " +
           $"{log.operatorID} | {GetEventSummary(log.eventDescription)}";
}

string GetEventSummary(string fullEvent)
{
    // 提取最关键的动作描述，这里简化处理
    if (fullEvent.Contains("清空")) return "申诉草稿创建并清空";
    if (fullEvent.Contains("签发")) return "签发解约文件";
    if (fullEvent.Contains("删除")) return "永久删除关键日志";
    // ... 其他情况的处理
    return fullEvent.Length > 20 ? fullEvent.Substring(0, 20) + "..." : fullEvent;
}
    
    // 按序号获取并显示特定日志（可用于逐条播放）
    public void DisplayLogByOrder(int order)
    {
        DOTLogEntry targetLog = allLogs.Find(log => log.displayOrder == order);
        if (targetLog != null)
        {
            logDisplayText.text = FormatSingleLog(targetLog);
        }
    }
    
    // 清空显示
    public void ClearDisplay()
    {
        logDisplayText.text = "";
    }
}