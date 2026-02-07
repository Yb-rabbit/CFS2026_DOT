using UnityEngine;
using System.Collections;

public class NoteSpawner : MonoBehaviour
{
    [Header("=== 生成设置 ===")]
    [Tooltip("拖拽做好的 Note 预制体")]
    public GameObject notePrefab;

    [Tooltip("生成速度（秒/个）。越小越快")]
    public float spawnInterval = 0.2f;

    [Tooltip("生成的范围大小")]
    public Vector3 spawnAreaSize = new Vector3(10f, 5f, 0f);

    [Header("=== 长短随机设置 ===")]
    [Tooltip("最小长度")]
    public float minLength = 1f;
    [Tooltip("最大长度")]
    public float maxLength = 5f;

    [Header("=== 颜色随机设置 ===")]
    [Tooltip("最小颜色值 (RGB, 0-1)")]
    public Color minColor = new Color(0f, 0.5f, 0f); // 默认偏绿

    [Tooltip("最大颜色值 (RGB, 0-1)")]
    public Color maxColor = new Color(0f, 1f, 0.5f); // 默认偏青绿

    void Start()
    {
        // 启动协程，循环生成
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // 这是一个死循环，只要脚本运行就在生成
        while (true)
        {
            SpawnOneNote();
            
            // 等待指定的时间
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnOneNote()
    {
        // 1. 计算随机生成位置
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
            0f
        );

        Vector3 spawnPosition = transform.position + randomOffset;

        // 2. 实例化生成音符
        GameObject note = Instantiate(notePrefab, spawnPosition, Quaternion.identity);

        // 3. 【核心需求 1】随机化长短
        // 我们通过改变 Y 轴的 Scale 来模拟音符的长短
        float randomLength = Random.Range(minLength, maxLength);
        note.transform.localScale = new Vector3(1f, randomLength, 1f); // 变长

        // 4. 【核心需求 2】随机化颜色
        MeshRenderer noteRenderer = note.GetComponent<MeshRenderer>();
        if (noteRenderer != null)
        {
            // 在 MinColor 和 MaxColor 之间随机插值，得到一个随机颜色
            Color randomColor = new Color(
                Random.Range(minColor.r, maxColor.r),
                Random.Range(minColor.g, maxColor.g),
                Random.Range(minColor.b, maxColor.b),
                1f // Alpha 固定为 1（不透明），如果你想要半透明可以改为 Random.Range(0.5f, 1f)
            );

            // 直接修改材质颜色
            // 注意：如果你希望每个预制体都是独立的材质副本，请用 noteRenderer.material
            // 如果你只是想临时改颜色且不影响共享材质，可以用 noteRenderer.sharedMaterial
            // 这里我们直接改 sharedMaterial 方便测试
            noteRenderer.material.color = randomColor;
        }
        
        // 5. (可选) 随机化飘动速度
        FloatingNote noteScript = note.GetComponent<FloatingNote>();
        if (noteScript != null)
        {
            noteScript.speed = Random.Range(5f, 8f);
        }
    }
    
    // 为了在 Scene 视图里看到生成范围，我们可以画一个线框框
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
