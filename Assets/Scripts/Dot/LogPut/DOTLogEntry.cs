// DOTLogEntry.cs
using UnityEngine;

[CreateAssetMenu(fileName = "DOTLogEntry", menuName = "D.O.T/Log Entry")]
public class DOTLogEntry : ScriptableObject
{
    [Header("基础信息")]
    public string logID;           // 格式: LOG_001
    public int displayOrder;       // 显示顺序 (1, 2, 3...)
    public string timestamp;       // 完整时间戳
    public string operatorID;      // 操作员ID
    public string logType;         // 日志类型
    
    [Header("内容")]
    [TextArea(2, 4)]
    public string eventDescription; // 精简后的事件描述
    
    [TextArea(1, 3)]
    public string dataSummary;      // 关键数据
    
    [Header("标记")]
    public bool isCriticalLog;     // 是否关键日志
    public bool showInPanel = true; // 是否在面板中显示
}