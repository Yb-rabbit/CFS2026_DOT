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

    private Coroutine typingCoroutine;
    private bool isTyping = false; // 标记是否正在打字中

    void Start()
    {
        if (currentData != null)
        {
            StartDialogue(startNodeId);
        }
    }

    // 开始新的一轮对话（追加模式）
    public void StartDialogue(int nodeId, string playerChoice = "")
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
        }

        DialogueNode node = currentData.GetNodeById(nodeId);
        if (node == null) return;

        string newContent = "";

        if (!string.IsNullOrEmpty(playerChoice))
        {
            newContent += "\n> " + playerChoice + "\n";
        }

        // 显示终端的回复
        newContent += ">> " + node.terminalText;
        
        typingCoroutine = StartCoroutine(AppendText(newContent));
        CreateChoices(node.choices);
    }


        IEnumerator AppendText(string text)
    {
        isTyping = true;
        
        // 每次开始打字前，先强制滚动一次到底部，防止之前的残留
        if (scrollRect != null) scrollRect.verticalNormalizedPosition = 0f;

        foreach (char c in text)
        {
            terminalText.text += c;
            
            yield return new WaitForSeconds(0.07f); // 正常打字间隔
            
            // 每隔几个字，或者每打完一个字，都尝试更新
            if (scrollRect != null)
            {
                // 1. 强制重建布局（告诉Unity别偷懒，马上重新算高度）
                LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
                
                // 2. 设置为0（最底部）
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }
        
        // 打字全部结束后，再等待一帧，最后确保滚到底部
        yield return new WaitForEndOfFrame();
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }

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
        // 1. 停止当前正在进行的打字效果（防止清空后字又蹦出来）
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        // 2. 清空文本框的内容
        terminalText.text = "";

        // 3. (可选) 如果你想清空后自动重新开始第一句对话，取消下面这行的注释：
        // StartDialogue(startNodeId);
    }
}
