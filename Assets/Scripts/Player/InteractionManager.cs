using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteractionManager : MonoBehaviour
{
    [Header("=== 设置 ===")]
    public Camera playerCamera;
    public float interactDistance = 5f;
    public UnityEngine.UI.Text interactionTipText;
    private IInteractable _currentInteractable;
    public bool IsUIActive { get; set; } = false;

    void Update()
    {
        if (IsUIActive)
        {
            // UI 打开时，确保提示文字是隐藏的
            HideTip(); 
            return;
        }

        if (playerCamera == null) playerCamera = Camera.main;
        if (playerCamera == null) return;

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool hitSomething = Physics.Raycast(ray, out hit, interactDistance);

        if (hitSomething)
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                if (_currentInteractable != interactable)
                {
                    _currentInteractable = interactable;
                    ShowTip(_currentInteractable.GetInteractionText());
                }

                if (Input.GetMouseButtonDown(0))
                {
                    _currentInteractable.Interact();
                }
                return;
            }
        }

        // 击中空地或非交互物体
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
    }

    public void HideTip()
    {
        if (interactionTipText != null)
        {
            interactionTipText.gameObject.SetActive(false);
        }
    }
}
