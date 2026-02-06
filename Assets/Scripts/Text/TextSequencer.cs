using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class TextSequencer : MonoBehaviour
{
    [Header("=== 引用 ===")]
    [Tooltip("拖拽包含 TypewriterEffect 脚本的游戏物体到这里")]
    public TypewriterEffect typewriter; // 关联上面的脚本

    [Header("=== 序列设置 ===")]
    [Tooltip("需要依次播放的所有文本")]
    public List<string> textSequence = new List<string> { 
        "系统启动...",
        "正在载入 D.O.T. 核心模块...",
        "检测到未知数据流...",
        "警告：精神稳定度下降至 50%..."
    };

    [Tooltip("每段文字之间的间隔时间（秒）")]
    public float delayBetweenLines = 3f;

    [Header("=== 运行时控制 ===")]
    [Tooltip("是否自动开始播放序列")]
    public bool autoStart = true;

    [Tooltip("是否循环播放序列")]
    public bool loopSequence = false;

    // 内部变量
    private int _currentIndex = 0;
    private bool _isPlaying = false;
    private Coroutine _delayCoroutine;

    void Start()
    {
        if (typewriter == null)
        {
            Debug.LogError("TextSequencer: 请在 Inspector 中指定 TypewriterEffect 组件！");
            return;
        }

        // 订阅打字机的完成事件
        typewriter.OnComplete += OnLineCompleted;

        if (autoStart)
        {
            StartSequence();
        }
    }

    private void OnDestroy()
    {
        // 防止内存泄漏，取消订阅
        if (typewriter != null)
        {
            typewriter.OnComplete -= OnLineCompleted;
        }
    }

    /// <summary>
    /// 开始播放序列
    /// </summary>
    public void StartSequence()
    {
        if (textSequence.Count == 0) return;

        _currentIndex = 0;
        _isPlaying = true;
        
        // 播放第一条
        PlayCurrentLine();
    }

    /// <summary>
    /// 播放当前行的文本
    /// </summary>
    private void PlayCurrentLine()
    {
        if (_currentIndex < textSequence.Count)
        {
            string currentText = textSequence[_currentIndex];
            typewriter.StartTyping(currentText);
        }
        else
        {
            // 序列播放完毕
            if (loopSequence)
            {
                _currentIndex = 0;
                PlayCurrentLine();
            }
            else
            {
                _isPlaying = false;
                Debug.Log("文本序列播放完毕。");
            }
        }
    }

    /// <summary>
    /// 当一行文字播放完毕时调用
    /// </summary>
    private void OnLineCompleted()
    {
        if (!_isPlaying) return;

        // 延时一定时间后播放下一条
        if (_delayCoroutine != null) StopCoroutine(_delayCoroutine);
        _delayCoroutine = StartCoroutine(DelayAndNext());
    }

    private IEnumerator DelayAndNext()
    {
        yield return new WaitForSeconds(delayBetweenLines);
        _currentIndex++;
        PlayCurrentLine();
    }

    /// <summary>
    /// 外部调用：强制跳过当前文字并播放下一条
    /// </summary>
    public void ForceNextLine()
    {
        // 立即完成当前打字（如果有）
        typewriter.SkipTyping();
        
        // 停止当前的延时等待，直接跳到下一条
        if (_delayCoroutine != null)
        {
            StopCoroutine(_delayCoroutine);
            _delayCoroutine = null;
        }
        
        _currentIndex++;
        PlayCurrentLine();
    }
}
