using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.UI;

public class TypewriterEffect : MonoBehaviour
{
    [Header("=== 设置 ===")]
    [Tooltip("显示文字的组件（支持 Text 或 TextMeshProUGUI）")]
    public UnityEngine.UI.Text textComponent; // 拖拽你的 Text 组件到这里
    // 如果你用的是 TextMeshPro，改成 public TMPro.TextMeshProUGUI textComponent;

    [Tooltip("打字速度（秒/字）")]
    public float typingSpeed = 0.05f;

    [Header("=== 测试内容 ===")]
    [Tooltip("需要显示的完整文本")]
    public string fullText = "系统初始化... D.O.T. 核心已加载。";

    // 内部变量
    private Coroutine _typingCoroutine;
    private bool _isTyping = false;

    private void Start()
    {
        // 开始时自动播放
        if (textComponent != null)
        {
            StartTyping(fullText);
        }
        else
        {
            Debug.LogError("请将 Text 组件拖拽到 textComponent 字段上！");
        }
    }

    private void Update()
    {
        // 交互逻辑：点击鼠标左键
        if (Input.GetMouseButtonDown(0)) 
        {
            if (_isTyping)
            {
                // 正在打字 -> 点击则“瞬间完成”
                SkipTyping();
            }
            else
            {
                // 打字已完成 -> 点击则“重新开始”或“播放下一条”
                // 这里为了演示简单，我们重新播放当前的文本
                StartTyping(fullText);
            }
        }
    }

    /// <summary>
    /// 开始打字
    /// </summary>
    public void StartTyping(string text)
    {
        // 清空当前文字
        textComponent.text = "";
        
        // 如果正在打字，停止之前的协程
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }

        // 开始新的打字协程
        _typingCoroutine = StartCoroutine(TypeTextCoroutine(text));
    }

    /// <summary>
    /// 瞬间完成打字
    /// </summary>
    public void SkipTyping()
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }
        textComponent.text = fullText; // 直接显示全部文字
        _isTyping = false;
    }

    // 打字协程逻辑
    private IEnumerator TypeTextCoroutine(string textToType)
    {
        _isTyping = true;
        fullText = textToType; // 更新缓存文本

        foreach (char c in textToType)
        {
            // 每次循环加一个字
            textComponent.text += c;
            
            // 等待指定时间
            yield return new WaitForSeconds(typingSpeed);
        }

        _isTyping = false;
    }
}
