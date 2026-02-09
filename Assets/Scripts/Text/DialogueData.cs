using System.Collections.Generic;
using UnityEngine;

// 1. 选项的结构（玩家可以选择的回答）
[System.Serializable]
public class ChoiceOption
{
    public string optionText; // 选项文字，如："知道" / "不知道"
    public int nextNodeId;    // 选择这个选项后，跳转到哪个对话节点
}

// 2. 单个对话节点的结构（终端的一句台词）
[System.Serializable]
public class DialogueNode
{
    public int nodeId;               // 节点的唯一ID
    [TextArea(3, 5)] 
    public string terminalText;      // 终端说的话（NPC文本）
    public List<ChoiceOption> choices; // 玩家可选的选项列表
}

// 3. 整个对话数据的容器
[CreateAssetMenu(fileName = "NewDialogue", menuName = "D.O.T/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public List<DialogueNode> dialogueList = new List<DialogueNode>();

    // 辅助方法：根据ID获取节点
    public DialogueNode GetNodeById(int id)
    {
        return dialogueList.Find(node => node.nodeId == id);
    }
}
