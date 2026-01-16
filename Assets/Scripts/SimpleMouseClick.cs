using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// 鼠标点击交互脚本 - 支持UI Canvas按钮
/// 将此脚本挂载到任意对象上即可（建议挂到摄像机）
/// </summary>
public class SimpleMouseClick : MonoBehaviour
{
    public float maxClickDistance = 100f;
    
    private Camera mainCamera;
    private EventSystem eventSystem;
    
    void Start()
    {
        // 获取摄像机
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        
        // 获取或创建EventSystem
        eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystem = eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
            Debug.Log("已自动创建EventSystem");
        }
    }
    
    void Update()
    {
        // 鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            TryClickButton();
        }
        
        // 空格键快捷键
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryClickButtonDirect();
        }
    }
    
    void TryClickButton()
    {
        // 方法1: 使用UI EventSystem检测点击
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };
        
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        
        foreach (RaycastResult result in results)
        {
            Button button = result.gameObject.GetComponent<Button>();
            if (button != null && button.interactable)
            {
                button.onClick.Invoke();
                Debug.Log("✓ UI点击成功: " + button.gameObject.name);
                return;
            }
        }
        
        // 方法2: 如果UI没找到，尝试3D物体
        TryClick3DButton();
    }
    
    void TryClick3DButton()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, maxClickDistance))
        {
            Button button = hit.collider.GetComponentInParent<Button>();
            if (button != null && button.interactable)
            {
                button.onClick.Invoke();
                Debug.Log("✓ 3D按钮点击成功: " + button.gameObject.name);
            }
        }
    }
    
    // 直接找到按钮并点击（用于快捷键）
    void TryClickButtonDirect()
    {
        Button button = FindObjectOfType<Button>();
        if (button != null && button.interactable)
        {
            button.onClick.Invoke();
            Debug.Log("✓ 快捷键触发: " + button.gameObject.name);
        }
    }
}
