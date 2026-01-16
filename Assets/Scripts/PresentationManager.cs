using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 演讲模拟主控制器
/// 统一管理所有子系统，控制演讲流程
/// </summary>
public class PresentationManager : MonoBehaviour
{
    [Header("子系统引用")]
    public HeartRateMonitor heartRateMonitor;
    public AudienceAttentionManager attentionManager;
    public PerformanceEvaluator performanceEvaluator;
    public SpeechFeedbackSystem speechFeedback;
    
    [Header("UI控制")]
    public Button startButton;                       // 开始按钮
    public Button stopButton;                        // 停止按钮
    public GameObject mainPanel;                     // 主面板
    public TextMeshProUGUI statusText;               // 状态文本
    public TextMeshProUGUI timerText;                // 计时器文本
    
    [Header("演讲设置")]
    public float presentationDuration = 300f;        // 演讲时长（秒，默认5分钟）
    public bool autoEvaluate = true;                 // 自动评估
    
    // 私有变量
    private bool isPresentationActive = false;
    private float presentationTime = 0f;             // 当前演讲时间
    private float startTime = 0f;
    
    void Start()
    {
        // 自动查找子系统
        if (heartRateMonitor == null)
            heartRateMonitor = FindObjectOfType<HeartRateMonitor>();
        
        if (attentionManager == null)
            attentionManager = FindObjectOfType<AudienceAttentionManager>();
        
        if (performanceEvaluator == null)
            performanceEvaluator = FindObjectOfType<PerformanceEvaluator>();
        
        if (speechFeedback == null)
            speechFeedback = FindObjectOfType<SpeechFeedbackSystem>();
        
        // 设置按钮监听
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartPresentation);
        }
        
        if (stopButton != null)
        {
            stopButton.onClick.AddListener(StopPresentation);
            stopButton.gameObject.SetActive(false);
        }
        
        // 初始化UI
        if (statusText != null)
            statusText.text = "准备就绪";
        
        if (timerText != null)
            timerText.text = "00:00";
        
        Debug.Log("=== VR演讲模拟器已初始化 ===");
        Debug.Log("子系统状态:");
        Debug.Log("  - 心率监测: " + (heartRateMonitor != null ? "✓" : "✗"));
        Debug.Log("  - 观众专注度: " + (attentionManager != null ? "✓" : "✗"));
        Debug.Log("  - 表现评估: " + (performanceEvaluator != null ? "✓" : "✗"));
        Debug.Log("  - 语音反馈: " + (speechFeedback != null ? "✓" : "✗"));
    }
    
    void Update()
    {
        if (isPresentationActive)
        {
            // 更新计时器
            presentationTime += Time.deltaTime;
            UpdateTimerDisplay();
            
            // 检查是否超时
            if (presentationTime >= presentationDuration)
            {
                StopPresentation();
            }
        }
    }
    
    /// <summary>
    /// 开始演讲
    /// </summary>
    public void StartPresentation()
    {
        isPresentationActive = true;
        presentationTime = 0f;
        startTime = Time.time;
        
        // 启动所有子系统
        if (heartRateMonitor != null)
            heartRateMonitor.StartPresentation();
        
        if (attentionManager != null)
            attentionManager.StartPresentation();
        
        if (speechFeedback != null)
            speechFeedback.StartPresentation();
        
        // 更新UI
        if (statusText != null)
            statusText.text = "演讲进行中...";
        
        if (startButton != null)
            startButton.gameObject.SetActive(false);
        
        if (stopButton != null)
            stopButton.gameObject.SetActive(true);
        
        Debug.Log("========================================");
        Debug.Log("演讲开始！");
        Debug.Log(string.Format("预计时长: {0}秒 ({1}分钟)", presentationDuration, (presentationDuration/60f).ToString("F1")));
        Debug.Log("========================================");
    }
    
    /// <summary>
    /// 停止演讲
    /// </summary>
    public void StopPresentation()
    {
        if (!isPresentationActive) return;
        
        isPresentationActive = false;
        
        // 停止所有子系统
        if (heartRateMonitor != null)
            heartRateMonitor.StopPresentation();
        
        if (attentionManager != null)
            attentionManager.StopPresentation();
        
        if (speechFeedback != null)
            speechFeedback.StopPresentation();
        
        // 更新UI
        if (statusText != null)
            statusText.text = "演讲结束";
        
        if (startButton != null)
            startButton.gameObject.SetActive(true);
        
        if (stopButton != null)
            stopButton.gameObject.SetActive(false);
        
        Debug.Log("========================================");
        Debug.Log("演讲结束！");
        Debug.Log(string.Format("实际时长: {0}秒 ({1}分钟)", presentationTime.ToString("F1"), (presentationTime/60f).ToString("F1")));
        Debug.Log("========================================");
        
        // 延迟1秒后显示评估
        if (autoEvaluate && performanceEvaluator != null)
        {
            StartCoroutine(ShowEvaluationDelayed(1f));
        }
    }
    
    /// <summary>
    /// 延迟显示评估
    /// </summary>
    IEnumerator ShowEvaluationDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (statusText != null)
            statusText.text = "正在生成评估报告...";
        
        yield return new WaitForSeconds(0.5f);
        
        if (performanceEvaluator != null)
        {
            performanceEvaluator.StartEvaluation();
        }
    }
    
    /// <summary>
    /// 更新计时器显示
    /// </summary>
    void UpdateTimerDisplay()
    {
        if (timerText == null) return;
        
        int minutes = Mathf.FloorToInt(presentationTime / 60f);
        int seconds = Mathf.FloorToInt(presentationTime % 60f);
        
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        // 快超时时改变颜色
        if (presentationTime >= presentationDuration * 0.9f)
        {
            timerText.color = Color.red;
        }
        else if (presentationTime >= presentationDuration * 0.75f)
        {
            timerText.color = Color.yellow;
        }
        else
        {
            timerText.color = Color.white;
        }
    }
    
    /// <summary>
    /// 暂停演讲
    /// </summary>
    public void PausePresentation()
    {
        Time.timeScale = 0f;
        Debug.Log("演讲已暂停");
    }
    
    /// <summary>
    /// 恢复演讲
    /// </summary>
    public void ResumePresentation()
    {
        Time.timeScale = 1f;
        Debug.Log("演讲已恢复");
    }
    
    /// <summary>
    /// 重置所有系统
    /// </summary>
    public void ResetAllSystems()
    {
        if (isPresentationActive)
        {
            StopPresentation();
        }
        
        presentationTime = 0f;
        
        if (timerText != null)
            timerText.text = "00:00";
        
        if (statusText != null)
            statusText.text = "已重置";
        
        Debug.Log("所有系统已重置");
    }
    
    // ===== 公共接口 =====
    
    /// <summary>
    /// 获取当前演讲时间
    /// </summary>
    public float GetPresentationTime()
    {
        return presentationTime;
    }
    
    /// <summary>
    /// 是否正在演讲
    /// </summary>
    public bool IsPresenting()
    {
        return isPresentationActive;
    }
    
    /// <summary>
    /// 手动触发评估
    /// </summary>
    public void TriggerEvaluation()
    {
        if (performanceEvaluator != null)
        {
            performanceEvaluator.StartEvaluation();
        }
    }
    
    /// <summary>
    /// 设置演讲时长
    /// </summary>
    public void SetPresentationDuration(float seconds)
    {
        presentationDuration = seconds;
        Debug.Log(string.Format("演讲时长设置为: {0}秒 ({1}分钟)", seconds, (seconds/60f).ToString("F1")));
    }
    
    // ===== 快捷测试函数 =====
    
    /// <summary>
    /// 模拟优秀表现（用于测试）
    /// </summary>
    public void SimulateExcellentPerformance()
    {
        if (speechFeedback != null)
            speechFeedback.SimulateExcellentPerformance();
        
        if (attentionManager != null)
            attentionManager.ExcellentContent();
    }
    
    /// <summary>
    /// 模拟压力事件（用于测试）
    /// </summary>
    public void SimulateStressEvent()
    {
        if (heartRateMonitor != null)
            heartRateMonitor.AddStressEvent(20f);
    }
    
    /// <summary>
    /// 快速测试模式（30秒演讲）
    /// </summary>
    public void QuickTestMode()
    {
        SetPresentationDuration(30f);
        StartPresentation();
        Debug.Log("=== 快速测试模式激活（30秒）===");
    }
}
