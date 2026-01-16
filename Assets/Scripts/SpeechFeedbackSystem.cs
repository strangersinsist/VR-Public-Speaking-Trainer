using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 语音表达反馈系统
/// 模拟语音检测，提供实时反馈和字幕显示
/// </summary>
public class SpeechFeedbackSystem : MonoBehaviour
{
    [Header("UI组件")]
    public TextMeshProUGUI subtitleText;             // 实时字幕显示
    public TextMeshProUGUI fillerWordCountText;      // 填充词计数显示
    public GameObject stutterWarning;                // 卡壳警告提示
    public GameObject silenceWarning;                // 长时间沉默警告
    public Image feedbackIndicator;                  // 反馈指示器
    
    [Header("检测设置")]
    public float detectionInterval = 2f;             // 检测间隔（秒）
    public float stutterProbability = 0.15f;         // 卡壳概率
    public float silenceProbability = 0.1f;          // 沉默概率
    public float fillerWordProbability = 0.25f;      // 填充词概率
    
    [Header("填充词列表")]
    public string[] fillerWords = { "嗯", "啊", "然后", "那个", "就是", "这个", "呃" };
    
    [Header("模拟演讲内容")]
    public string[] speechPhrases = {
        "各位评委老师好，今天我演讲的主题是...",
        "我们的研究主要关注...",
        "通过实验数据可以看出...",
        "这项技术的创新点在于...",
        "未来我们计划...",
        "综上所述...",
        "感谢各位的聆听"
    };
    
    [Header("震动反馈模拟")]
    public float vibrationDuration = 0.3f;           // 震动持续时间
    
    // 私有变量
    private int fillerWordCount = 0;                 // 填充词计数
    private int stutterCount = 0;                    // 卡壳次数
    private int contentDeviationCount = 0;           // 内容偏离次数
    private float timeSinceLastDetection = 0f;
    private bool isPresentationActive = false;
    private int currentPhraseIndex = 0;
    private Coroutine subtitleCoroutine;
    
    // 引用其他系统
    private AudienceAttentionManager attentionManager;
    private HeartRateMonitor heartRateMonitor;
    
    void Start()
    {
        // 查找其他系统
        attentionManager = FindObjectOfType<AudienceAttentionManager>();
        heartRateMonitor = FindObjectOfType<HeartRateMonitor>();
        
        // 初始化UI
        if (subtitleText == null)
        {
            GameObject obj = GameObject.Find("SubtitleText");
            if (obj != null) subtitleText = obj.GetComponent<TextMeshProUGUI>();
        }
        
        if (fillerWordCountText == null)
        {
            GameObject obj = GameObject.Find("FillerWordCountText");
            if (obj != null) fillerWordCountText = obj.GetComponent<TextMeshProUGUI>();
        }
        
        // 隐藏警告
        if (stutterWarning != null)
            stutterWarning.SetActive(false);
        
        if (silenceWarning != null)
            silenceWarning.SetActive(false);
        
        UpdateFillerWordCountUI();
        
        if (subtitleText != null)
            subtitleText.text = "";
    }
    
    void Update()
    {
        if (!isPresentationActive) return;
        
        timeSinceLastDetection += Time.deltaTime;
        
        // 定期进行语音检测
        if (timeSinceLastDetection >= detectionInterval)
        {
            timeSinceLastDetection = 0f;
            PerformSpeechDetection();
        }
    }
    
    /// <summary>
    /// 执行语音检测（模拟）
    /// </summary>
    void PerformSpeechDetection()
    {
        float random = Random.value;
        
        // 检测填充词
        if (random < fillerWordProbability)
        {
            DetectFillerWord();
        }
        // 检测卡壳
        else if (random < fillerWordProbability + stutterProbability)
        {
            DetectStutter();
        }
        // 检测沉默
        else if (random < fillerWordProbability + stutterProbability + silenceProbability)
        {
            DetectSilence();
        }
        // 正常演讲
        else
        {
            NormalSpeech();
        }
    }
    
    /// <summary>
    /// 检测到填充词
    /// </summary>
    void DetectFillerWord()
    {
        fillerWordCount++;
        string filler = fillerWords[Random.Range(0, fillerWords.Length)];
        
        // 显示字幕
        if (subtitleText != null)
        {
            subtitleText.text = "<color=orange>" + filler + "...</color>";
            subtitleText.color = Color.yellow;
        }
        
        // 更新计数
        UpdateFillerWordCountUI();
        
        // 轻微震动反馈
        TriggerVibrationFeedback(0.5f);
        
        // 轻微影响观众专注度
        if (attentionManager != null && fillerWordCount % 3 == 0)
        {
            attentionManager.PoorPerformance();
        }
        
        Debug.Log(string.Format("检测到填充词: {0} (总计: {1})", filler, fillerWordCount));
    }
    
    /// <summary>
    /// 检测到卡壳/结巴
    /// </summary>
    void DetectStutter()
    {
        stutterCount++;
        
        // 显示警告
        if (stutterWarning != null)
        {
            StartCoroutine(ShowWarning(stutterWarning, 2f));
        }
        
        // 显示字幕
        if (subtitleText != null)
        {
            subtitleText.text = "<color=red>我我我...</color>";
            subtitleText.color = Color.red;
        }
        
        // 震动反馈
        TriggerVibrationFeedback(1f);
        
        // 影响观众专注度和心率
        if (attentionManager != null)
        {
            attentionManager.PoorPerformance();
        }
        
        if (heartRateMonitor != null)
        {
            heartRateMonitor.AddStressEvent(10f);
        }
        
        Debug.Log(string.Format("检测到卡壳！(总计: {0})", stutterCount));
    }
    
