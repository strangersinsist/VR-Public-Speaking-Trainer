using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 观众专注度反馈系统
/// 根据演讲表现动态调整观众的专注程度
/// </summary>
public class AudienceAttentionManager : MonoBehaviour
{
    [Header("UI组件")]
    public TextMeshProUGUI attentionPercentText;     // 专注度百分比文本
    public Image attentionFillBar;                   // 专注度进度条
    public GameObject attentionPanel;                // 专注度面板
    
    [Header("观众对象")]
    public GameObject[] audienceMembers;             // 观众游戏对象数组
    
    [Header("专注度设置")]
    [Range(0f, 100f)]
    public float currentAttention = 80f;             // 当前专注度
    public float maxAttention = 100f;                // 最大专注度
    public float minAttention = 0f;                  // 最小专注度
    
    [Header("专注度变化速率")]
    public float attentionDecayRate = 5f;            // 自然衰减速率（每秒）
    public float goodPerformanceBonus = 10f;         // 良好表现加成
    public float poorPerformancePenalty = 15f;       // 差劲表现惩罚
    
    [Header("阈值设置")]
    public float highAttentionThreshold = 70f;       // 高专注度阈值
    public float mediumAttentionThreshold = 40f;     // 中等专注度阈值
    public float lowAttentionThreshold = 20f;        // 低专注度阈值
    
    [Header("音效")]
    public AudioClip[] whisperSounds;                // 窃窃私语音效
    public AudioClip[] coughSounds;                  // 咳嗽音效
    private AudioSource audioSource;
    
    // 观众状态
    public enum AudienceState
    {
        Attentive,      // 专注
        Bored,          // 无聊
        Sleeping,       // 打瞌睡
        OnPhone         // 玩手机
    }
    
    private Dictionary<GameObject, AudienceState> audienceStates = new Dictionary<GameObject, AudienceState>();
    private float timeSinceLastSound = 0f;
    private float soundInterval = 5f;                // 音效间隔
    private bool isPresentationActive = false;
    
    void Start()
    {
        // 初始化音频源
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.3f;
        
        // 自动查找观众对象（如果没有手动指定）
        if (audienceMembers == null || audienceMembers.Length == 0)
        {
            try
            {
                audienceMembers = GameObject.FindGameObjectsWithTag("Audience");
            }
            catch (UnityException)
            {
                // Tag不存在，跳过
                audienceMembers = new GameObject[0];
            }
            
            if (audienceMembers.Length == 0)
            {
                Debug.LogWarning("未找到标记为'Audience'的游戏对象。观众系统将使用模拟数据运行。");
            }
        }
        
        // 初始化观众状态
        foreach (var audience in audienceMembers)
        {
            if (audience != null)
            {
                audienceStates[audience] = AudienceState.Attentive;
            }
        }
        
        // 初始化UI
        if (attentionPercentText == null)
        {
            GameObject obj = GameObject.Find("AttentionPercentText");
            if (obj != null) attentionPercentText = obj.GetComponent<TextMeshProUGUI>();
        }
        
        if (attentionFillBar == null)
        {
            GameObject obj = GameObject.Find("AttentionFillBar");
            if (obj != null) attentionFillBar = obj.GetComponent<Image>();
        }
        
        UpdateAttentionUI();
    }
    
    void Update()
    {
        if (!isPresentationActive) return;
        
        // 专注度自然衰减
        currentAttention -= attentionDecayRate * Time.deltaTime;
        currentAttention = Mathf.Clamp(currentAttention, minAttention, maxAttention);
        
        // 更新观众状态
        UpdateAudienceStates();
        
        // 更新UI
        UpdateAttentionUI();
        
        // 播放环境音效
        PlayAmbientSounds();
    }
    
    /// <summary>
    /// 更新观众状态
    /// </summary>
    void UpdateAudienceStates()
    {
        if (audienceMembers == null || audienceMembers.Length == 0) return;
        
        foreach (var audience in audienceMembers)
        {
            if (audience == null) continue;
            
            // 根据专注度决定观众状态
            AudienceState newState;
            
            if (currentAttention >= highAttentionThreshold)
            {
                newState = AudienceState.Attentive;
            }
            else if (currentAttention >= mediumAttentionThreshold)
            {
                // 中等专注度：随机分配状态
                float random = Random.value;
                if (random < 0.6f)
                    newState = AudienceState.Attentive;
                else if (random < 0.9f)
                    newState = AudienceState.Bored;
                else
                    newState = AudienceState.OnPhone;
            }
            else if (currentAttention >= lowAttentionThreshold)
            {
                // 低专注度：大部分观众不专注
                float random = Random.value;
                if (random < 0.3f)
                    newState = AudienceState.Attentive;
                else if (random < 0.5f)
                    newState = AudienceState.Bored;
                else if (random < 0.8f)
                    newState = AudienceState.OnPhone;
                else
                    newState = AudienceState.Sleeping;
            }
            else
            {
                // 极低专注度：大多数人睡觉或玩手机
                float random = Random.value;
                if (random < 0.5f)
                    newState = AudienceState.Sleeping;
                else if (random < 0.9f)
                    newState = AudienceState.OnPhone;
                else
                    newState = AudienceState.Bored;
            }
            
            // 更新状态（如果有变化）
            if (!audienceStates.ContainsKey(audience) || audienceStates[audience] != newState)
            {
                audienceStates[audience] = newState;
                ApplyAudienceState(audience, newState);
            }
        }
    }
    
