using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    [Header("=== 设置 ===")]
    [Tooltip("拖拽这里的相机。如果是 Cinemachine，通常拖拽 CinemachineBrain 绑定的那个 Camera")]
    public Camera playerCamera;

    [Tooltip("射线检测的距离")]
    public float interactDistance = 5f;

    [Tooltip("用于显示提示的 UI Text 组件（可选）")]
    public UnityEngine.UI.Text interactionTipText;

    // 当前选中的交互物体
    private IInteractable _currentInteractable;

    void Update()
    {
        // 确保有相机
        if (playerCamera == null) playerCamera = Camera.main;
        if (playerCamera == null) return;

        // 1. 从屏幕中心（或鼠标位置）发射射线
        // 这里我们用鼠标位置，方便点击
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 射线检测：只检测距离内的物体
        bool hitSomething = Physics.Raycast(ray, out hit, interactDistance);

        // 2. 检查击中的物体是否实现了 IInteractable 接口
        if (hitSomething)
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                // 找到了可交互物体
                if (_currentInteractable != interactable)
                {
                    _currentInteractable = interactable;
                    ShowTip(_currentInteractable.GetInteractionText());
                }

                // 3. 检测点击输入
                if (Input.GetMouseButtonDown(0)) // 左键点击
                {
                    _currentInteractable.Interact();
                }
                return;
            }
        }

        // 如果什么都没击中，或者击中的不是交互物体
        if (_currentInteractable != null)
        {
            _currentInteractable = null;
            HideTip();
        }
    }

    private void ShowTip(string text)
    {
        if (interactionTipText != null)
        {
            interactionTipText.text = text;
            interactionTipText.gameObject.SetActive(true);
        }
        // 如果没有 UI，也可以用 Debug.Log 代替，方便测试
        else
        {
            // Debug.Log("交互提示: " + text); 
        }
    }

    private void HideTip()
    {
        if (interactionTipText != null)
        {
            interactionTipText.gameObject.SetActive(false);
        }
    }
}