    /// <summary>
    /// 检测到长时间沉默
    /// </summary>
    void DetectSilence()
    {
        // 显示警告
        if (silenceWarning != null)
        {
            StartCoroutine(ShowWarning(silenceWarning, 3f));
        }
        
        // 显示字幕
        if (subtitleText != null)
        {
            subtitleText.text = "[长时间沉默...]";
            subtitleText.color = Color.gray;
        }
        
        // 严重影响观众专注度
        if (attentionManager != null)
        {
            attentionManager.LongSilence();
        }
        
        Debug.Log("检测到长时间沉默！");
    }
    
    /// <summary>
    /// 正常流畅的演讲
    /// </summary>
    void NormalSpeech()
    {
        // 显示演讲内容
        if (subtitleText != null && speechPhrases.Length > 0)
        {
            string phrase = speechPhrases[currentPhraseIndex];
            
            // 开始打字机效果
            if (subtitleCoroutine != null)
            {
                StopCoroutine(subtitleCoroutine);
            }
            subtitleCoroutine = StartCoroutine(TypewriterEffect(phrase));
            
            currentPhraseIndex = (currentPhraseIndex + 1) % speechPhrases.Length;
        }
        
        // 良好表现，提升观众专注度
        if (attentionManager != null && Random.value < 0.3f)
        {
            attentionManager.GoodPerformance();
        }
    }
    
    /// <summary>
    /// 打字机效果显示字幕
    /// </summary>
    IEnumerator TypewriterEffect(string text)
    {
        if (subtitleText == null) yield break;
        
        subtitleText.text = "";
        subtitleText.color = Color.white;
        
        foreach (char c in text)
        {
            subtitleText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    /// <summary>
    /// 显示警告提示
    /// </summary>
    IEnumerator ShowWarning(GameObject warning, float duration)
    {
        warning.SetActive(true);
        yield return new WaitForSeconds(duration);
        warning.SetActive(false);
    }
    
    /// <summary>
    /// 触发震动反馈（视觉模拟）
    /// </summary>
    void TriggerVibrationFeedback(float intensity)
    {
        if (feedbackIndicator != null)
        {
            StartCoroutine(VibrationEffect(intensity));
        }
    }
    
    /// <summary>
    /// 震动效果协程
    /// </summary>
    IEnumerator VibrationEffect(float intensity)
    {
        if (feedbackIndicator == null) yield break;
        
        // 闪烁效果模拟震动
        Color originalColor = feedbackIndicator.color;
        float elapsed = 0f;
        
        while (elapsed < vibrationDuration)
        {
            feedbackIndicator.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            feedbackIndicator.color = originalColor;
            yield return new WaitForSeconds(0.05f);
            elapsed += 0.1f;
        }
        
        feedbackIndicator.color = originalColor;
    }
    
    /// <summary>
    /// 更新填充词计数UI
    /// </summary>
    void UpdateFillerWordCountUI()
    {
        if (fillerWordCountText != null)
        {
            fillerWordCountText.text = "填充词: " + fillerWordCount;
            
            // 根据数量改变颜色
            if (fillerWordCount < 10)
                fillerWordCountText.color = Color.green;
            else if (fillerWordCount < 25)
                fillerWordCountText.color = Color.yellow;
            else
                fillerWordCountText.color = Color.red;
        }
    }
    
    // ===== 公共接口 =====
    
    /// <summary>
    /// 开始演讲
    /// </summary>
    public void StartPresentation()
    {
        isPresentationActive = true;
        fillerWordCount = 0;
        stutterCount = 0;
        contentDeviationCount = 0;
        currentPhraseIndex = 0;
        UpdateFillerWordCountUI();
        
        if (subtitleText != null)
            subtitleText.text = "准备开始演讲...";
        
        Debug.Log("语音反馈系统：演讲开始");
    }
    
    /// <summary>
    /// 停止演讲
    /// </summary>
    public void StopPresentation()
    {
        isPresentationActive = false;
        
        if (subtitleCoroutine != null)
        {
            StopCoroutine(subtitleCoroutine);
        }
        
        if (subtitleText != null)
            subtitleText.text = "";
        
        Debug.Log(string.Format("语音反馈系统：演讲结束。填充词: {0}, 卡壳: {1}", fillerWordCount, stutterCount));
    }
    
    /// <summary>
    /// 手动添加填充词
    /// </summary>
    public void AddFillerWord()
    {
        fillerWordCount++;
        UpdateFillerWordCountUI();
    }
    
    /// <summary>
    /// 手动添加内容偏离
    /// </summary>
    public void AddContentDeviation()
    {
        contentDeviationCount++;
        Debug.Log(string.Format("内容偏离次数: {0}", contentDeviationCount));
    }
    
    /// <summary>
    /// 获取填充词数量
    /// </summary>
    public int GetFillerWordCount()
    {
        return fillerWordCount;
    }
    
    /// <summary>
    /// 获取卡壳次数
    /// </summary>
    public int GetStutterCount()
    {
        return stutterCount;
    }
    
    /// <summary>
    /// 获取内容偏离次数
    /// </summary>
    public int GetContentDeviationCount()
    {
        return contentDeviationCount;
    }
    
    /// <summary>
    /// 模拟优秀表现（用于测试）
    /// </summary>
    public void SimulateExcellentPerformance()
    {
        if (attentionManager != null)
        {
            attentionManager.ExcellentContent();
        }
        
        NormalSpeech();
    }
}
