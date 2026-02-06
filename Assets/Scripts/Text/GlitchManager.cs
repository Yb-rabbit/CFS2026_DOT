using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GlitchManager : MonoBehaviour
{
    [Header("=== 目标对象 ===")]
    [Tooltip("需要进行故障特效的文字组件")]
    public Text targetText; // 如果用 TextMeshPro，请改为 public TextMeshProUGUI targetText;

    [Tooltip("屏幕背景面板（用于颜色闪烁效果）")]
    public Image backgroundPanel;

    [Header("=== 故障参数 ===")]
    [Range(0f, 1f)]
    [Tooltip("故障发生的频率 (0.01 = 偶尔, 0.2 = 频繁)")]
    public float glitchChance = 0.05f;

    [Tooltip("发生乱码故障的几率 (应比抖动低)")]
    public float garbleChance = 0.005f;

    [Header("=== 颜色设置 ===")]
    public Color normalColor = Color.green; // 正常的终端绿
    public Color errorColor = Color.red;    // 报错红
    public Color ghostColor = Color.white;   // 鬼影白

    // 内部状态
    private bool _isGlitching = false; // 防止故障重叠
    private string _lastValidText = "";
    private Color _lastValidColor;

    void Start()
    {
        if (targetText != null)
        {
            _lastValidColor = targetText.color;
        }
        else
        {
            Debug.LogError("GlitchManager: 请将 Text 组件拖拽到 targetText 字段！");
        }
    }

    void Update()
    {
        if (targetText == null) return;

        // 1. 随机位置抖动 - 高频，小幅度
        if (Random.value < glitchChance)
        {
            ApplyJitter();
        }

        // 2. 随机乱码 - 低频，恐怖感强
        if (!_isGlitching && Random.value < garbleChance)
        {
            StartCoroutine(GarbleTextCoroutine());
        }

        // 3. 随机颜色故障 - 低频
        if (Random.value < garbleChance * 0.5f)
        {
            StartCoroutine(ColorGlitchCoroutine());
        }
    }

    /// <summary>
    /// 文字位置随机抖动
    /// </summary>
    private void ApplyJitter()
    {
        // 随机偏移 2 个像素左右
        float offsetX = Random.Range(-2f, 2f);
        float offsetY = Random.Range(-1f, 1f);
        
        // 仅偏移 Position，不改变 Scale
        targetText.rectTransform.anchoredPosition += new Vector2(offsetX, offsetY);
    }

    /// <summary>
    /// 文字变成乱码后恢复
    /// </summary>
    private IEnumerator GarbleTextCoroutine()
    {
        _isGlitching = true;
        _lastValidText = targetText.text;

        // 生成乱码字符串 (保持原长度)
        string garbled = GetRandomGarble(_lastValidText.Length);
        targetText.text = garbled;
        
        // 持续时间短，营造“闪现”的感觉
        yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));

        // 恢复原状
        targetText.text = _lastValidText;
        _isGlitching = false;
    }

    /// <summary>
    /// 颜色异常后恢复
    /// </summary>
    private IEnumerator ColorGlitchCoroutine()
    {
        // 随机选择一种故障颜色
        Color faultColor = (Random.value > 0.5f) ? errorColor : ghostColor;
        
        targetText.color = faultColor;

        // 故障持续时间
        yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));

        targetText.color = _lastValidColor;
    }

    /// <summary>
    /// 生成指定长度的随机乱码字符串
    /// </summary>
    private string GetRandomGarble(int length)
    {
        // 这里可以添加更多奇怪的字符，模拟编码错误
        string chars = "01234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ@#$%^&*鎴戜篃瑕要槸鐐瑰<>=/+?!";
        string result = "";
        
        for (int i = 0; i < length; i++)
        {
            result += chars[Random.Range(0, chars.Length)];
        }
        return result;
    }
}
