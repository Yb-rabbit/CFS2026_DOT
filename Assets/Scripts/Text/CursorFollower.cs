using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CursorFollower : MonoBehaviour, IPointerDownHandler
{
    [Header("=== 核心设置 ===")]
    [Tooltip("拖拽黑点 Image 的 RectTransform 到这里")]
    public RectTransform rectTransform; 

    [Header("=== 行为模式 ===")]
    [Tooltip("开启后，按鼠标左键点击屏幕任意位置，黑点会瞬移过去")]
    public bool enableClickToMove = true; 

    [Tooltip("开启后，鼠标移动时黑点会实时跟随")]
    public bool enableRealTimeFollow = false; 

    private Canvas _parentCanvas;
    private Camera _eventCamera;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        _parentCanvas = GetComponentInParent<Canvas>();
        
        if (_parentCanvas != null && _parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            _eventCamera = Camera.main;
        }
    }

    // 当检测到点击时调用
    public void OnPointerDown(PointerEventData eventData)
    {
        if (enableClickToMove)
        {
            MoveToPoint(eventData.position, eventData.pressEventCamera);
        }
    }

    void Update()
    {
        if (enableRealTimeFollow)
        {
            Vector2 mousePos = Input.mousePosition;
            // 简单的 Overlay 模式实时跟随（不涉及复杂转换）
            if (_parentCanvas != null && _parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                 rectTransform.anchoredPosition = mousePos;
            }
        }
    }

    /// <summary>
    /// 核心功能：将屏幕上的点击位置转换为 UI 内部的本地坐标
    /// </summary>
    private void MoveToPoint(Vector2 screenPoint, Camera camera)
    {
        if (rectTransform == null || _parentCanvas == null || camera == null) return;

        Vector2 localPoint;

        // === 模式 A：Screen Space - Overlay (最常见，最简单) ===
        if (_parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Overlay 模式：直接使用屏幕坐标
            rectTransform.anchoredPosition = screenPoint;
        }
        // === 模式 B：Screen Space - Camera (需要转换) ===
        else
        {
            // === 【关键修复】计算 Canvas 距离相机的真实距离 ===
            float distanceToCanvas = Vector3.Distance(camera.transform.position, _parentCanvas.transform.position);

            // 使用该距离作为深度进行屏幕转世界坐标的转换
            // 这样可以确保点击点准确落在 Canvas 的平面上
            Vector3 worldPoint = camera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, distanceToCanvas));

            // 将世界坐标转换为 RectTransform 的本地坐标
            localPoint = rectTransform.InverseTransformPoint(worldPoint);
            
            rectTransform.anchoredPosition = localPoint;
        }
    }
}
