using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 实时心率监测系统
/// 模拟演讲时的心率变化，当心率过高时显示放松提示
/// </summary>
public class HeartRateMonitor : MonoBehaviour
{
    [Header("UI组件")]
    public TextMeshProUGUI heartRateText;           // 心率数值显示
    public Image heartIcon;                          // 心形图标
    public GameObject relaxPanel;                    // "Relax!"提示面板
    public Image heartRateFillBar;                   // 心率进度条
    
    [Header("心率设置")]
    public float baseHeartRate = 75f;                // 基础心率
    public float minHeartRate = 60f;                 // 最低心率
    public float maxHeartRate = 140f;                // 最高心率
    public float stressIncreaseRate = 0.5f;          // 压力增加速率
    public float relaxDecreaseRate = 0.3f;           // 放松降低速率
    public float highHeartRateThreshold = 120f;      // 高心率阈值（触发提示）
    
    [Header("心跳动画")]
    public float heartbeatAnimationSpeed = 1.0f;     // 心跳动画速度
    public float heartbeatScaleMin = 0.9f;           // 心跳缩放最小值
    public float heartbeatScaleMax = 1.1f;           // 心跳缩放最大值
    
    // 私有变量
    private float currentHeartRate;                  // 当前心率
    private bool isPresentationActive = false;       // 演讲是否进行中
    private bool isRelaxing = false;                 // 是否处于放松状态
    private float heartbeatTimer = 0f;               // 心跳计时器
    private Color normalHeartColor = new Color(1f, 0.3f, 0.3f);      // 正常心率颜色（红色）
    private Color highHeartColor = new Color(1f, 0f, 0f);            // 高心率颜色（深红色）
    
    void Start()
    {
        currentHeartRate = baseHeartRate;
        
        // 初始化UI引用（如果没有手动指定）
        if (heartRateText == null)
        {
            GameObject obj = GameObject.Find("HeartRateText");
            if (obj != null) heartRateText = obj.GetComponent<TextMeshProUGUI>();
        }
        
        if (heartIcon == null)
        {
            GameObject obj = GameObject.Find("HeartIcon");
            if (obj != null) heartIcon = obj.GetComponent<Image>();
        }
            
        if (relaxPanel != null)
            relaxPanel.SetActive(false);
        
        UpdateHeartRateUI();
    }
    
    void Update()
    {
        // 模拟心率变化
        SimulateHeartRate();
        
        // 更新心跳动画
        AnimateHeartbeat();
        
        // 更新UI
        UpdateHeartRateUI();
        
        // 检查是否需要显示放松提示
        CheckRelaxPrompt();
    }
    
    /// <summary>
    /// 模拟心率变化
    /// </summary>
    void SimulateHeartRate()
    {
        if (isPresentationActive)
        {
            // 演讲进行中，心率会随机波动并趋向上升
            float randomFactor = Random.Range(-2f, 5f);
            float targetRate = Mathf.Min(baseHeartRate + 30f, maxHeartRate);
            currentHeartRate += (randomFactor + stressIncreaseRate) * Time.deltaTime;
            
            // 添加自然波动
            currentHeartRate += Mathf.Sin(Time.time * 0.5f) * 0.5f;
        }
        else if (isRelaxing)
        {
            // 放松状态，心率下降
            currentHeartRate -= relaxDecreaseRate * 10f * Time.deltaTime;
        }
        else
        {
            // 静止状态，心率缓慢恢复到基础值
            currentHeartRate = Mathf.Lerp(currentHeartRate, baseHeartRate, Time.deltaTime * 0.5f);
        }
        
        // 限制心率范围
        currentHeartRate = Mathf.Clamp(currentHeartRate, minHeartRate, maxHeartRate);
    }
    
