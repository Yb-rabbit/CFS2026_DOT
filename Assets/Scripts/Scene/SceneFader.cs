using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    // 单例实例，确保全局唯一
    public static SceneFader instance;

    // 全屏遮罩（用于淡入淡出的Image）
    [Header("核心组件")]
    public Image fadeImage;

    // 淡入/淡出时长（默认1秒，适配游戏节奏）
    [Header("过渡参数")]
    [Tooltip("淡入/淡出的总时长（秒）")]
    public float fadeDuration = 1f;
    [Tooltip("遮罩默认颜色（推荐黑色，匹配游戏风格）")]
    public Color fadeColor = Color.black;

    // 初始状态：遮罩透明
    private void Awake()
    {
        // 单例初始化（避免重复创建）
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); // 跨场景保留
        }
        else
        {
            Destroy(gameObject);
        }

        // 初始化遮罩
        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f); // 初始透明
            fadeImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height); // 全屏适配
        }
        else
        {
            Debug.LogError("请给SceneFader绑定全屏遮罩Image！");
        }
    }

    /// <summary>
    /// 静态调用：切换到指定场景（先淡出，加载场景，再淡入）
    /// </summary>
    /// <param name="sceneName">目标场景名称</param>
    public static void FadeToScene(string sceneName)
    {
        if (instance != null)
        {
            instance.StartCoroutine(instance.FadeCoroutine(sceneName));
        }
        else
        {
            Debug.LogError("SceneFader实例不存在！请确保脚本已挂载并激活");
            // 无淡入效果时直接切换场景（兜底）
            SceneManager.LoadScene(sceneName);
        }
    }

    /// <summary>
    /// 仅淡入当前场景（如游戏启动/结局触发后）
    /// </summary>
    public static void OnlyFadeIn()
    {
        if (instance != null)
        {
            instance.StartCoroutine(instance.FadeInCoroutine());
        }
    }

    /// <summary>
    /// 核心协程：淡出→加载场景→淡入
    /// </summary>
    private IEnumerator FadeCoroutine(string sceneName)
    {
        // 第一步：淡出（遮罩从透明变不透明）
        yield return FadeOutCoroutine();

        // 第二步：加载目标场景
        yield return SceneManager.LoadSceneAsync(sceneName);

        // 第三步：淡入（遮罩从不透明变透明）
        yield return FadeInCoroutine();
    }

    /// <summary>
    /// 淡出协程（遮罩显示）
    /// </summary>
    private IEnumerator FadeOutCoroutine()
    {
        float elapsedTime = 0f;
        Color targetColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1f); // 不透明

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }

        fadeImage.color = targetColor; // 确保最终状态正确
    }

    /// <summary>
    /// 淡入协程（遮罩消失）
    /// </summary>
    private IEnumerator FadeInCoroutine()
    {
        float elapsedTime = 0f;
        Color startColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1f); // 不透明

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f); // 最终透明
    }
}