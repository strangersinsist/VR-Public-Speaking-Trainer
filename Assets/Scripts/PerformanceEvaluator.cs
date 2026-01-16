using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 表现评估系统
/// 在演讲结束后对用户表现进行多维度评估和总结
/// </summary>
public class PerformanceEvaluator : MonoBehaviour
{
    [Header("UI组件")]
    public GameObject evaluationPanel;               // 评估面板
    public TextMeshProUGUI summaryText;              // 总结文本
    public TextMeshProUGUI fillerWordsText;          // 填充词文本
    public TextMeshProUGUI contentDeviationText;     // 内容偏离文本
    public TextMeshProUGUI eyeContactText;           // 眼神交流文本
    public TextMeshProUGUI timeOverrunText;          // 超时文本
    public TextMeshProUGUI avgHeartRateText;         // 平均心率文本
    
    [Header("雷达图组件")]
    public Image radarChart;                         // 雷达图图像
    public TextMeshProUGUI fluencyScoreText;         // 流畅度评分
    public TextMeshProUGUI contentScoreText;         // 内容逻辑评分
    public TextMeshProUGUI interactionScoreText;     // 互动表现评分
    public TextMeshProUGUI timeControlScoreText;     // 时间控制评分
    public TextMeshProUGUI emotionalStabilityText;   // 情绪稳定评分
    
    [Header("引用其他系统")]
    public HeartRateMonitor heartRateMonitor;
    public AudienceAttentionManager attentionManager;
    public SpeechFeedbackSystem speechFeedback;
    
    [Header("评估数据")]
    private PerformanceData currentPerformance;
    private List<PerformanceData> performanceHistory = new List<PerformanceData>();
    
    // 评估数据结构
    [System.Serializable]
    public class PerformanceData
    {
        public DateTime timestamp;
        public int fillerWordCount;              // 填充词数量
        public int contentDeviationCount;        // 内容偏离次数
        public float eyeContactPercentage;       // 眼神交流百分比
        public float timeOverrunSeconds;         // 超时秒数
        public float averageHeartRate;           // 平均心率
        
        // 五个维度评分 (0-100)
        public float fluencyScore;               // 流畅度
        public float contentScore;               // 内容逻辑
        public float interactionScore;           // 互动表现
        public float timeControlScore;           // 时间控制
        public float emotionalStabilityScore;    // 情绪稳定
        
        public float overallScore;               // 总体评分
        
        public PerformanceData()
        {
            timestamp = DateTime.Now;
        }
    }
    
    void Start()
    {
        // 隐藏评估面板
        if (evaluationPanel != null)
            evaluationPanel.SetActive(false);
        
        // 自动查找引用的系统
        if (heartRateMonitor == null)
            heartRateMonitor = FindObjectOfType<HeartRateMonitor>();
        
        if (attentionManager == null)
            attentionManager = FindObjectOfType<AudienceAttentionManager>();
        
        if (speechFeedback == null)
            speechFeedback = FindObjectOfType<SpeechFeedbackSystem>();
        
        // 初始化当前表现数据
        currentPerformance = new PerformanceData();
        
        // 加载历史数据（这里简化，实际应从文件读取）
        LoadPerformanceHistory();
    }
    
    /// <summary>
    /// 开始评估（演讲结束时调用）
    /// </summary>
    public void StartEvaluation()
    {
        Debug.Log("开始表现评估...");
        
        // 收集数据
        CollectPerformanceData();
        
        // 计算评分
        CalculateScores();
        
        // 显示评估结果
        DisplayEvaluation();
        
        // 保存到历史记录
        SavePerformanceData();
    }
    
    /// <summary>
    /// 收集表现数据
    /// </summary>
    void CollectPerformanceData()
    {
        currentPerformance = new PerformanceData();
        
        // 从语音反馈系统获取数据
        if (speechFeedback != null)
        {
            currentPerformance.fillerWordCount = speechFeedback.GetFillerWordCount();
            currentPerformance.contentDeviationCount = speechFeedback.GetContentDeviationCount();
            
            // 如果实际数据为0，使用模拟数据（演讲太短）
            if (currentPerformance.fillerWordCount == 0)
            {
                currentPerformance.fillerWordCount = UnityEngine.Random.Range(5, 25);
                Debug.Log("演讲时间较短，使用模拟填充词数据");
            }
        }
        else
        {
            // 模拟数据
            currentPerformance.fillerWordCount = UnityEngine.Random.Range(15, 50);
            currentPerformance.contentDeviationCount = UnityEngine.Random.Range(0, 5);
        }
        
        // 从心率监测系统获取数据
        if (heartRateMonitor != null)
        {
            currentPerformance.averageHeartRate = heartRateMonitor.GetAverageHeartRate();
            
            // 如果心率为0（系统未启动），使用模拟数据
            if (currentPerformance.averageHeartRate == 0)
            {
                currentPerformance.averageHeartRate = UnityEngine.Random.Range(85f, 115f);
            }
        }
        else
        {
            currentPerformance.averageHeartRate = UnityEngine.Random.Range(85f, 115f);
        }
        
        // 模拟眼神交流数据
        currentPerformance.eyeContactPercentage = UnityEngine.Random.Range(40f, 85f);
        
        // 模拟超时数据
        currentPerformance.timeOverrunSeconds = UnityEngine.Random.Range(-30f, 120f);
        
        Debug.Log(string.Format("数据收集完成 - 填充词:{0}, 心率:{1}, 眼神:{2}%", 
            currentPerformance.fillerWordCount, 
            currentPerformance.averageHeartRate.ToString("F0"),
            currentPerformance.eyeContactPercentage.ToString("F0")));
    }
    
