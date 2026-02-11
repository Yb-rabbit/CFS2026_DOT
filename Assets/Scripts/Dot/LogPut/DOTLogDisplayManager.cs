// DOTLogDisplayManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DOTLogDisplayManager : MonoBehaviour
{
    [Header("日志数据")]
    public List<DOTLogEntry> allLogs = new List<DOTLogEntry>();
    
    [Header("显示面板")]
    public Text[] displayPanels;   // 4个显示面板，分别对应不同阶段
    
    [Header("面板分配")]
    public int[] panel1Logs = {1, 2};     // 面板1
    public int[] panel2Logs = {3, 4};     // 面板2
    public int[] panel3Logs = {5, 6};     // 面板3
    public int[] panel4Logs = {7, 8};     // 面板4
    
    void Start()
    {
        // 排序日志
        allLogs.Sort((a, b) => a.displayOrder.CompareTo(b.displayOrder));
        
        // 初始化所有面板
        InitializeAllPanels();
    }
    
    // 初始化所有面板
    void InitializeAllPanels()
    {
        if (displayPanels == null || displayPanels.Length < 4)
        {
            Debug.LogError("需要4个显示面板！");
            return;
        }
        
        // 为每个面板设置内容
        SetPanelContent(displayPanels[0], panel1Logs, "隐患与删除阶段");
        SetPanelContent(displayPanels[1], panel2Logs, "仲裁与真相阶段");
        SetPanelContent(displayPanels[2], panel3Logs, "清除与遗留阶段");
        SetPanelContent(displayPanels[3], panel4Logs, "继承与感染阶段");
    }
    
    // 设置面板内容
    void SetPanelContent(Text panel, int[] logOrders, string stageName = "")
    {
        if (panel == null) return;
        
        string content = "";
        
        foreach (int order in logOrders)
        {
            DOTLogEntry log = allLogs.Find(l => l.displayOrder == order);
            if (log != null && log.showInPanel)
            {
                content += FormatLogForDisplay(log) + "\n\n";
            }
        }
        
        panel.text = content;
    }
    
    string FormatLogForDisplay(DOTLogEntry log)
    {
    string formatted = "";
    
    // 基础信息
    formatted += $"<b>{log.displayOrder:00}</b> ";
    
    // 提取时间部分（只显示时分秒）
    if (log.timestamp.Length > 11)
    {
        formatted += $"<color=#88CCFF>{log.timestamp.Substring(11, 8)}</color> ";
    }
    
    // 操作员
    formatted += $"<color=#FFAA66>{log.operatorID}</color>\n";
    
    // 事件描述
    formatted += $"   {log.eventDescription}";
    
    // 关键日志标记放在事件描述后面
    if (log.isCriticalLog)
    {
        formatted += " <color=#FF5555>△</color>";
    }
    
    formatted += "\n";
    
    // 数据摘要
    if (!string.IsNullOrEmpty(log.dataSummary))
    {
        formatted += $"   <size=35%><color=#AAAAAA>{log.dataSummary}</color></size>";
    }
    
    return formatted;
    }
    
    // 切换日志显示/隐藏
    public void ToggleLogVisibility(int logOrder, bool show)
    {
        DOTLogEntry log = allLogs.Find(l => l.displayOrder == logOrder);
        if (log != null)
        {
            log.showInPanel = show;
            InitializeAllPanels(); // 重新刷新所有面板
        }
    }
    
    // 获取单条日志的完整信息（用于详情弹窗）
    public string GetLogFullInfo(int logOrder)
    {
        DOTLogEntry log = allLogs.Find(l => l.displayOrder == logOrder);
        if (log == null) return "";
        
        return $"【{log.logID}】\n" +
               $"时间: {log.timestamp}\n" +
               $"操作员: {log.operatorID}\n" +
               $"类型: {log.logType}\n\n" +
               $"事件: {log.eventDescription}\n\n" +
               $"数据: {log.dataSummary}\n" +
               $"{(log.isCriticalLog ? "⚠ 关键记录" : "")}";
    }
}