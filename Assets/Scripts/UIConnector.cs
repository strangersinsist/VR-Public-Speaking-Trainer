using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI连接器 - 将原有的按钮和计时器连接到新的演讲系统
/// 挂载到任意GameObject上即可
/// </summary>
public class UIConnector : MonoBehaviour
{
    [Header("会自动查找这些对象")]
    public Button startPauseButton;          // 原有的"Start and Pause"按钮
    public timer originalTimer;              // 原有的timer脚本
    public PresentationManager presentationManager;
    
    private bool isPresenting = false;
    
    void Start()
    {
        // 自动查找原有的按钮
        if (startPauseButton == null)
        {
            GameObject btnObj = GameObject.Find("Start and Pause");
            if (btnObj != null)
            {
                startPauseButton = btnObj.GetComponent<Button>();
            }
        }
        
        // 自动查找timer脚本
        if (originalTimer == null)
        {
            originalTimer = FindObjectOfType<timer>();
        }
        
        // 自动查找PresentationManager
        if (presentationManager == null)
        {
            presentationManager = FindObjectOfType<PresentationManager>();
        }
        
        // 连接按钮事件
        if (startPauseButton != null)
        {
            startPauseButton.onClick.RemoveAllListeners(); // 清除原有事件
            startPauseButton.onClick.AddListener(OnStartPauseClick);
            Debug.Log("✓ 已连接原有按钮到新系统");
        }
        else
        {
            Debug.LogWarning("未找到'Start and Pause'按钮");
        }
    }
    
    /// <summary>
    /// 按钮点击事件
    /// </summary>
    void OnStartPauseClick()
    {
        if (presentationManager == null)
        {
            Debug.LogError("未找到PresentationManager!");
            return;
        }
        
        if (!isPresenting)
        {
            // 开始演讲
            presentationManager.StartPresentation();
            
            // 同时启动原有计时器
            if (originalTimer != null)
            {
                originalTimer.paused = false;
            }
            
            isPresenting = true;
            Debug.Log("▶ 演讲开始（通过原有按钮）");
        }
        else
        {
            // 停止演讲
            presentationManager.StopPresentation();
            
            // 暂停原有计时器
            if (originalTimer != null)
            {
                originalTimer.paused = true;
            }
            
            isPresenting = false;
            Debug.Log("⏸ 演讲暂停（通过原有按钮）");
        }
    }
    
    /// <summary>
    /// 重置系统
    /// </summary>
    public void ResetAll()
    {
        if (presentationManager != null)
        {
            presentationManager.ResetAllSystems();
        }
        
        if (originalTimer != null)
        {
            originalTimer.paused = true;
            originalTimer.totalTime = 60f * 5f; // 重置为5分钟
        }
        
        isPresenting = false;
        Debug.Log("🔄 系统已重置");
    }
}
