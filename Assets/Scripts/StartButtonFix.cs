using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 按钮连接器 - 确保Start按钮能正确启动演讲
/// </summary>
public class StartButtonFix : MonoBehaviour
{
    void Start()
    {
        // 查找Start and Pause按钮
        GameObject buttonObj = GameObject.Find("Start and Pause");
        if (buttonObj != null)
        {
            Button btn = buttonObj.GetComponent<Button>();
            if (btn != null)
            {
                // 清除所有旧的监听器
                btn.onClick.RemoveAllListeners();
                
                // 添加新的监听器
                btn.onClick.AddListener(OnStartButtonClick);
                
                Debug.Log("✓ Start按钮已重新连接到PresentationManager");
            }
        }
    }
    
    void OnStartButtonClick()
    {
        PresentationManager manager = FindObjectOfType<PresentationManager>();
        if (manager != null)
        {
            manager.StartPresentation();
            Debug.Log("=== 通过按钮启动演讲！===");
        }
        else
        {
            Debug.LogError("未找到PresentationManager！");
        }
    }
}
