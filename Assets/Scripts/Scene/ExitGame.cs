using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExitGame : MonoBehaviour
{
    public Image fadeImage; // 拖入全屏黑色图像
    public float fadeDuration = 2.0f; // 渐变持续时间
    public float delayBeforeExit = 0.5f; // 完全变黑后延迟退出时间

    // 按钮事件处理方法
    public void OnExitButtonPressed()
    {
        StartCoroutine(FadeAndExit());
    }

    // 渐变并退出游戏的协程
    private IEnumerator FadeAndExit()
    {
        // 确保黑色图像启用
        fadeImage.gameObject.SetActive(true);

        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // 等待一段时间以确保图像完全变黑
        yield return new WaitForSeconds(delayBeforeExit);

        // 退出游戏
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
