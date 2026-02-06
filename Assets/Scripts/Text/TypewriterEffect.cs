using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TypewriterEffect : MonoBehaviour
{
    [Header("=== 设置 ===")]
    [Tooltip("显示文字的组件（Text）")]
    public UnityEngine.UI.Text textComponent;

    [Tooltip("打字速度（秒/字）")]
    public float typingSpeed = 0.05f;

    [Header("=== 调试 ===")]
    [Tooltip("运行时手动修改此文本测试打字效果")]
    public string fullText = "系统初始化... D.O.T. 核心已加载。";

    // 内部变量
    private Coroutine _typingCoroutine;
    private bool _isTyping = false;
    private string _lastTypedText = ""; // 记录上次成功打字的文本

    // 打字完成事件，用于通知序列管理器播放下一条
    public Action OnComplete;

    private void Start()
    {
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
                // 如果点击时已经完成，不再强制重新播放
                // 如果你想在打完后点击强制重播，可以在这里取消注释：
                // StartTyping(fullText);
            }
        }
    }

    /// <summary>
    /// 开始打字（包含智能缓存逻辑）
    /// </summary>
    public void StartTyping(string text)
    {
        // 如果当前显示的就是这段文字，直接返回，不重新打字
        if (text == _lastTypedText && textComponent.text == text)
        {
            return;
        }

        // 清空当前文字
        textComponent.text = "";
        _lastTypedText = text; // 更新缓存

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
        
        // 直接显示全部文字
        textComponent.text = _lastTypedText; 
        
        // 如果是点击跳过，也触发完成事件
        _isTyping = false;
        OnComplete?.Invoke();
    }

    /// <summary>
    /// 重置状态（用于强制清空或重新开始）
    /// </summary>
    public void Reset()
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }
        textComponent.text = "";
        _lastTypedText = "";
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
        OnComplete?.Invoke(); // 打字自然结束，通知外界
    }
}
