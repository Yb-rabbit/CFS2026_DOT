using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyLoadScene : MonoBehaviour
{
    public string targetSceneName;// 目标场景名称

    void Update()
    {
        // 检测是否按下任意键
        if (Input.anyKeyDown)
        {
            // 加载目标场景
            SceneManager.LoadScene(targetSceneName);
        }
    }
}