    /// <summary>
    /// 应用观众状态（可以在这里添加动画控制）
    /// </summary>
    void ApplyAudienceState(GameObject audience, AudienceState state)
    {
        // 如果有Animator组件，可以触发相应动画
        Animator animator = audience.GetComponent<Animator>();
        if (animator != null)
        {
            // 设置动画参数（需要在Animator Controller中预先配置）
            animator.SetInteger("AttentionState", (int)state);
        }
    }
    
    /// <summary>
    /// 更新专注度UI
    /// </summary>
    void UpdateAttentionUI()
    {
        // 更新百分比文本
        if (attentionPercentText != null)
        {
            attentionPercentText.text = "观众专注度: " + Mathf.RoundToInt(currentAttention) + "%";
            
            // 根据专注度改变文字颜色
            if (currentAttention >= highAttentionThreshold)
            {
                attentionPercentText.color = Color.green;
            }
            else if (currentAttention >= mediumAttentionThreshold)
            {
                attentionPercentText.color = Color.yellow;
            }
            else
            {
                attentionPercentText.color = Color.red;
            }
        }
        
        // 更新进度条
        if (attentionFillBar != null)
        {
            attentionFillBar.fillAmount = currentAttention / maxAttention;
            
            // 根据专注度改变进度条颜色
            if (currentAttention >= highAttentionThreshold)
            {
                attentionFillBar.color = Color.green;
            }
            else if (currentAttention >= mediumAttentionThreshold)
            {
                attentionFillBar.color = Color.yellow;
            }
            else
            {
                attentionFillBar.color = Color.red;
            }
        }
    }
    
    /// <summary>
    /// 播放环境音效
    /// </summary>
    void PlayAmbientSounds()
    {
        timeSinceLastSound += Time.deltaTime;
        
        if (timeSinceLastSound >= soundInterval)
        {
            timeSinceLastSound = 0f;
            
            // 根据专注度决定是否播放音效
            if (currentAttention < mediumAttentionThreshold)
            {
                float random = Random.value;
                
                // 低专注度时更频繁播放干扰音效
                if (random < 0.5f && whisperSounds != null && whisperSounds.Length > 0)
                {
                    AudioClip clip = whisperSounds[Random.Range(0, whisperSounds.Length)];
                    audioSource.PlayOneShot(clip, 0.5f);
                }
                else if (random < 0.7f && coughSounds != null && coughSounds.Length > 0)
                {
                    AudioClip clip = coughSounds[Random.Range(0, coughSounds.Length)];
                    audioSource.PlayOneShot(clip, 0.3f);
                }
            }
        }
    }
    
    // ===== 公共接口 =====
    
    /// <summary>
    /// 开始演讲
    /// </summary>
    public void StartPresentation()
    {
        isPresentationActive = true;
        currentAttention = 80f; // 初始专注度设为80%
        Debug.Log("观众专注度系统：演讲开始");
    }
    
    /// <summary>
    /// 停止演讲
    /// </summary>
    public void StopPresentation()
    {
        isPresentationActive = false;
        Debug.Log("观众专注度系统：演讲结束");
    }
    
    /// <summary>
    /// 良好表现（提升专注度）
    /// </summary>
    public void GoodPerformance()
    {
        currentAttention += goodPerformanceBonus;
        currentAttention = Mathf.Clamp(currentAttention, minAttention, maxAttention);
        Debug.Log(string.Format("良好表现！专注度提升到 {0}%", Mathf.RoundToInt(currentAttention)));
    }
    
    /// <summary>
    /// 差劲表现（降低专注度）
    /// </summary>
    public void PoorPerformance()
    {
        currentAttention -= poorPerformancePenalty;
        currentAttention = Mathf.Clamp(currentAttention, minAttention, maxAttention);
        Debug.Log(string.Format("表现不佳！专注度降低到 {0}%", Mathf.RoundToInt(currentAttention)));
    }
    
    /// <summary>
    /// 长时间沉默（大幅降低专注度）
    /// </summary>
    public void LongSilence()
    {
        currentAttention -= poorPerformancePenalty * 2f;
        currentAttention = Mathf.Clamp(currentAttention, minAttention, maxAttention);
        Debug.Log(string.Format("长时间沉默！专注度大幅降低到 {0}%", Mathf.RoundToInt(currentAttention)));
    }
    
    /// <summary>
    /// 精彩内容（大幅提升专注度）
    /// </summary>
    public void ExcellentContent()
    {
        currentAttention += goodPerformanceBonus * 2f;
        currentAttention = Mathf.Clamp(currentAttention, minAttention, maxAttention);
        Debug.Log(string.Format("精彩表现！专注度大幅提升到 {0}%", Mathf.RoundToInt(currentAttention)));
    }
    
    /// <summary>
    /// 获取当前专注度
    /// </summary>
    public float GetCurrentAttention()
    {
        return currentAttention;
    }
    
    /// <summary>
    /// 获取专注的观众数量
    /// </summary>
    public int GetAttentiveAudienceCount()
    {
        int count = 0;
        foreach (var state in audienceStates.Values)
        {
            if (state == AudienceState.Attentive)
                count++;
        }
        return count;
    }
    
    /// <summary>
    /// 获取观众总数
    /// </summary>
    public int GetTotalAudienceCount()
    {
        return audienceMembers != null ? audienceMembers.Length : 0;
    }
}
