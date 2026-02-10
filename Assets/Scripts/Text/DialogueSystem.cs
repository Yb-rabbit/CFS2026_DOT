using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI 引用")]
    public Text terminalText;        // 显示终端历史记录的 Text
    public Transform buttonContainer; // 存放按钮的容器
    public GameObject buttonPrefab;   // 按钮预制体
    public ScrollRect scrollRect;     // 滚动视图
    public Button clearButton;       // 清除按钮

    [Header("进度条设置")]
    public Scrollbar progressBar;    // 外置的进度条
    public GameObject endButton;     // 对话结束时显示的指定按钮
    
    [Header("对话数据")]
    public DialogueData currentData;
    public int startNodeId = 1;

    [Header("显示设置")]
    [Tooltip("开启后，新消息会先瞬间显示，然后再打字机输出一遍（复读机效果）")]
    public bool isRepeaterMode = true; // 默认开启复读模式

    // --- 复读机优化：历史记录管理 ---
    private List<string> dialogueHistory = new List<string>();
    private const int MaxHistoryCount = 2; // 限制最大文本量为2
    private int currentMaxNodeId = 0;      // 缓存最大节点ID用于计算进度
    private int currentActiveNodeId = 1;  // 【新增】记录当前正在显示的节点ID，用于重绘
    // -----------------------------

    private Coroutine typingCoroutine;
    private bool isTyping = false;

    // 定义一个事件，方便在Inspector中绑定
    public UnityEvent onDialogueCompleted;

    void Start()
    {
        if (currentData != null)
        {
            // 初始化计算最大ID
            CalculateMaxNodeId();
            StartDialogue(startNodeId);
        }
    }

    // 计算当前对话数据中最大的节点ID
    void CalculateMaxNodeId()
    {
        currentMaxNodeId = 1; // 默认至少为1
        
        if (currentData != null && currentData.dialogueList != null) 
        {
             foreach (var node in currentData.dialogueList)
             {
                 if (node.nodeId > currentMaxNodeId) currentMaxNodeId = node.nodeId;
             }
        }
        else
        {
            currentMaxNodeId = 1; 
        }
    }

    public void StartDialogue(int nodeId, string playerChoice = "")
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
        }

        DialogueNode node = currentData.GetNodeById(nodeId);
        if (node == null) return;

        // 【新增】记录当前节点ID，以便 ClearHistory 能够重新加载它
        currentActiveNodeId = nodeId;

        // --- 1. 更新进度条 ---
        UpdateProgressBar(nodeId);

        // --- 2. 检查是否到达末端 ---
        bool isEnd = (node.choices == null || node.choices.Count == 0);
        if (isEnd)
        {
            if (onDialogueCompleted != null) onDialogueCompleted.Invoke();
            if (endButton != null) endButton.SetActive(true);
        }
        else
        {
            if (endButton != null) endButton.SetActive(false);
        }

        // --- 3. 文本显示逻辑 (复读机版 + 限制2条) ---
        string currentLine = "";
        if (!string.IsNullOrEmpty(playerChoice))
        {
            currentLine += "> " + playerChoice + "\n";
        }
        currentLine += ">> " + node.terminalText;

        dialogueHistory.Add(currentLine);

        // 限制历史长度为2
        while (dialogueHistory.Count > MaxHistoryCount)
        {
            dialogueHistory.RemoveAt(0);
        }

        terminalText.text = "";

        if (isRepeaterMode)
        {
            // 复读机模式：瞬间显示所有历史（包括最新的）
            for (int i = 0; i < dialogueHistory.Count; i++)
            {
                terminalText.text += dialogueHistory[i] + "\n";
            }
            // 打字机再打一遍最新的
            typingCoroutine = StartCoroutine(AppendText(currentLine));
        }
        else
        {
            // 正常模式：只显示旧历史，最新的打字机输出
            for (int i = 0; i < dialogueHistory.Count - 1; i++)
            {
                terminalText.text += dialogueHistory[i] + "\n";
            }
            typingCoroutine = StartCoroutine(AppendText(currentLine));
        }
        
        // --- 4. 生成选项 (如果还有选项) ---
        CreateChoices(node.choices);
    }

    // 更新进度条方法
    void UpdateProgressBar(int currentNodeId)
    {
        if (progressBar != null && currentMaxNodeId > 0)
        {
            float progress = (float)currentNodeId / (float)currentMaxNodeId;
            progress = Mathf.Clamp01(progress);
            progressBar.size = progress; 
        }
    }

    IEnumerator AppendText(string text)
    {
        isTyping = true;
    
        foreach (char c in text)
        {
            terminalText.text += c;
            yield return new WaitForSeconds(0.05f); 
        }
    
        // 打字结束后，更新高度并滚动
        UpdateContentHeight();
    
        isTyping = false;
    }

    void UpdateContentHeight()
    {
        // 获取 Text 组件实际宽度，防止计算错误
        Vector2 textSize = terminalText.rectTransform.rect.size;
        TextGenerationSettings settings = terminalText.GetGenerationSettings(textSize); 
        settings.font = terminalText.font; 
    
        terminalText.cachedTextGenerator.Populate(terminalText.text, settings);
        float textHeight = terminalText.cachedTextGenerator.rectExtents.y * 2;
    
        RectTransform contentRect = scrollRect.content;
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, textHeight);
    
        scrollRect.verticalNormalizedPosition = 0f;
    }

    void CreateChoices(List<ChoiceOption> choices)
    {
        // 清空旧按钮
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // 如果没有选项，就不生成
        if (choices == null) return;

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
        // 1. 停止当前打字
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        isTyping = false;

        // 2. 清空屏幕文字和历史记录
        terminalText.text = "";
        dialogueHistory.Clear();

        // 3. 重置布局高度（防止残留）
        if (scrollRect != null)
        {
            RectTransform contentRect = scrollRect.content;
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, 0); 
        }

        // 4. 重新显示当前节点的对话
        // 传入空字符串作为 playerChoice，因为这不是玩家选择触发的，而是刷新触发的
        StartDialogue(currentActiveNodeId, ""); 
    }
    
    public void ToggleRepeaterMode(bool isOn)
    {
        isRepeaterMode = isOn;
    }
}