    /// <summary>
    /// 心跳动画效果
    /// </summary>
    void AnimateHeartbeat()
    {
        if (heartIcon == null) return;
        
        // 根据心率调整心跳速度
        float beatSpeed = (currentHeartRate / 60f) * heartbeatAnimationSpeed;
        heartbeatTimer += Time.deltaTime * beatSpeed;
        
        // 使用正弦波创建心跳效果
        float scale = Mathf.Lerp(heartbeatScaleMin, heartbeatScaleMax, 
            (Mathf.Sin(heartbeatTimer * Mathf.PI * 2f) + 1f) / 2f);
        
        heartIcon.transform.localScale = Vector3.one * scale;
        
        // 根据心率改变颜色
        Color targetColor = currentHeartRate > highHeartRateThreshold ? highHeartColor : normalHeartColor;
        heartIcon.color = Color.Lerp(heartIcon.color, targetColor, Time.deltaTime * 2f);
    }
    
    /// <summary>
    /// 更新心率UI显示
    /// </summary>
    void UpdateHeartRateUI()
    {
        if (heartRateText != null)
        {
            heartRateText.text = Mathf.RoundToInt(currentHeartRate) + " BPM";
            
            // 高心率时改变文字颜色
            if (currentHeartRate > highHeartRateThreshold)
            {
                heartRateText.color = Color.red;
            }
            else
            {
                heartRateText.color = Color.white;
            }
        }
        
        // 更新心率进度条
        if (heartRateFillBar != null)
        {
            float fillAmount = (currentHeartRate - minHeartRate) / (maxHeartRate - minHeartRate);
            heartRateFillBar.fillAmount = fillAmount;
            
            // 根据心率改变进度条颜色
            if (currentHeartRate > highHeartRateThreshold)
            {
                heartRateFillBar.color = Color.red;
            }
            else if (currentHeartRate > 100f)
            {
                heartRateFillBar.color = Color.yellow;
            }
            else
            {
                heartRateFillBar.color = Color.green;
            }
        }
    }
    
    /// <summary>
    /// 检查是否需要显示放松提示
    /// </summary>
    void CheckRelaxPrompt()
    {
        if (relaxPanel == null) return;
        
        if (currentHeartRate > highHeartRateThreshold && !isRelaxing)
        {
            relaxPanel.SetActive(true);
        }
        else
        {
            relaxPanel.SetActive(false);
        }
    }
    
    // ===== 公共接口 =====
    
    /// <summary>
    /// 开始演讲（激活心率监测）
    /// </summary>
    public void StartPresentation()
    {
        isPresentationActive = true;
        isRelaxing = false;
        Debug.Log("心率监测：演讲开始");
    }
    
    /// <summary>
    /// 停止演讲
    /// </summary>
    public void StopPresentation()
    {
        isPresentationActive = false;
        Debug.Log("心率监测：演讲结束");
    }
    
    /// <summary>
    /// 触发放松状态
    /// </summary>
    public void TriggerRelaxation()
    {
        StartCoroutine(RelaxationRoutine());
    }
    
    /// <summary>
    /// 放松协程（持续5秒）
    /// </summary>
    IEnumerator RelaxationRoutine()
    {
        isRelaxing = true;
        isPresentationActive = false;
        
       Debug.Log("深呼吸放松中...");
        yield return new WaitForSeconds(5f);
        
        isRelaxing = false;
        isPresentationActive = true;
        Debug.Log("放松结束，继续演讲");
    }
    
    /// <summary>
    /// 增加压力事件（如评委提问）
    /// </summary>
    public void AddStressEvent(float stressAmount)
    {
        currentHeartRate += stressAmount;
        currentHeartRate = Mathf.Clamp(currentHeartRate, minHeartRate, maxHeartRate);
        Debug.Log(string.Format("压力事件触发！心率上升到 {0} BPM", Mathf.RoundToInt(currentHeartRate)));
    }
    
    /// <summary>
    /// 获取当前心率
    /// </summary>
    public float GetCurrentHeartRate()
    {
        return currentHeartRate;
    }
    
    /// <summary>
    /// 获取平均心率（用于最终评估）
    /// </summary>
    public float GetAverageHeartRate()
    {
        // 简化实现，实际应该记录历史数据
        return (currentHeartRate + baseHeartRate) / 2f;
    }
}
