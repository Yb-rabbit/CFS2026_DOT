using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI 引用")]
    public Text terminalText;        // 显示终端历史记录的 Text
    public Transform buttonContainer; // 存放按钮的容器
    public GameObject buttonPrefab;   // 按钮预制体
    public ScrollRect scrollRect;     // 滚动视图，用于自动滚动到底部
    public Button clearButton;       // 清除按钮（可选）

    [Header("对话数据")]
    public DialogueData currentData;
    public int startNodeId = 1;

    // --- 优化：只保留最近的5条对话 ---
    private List<string> dialogueHistory = new List<string>(); // 存储每一轮对话的完整文本
    private const int MaxHistoryCount = 2; // 最多保留2条历史记录
    // -----------------------------

    private Coroutine typingCoroutine;
    private bool isTyping = false; // 标记是否正在打字中

    void Start()
    {
        // 1. 强制清空历史记录（避免初始残留）
        dialogueHistory.Clear();

        if (currentData != null)
        {
            StartDialogue(startNodeId);
        }
    }

    // 开始新的一轮对话（追加模式）
    public void StartDialogue(int nodeId, string playerChoice = "")
    {
        // 2. 停止之前的打字机协程（避免冲突）
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
        }

        DialogueNode node = currentData.GetNodeById(nodeId);
        if (node == null) return;

        // 3. 构建当前这轮对话的完整文本（包含玩家选择和NPC回复）
        string currentLine = "";
        if (!string.IsNullOrEmpty(playerChoice))
        {
            currentLine += "> " + playerChoice + "\n";
        }
        currentLine += ">> " + node.terminalText;

        // 4. 先删除旧记录（如果超过最大数量），再添加新记录
        while (dialogueHistory.Count >= MaxHistoryCount)
        {
            dialogueHistory.RemoveAt(0); // 删除最早的对话（索引0）
        }

        // 5. 添加到历史记录（最新消息在末尾）
        dialogueHistory.Add(currentLine);

        // 6. 重新构建Text内容（按顺序拼接历史记录）
        terminalText.text = "";
        for (int i = 0; i < dialogueHistory.Count; i++)
        {
            terminalText.text += dialogueHistory[i] + "\n";
        }

        // 7. 对最新的这一条进行打字机效果（仅追加当前新消息）
        typingCoroutine = StartCoroutine(AppendText(currentLine));
        
        // 8. 生成选项
        CreateChoices(node.choices);
    }

    IEnumerator AppendText(string text)
    {
        isTyping = true;
    
        foreach (char c in text)
        {
            terminalText.text += c; // 仅追加当前新消息，不干扰历史部分
            yield return new WaitForSeconds(0.05f); // 打字延迟
        }
    
        // 打字结束后，滚动到底部（完全贴底）
        scrollRect.verticalNormalizedPosition = 0f;
    
        isTyping = false;
    }

    void CreateChoices(List<ChoiceOption> choices)
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var choice in choices)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            Button btn = btnObj.GetComponent<Button>();
            Text btnText = btn.GetComponentInChildren<Text>(); 
            if (btnText) btnText.text = "▶ " + choice.optionText;

            int targetId = choice.nextNodeId;
            
            btn.onClick.AddListener(() => StartDialogue(targetId, choice.optionText));
        }
    }

    public void ClearHistory()
    {
        // 1. 停止当前正在进行的打字效果
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        isTyping = false;

        // 2. 清空文本框的内容
        terminalText.text = "";

        // 3. 强制清空历史记录列表
        dialogueHistory.Clear();

        // 4. 重置滚动条到顶部（可选，根据需求调整）
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