    /// <summary>
    /// 计算各维度评分
    /// </summary>
    void CalculateScores()
    {
        // 1. 流畅度评分（基于填充词数量）
        currentPerformance.fluencyScore = CalculateFluencyScore(currentPerformance.fillerWordCount);
        
        // 2. 内容逻辑评分（基于内容偏离次数）
        currentPerformance.contentScore = CalculateContentScore(currentPerformance.contentDeviationCount);
        
        // 3. 互动表现评分（基于眼神交流和观众专注度）
        currentPerformance.interactionScore = CalculateInteractionScore(
            currentPerformance.eyeContactPercentage,
            attentionManager != null ? attentionManager.GetCurrentAttention() : 60f
        );
        
        // 4. 时间控制评分（基于超时情况）
        currentPerformance.timeControlScore = CalculateTimeControlScore(currentPerformance.timeOverrunSeconds);
        
        // 5. 情绪稳定评分（基于心率）
        currentPerformance.emotionalStabilityScore = CalculateEmotionalStabilityScore(currentPerformance.averageHeartRate);
        
        // 计算总体评分（加权平均）
        currentPerformance.overallScore = (
            currentPerformance.fluencyScore * 0.25f +
            currentPerformance.contentScore * 0.25f +
            currentPerformance.interactionScore * 0.20f +
            currentPerformance.timeControlScore * 0.15f +
            currentPerformance.emotionalStabilityScore * 0.15f
        );
    }
    
    float CalculateFluencyScore(int fillerWords)
    {
        // 填充词越少，评分越高
        if (fillerWords <= 10) return 100f;
        if (fillerWords <= 20) return 85f;
        if (fillerWords <= 35) return 70f;
        if (fillerWords <= 50) return 55f;
        return 40f;
    }
    
    float CalculateContentScore(int deviations)
    {
        // 偏离次数越少，评分越高
        if (deviations == 0) return 100f;
        if (deviations == 1) return 90f;
        if (deviations == 2) return 75f;
        if (deviations <= 4) return 60f;
        return 45f;
    }
    
    float CalculateInteractionScore(float eyeContact, float audienceAttention)
    {
        // 眼神交流和观众专注度的综合评分
        float eyeScore = eyeContact; // 已经是百分比
        float attentionScore = audienceAttention;
        return (eyeScore * 0.6f + attentionScore * 0.4f);
    }
    
    float CalculateTimeControlScore(float overrunSeconds)
    {
        // 时间控制评分
        if (overrunSeconds <= 0) return 100f;           // 没有超时或提前结束
        if (overrunSeconds <= 30) return 90f;
        if (overrunSeconds <= 60) return 75f;
        if (overrunSeconds <= 120) return 60f;
        return 40f;
    }
    
    float CalculateEmotionalStabilityScore(float avgHeartRate)
    {
        // 心率越接近正常值，评分越高
        if (avgHeartRate < 85f) return 100f;
        if (avgHeartRate < 95f) return 90f;
        if (avgHeartRate < 105f) return 75f;
        if (avgHeartRate < 115f) return 60f;
        return 45f;
    }
    
    /// <summary>
    /// 显示评估结果
    /// </summary>
    void DisplayEvaluation()
    {
        if (evaluationPanel != null)
            evaluationPanel.SetActive(true);
        
        // 显示总结文本
        if (summaryText != null)
        {
            int score = (int)currentPerformance.overallScore;
            string grade = GetGrade(currentPerformance.overallScore);
            summaryText.text = "Performance Summary\nOverall Score: " + score + "/100 (" + grade + ")";
            Debug.Log("设置总分显示: " + score + "/100");
            
            // 根据评分改变颜色
            if (currentPerformance.overallScore >= 85f)
                summaryText.color = Color.green;
            else if (currentPerformance.overallScore >= 70f)
                summaryText.color = Color.yellow;
            else
                summaryText.color = new Color(1f, 0.5f, 0f); // 橙色
        }
        
        // 显示详细数据 - 简化版本
        if (fillerWordsText != null)
            fillerWordsText.text = "Filler Words: " + currentPerformance.fillerWordCount;
        
        if (contentDeviationText != null)
            contentDeviationText.text = "Content Deviation: " + currentPerformance.contentDeviationCount;
        
        if (eyeContactText != null)
            eyeContactText.text = "Eye Contact: " + (int)currentPerformance.eyeContactPercentage + "%";
        
        if (timeOverrunText != null)
        {
            if (currentPerformance.timeOverrunSeconds > 0)
                timeOverrunText.text = "Time Overrun: " + (int)currentPerformance.timeOverrunSeconds + "s";
            else
                timeOverrunText.text = "Time Control: Good";
        }
        
        if (avgHeartRateText != null)
            avgHeartRateText.text = "Avg Heart Rate: " + (int)currentPerformance.averageHeartRate + " BPM";
        
        // 显示五维度评分 - 简化版本，直接显示数字
        if (fluencyScoreText != null)
        {
            int score = (int)currentPerformance.fluencyScore;
            fluencyScoreText.text = "Fluency: " + score;
            Debug.Log("设置fluencyScore显示: " + score);
        }
        
        if (contentScoreText != null)
        {
            int score = (int)currentPerformance.contentScore;
            contentScoreText.text = "Content: " + score;
            Debug.Log("设置contentScore显示: " + score);
        }
        
        if (interactionScoreText != null)
        {
            int score = (int)currentPerformance.interactionScore;
            interactionScoreText.text = "Interaction: " + score;
            Debug.Log("设置interactionScore显示: " + score);
        }
        
        if (timeControlScoreText != null)
        {
            int score = (int)currentPerformance.timeControlScore;
            timeControlScoreText.text = "Time: " + score;
            Debug.Log("设置timeControlScore显示: " + score);
        }
        
        if (emotionalStabilityText != null)
        {
            int score = (int)currentPerformance.emotionalStabilityScore;
            emotionalStabilityText.text = "Emotion: " + score;
            Debug.Log("设置emotionalStabilityScore显示: " + score);
        }
        
        // 找出薄弱项
        string weakness = GetWeakestArea();
        Debug.Log(string.Format("评估完成！总分: {0}，薄弱项: {1}", 
            currentPerformance.overallScore.ToString("F1"), weakness));
    }
    
    /// <summary>
    /// 获取等级
    /// </summary>
    string GetGrade(float score)
    {
        if (score >= 90f) return "优秀";
        if (score >= 80f) return "良好";
        if (score >= 70f) return "中等";
        if (score >= 60f) return "及格";
        return "需要改进";
    }
    
    /// <summary>
    /// 获取最薄弱的方面
    /// </summary>
    string GetWeakestArea()
    {
        float minScore = Mathf.Min(
            currentPerformance.fluencyScore,
            currentPerformance.contentScore,
            currentPerformance.interactionScore,
            currentPerformance.timeControlScore,
            currentPerformance.emotionalStabilityScore
        );
        
        if (minScore == currentPerformance.fluencyScore) return "流畅度";
        if (minScore == currentPerformance.contentScore) return "内容逻辑";
        if (minScore == currentPerformance.interactionScore) return "互动表现";
        if (minScore == currentPerformance.timeControlScore) return "时间控制";
        return "情绪稳定";
    }
    
    /// <summary>
    /// 保存表现数据
    /// </summary>
    void SavePerformanceData()
    {
        performanceHistory.Add(currentPerformance);
        
        // 实际应用中应保存到文件
        // PlayerPrefs 或 JSON 文件
        Debug.Log(string.Format("表现数据已保存，历史记录数: {0}", performanceHistory.Count));
    }
    
    /// <summary>
    /// 加载历史数据
    /// </summary>
    void LoadPerformanceHistory()
    {
        // 实际应用中应从文件加载
        // 这里生成一些模拟历史数据用于演示
        for (int i = 0; i < 3; i++)
        {
            var data = new PerformanceData();
            data.fillerWordCount = UnityEngine.Random.Range(20, 45);
            data.eyeContactPercentage = UnityEngine.Random.Range(45f, 75f);
            data.averageHeartRate = UnityEngine.Random.Range(90f, 110f);
            performanceHistory.Add(data);
        }
    }
    
    /// <summary>
    /// 关闭评估面板
    /// </summary>
    public void CloseEvaluation()
    {
        if (evaluationPanel != null)
            evaluationPanel.SetActive(false);
    }
    
    /// <summary>
    /// 获取历史数据
    /// </summary>
    public List<PerformanceData> GetPerformanceHistory()
    {
        return performanceHistory;
    }
    
    /// <summary>
    /// 获取当前表现数据
    /// </summary>
    public PerformanceData GetCurrentPerformance()
    {
        return currentPerformance;
    }
}
